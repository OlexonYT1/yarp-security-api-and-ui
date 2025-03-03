import {
	HubConnection,
	HubConnectionBuilder,
	HubConnectionState,
	LogLevel,
	HttpTransportType
} from '@microsoft/signalr';
import { browser } from '$app/environment';

//TODO: no log on prod.
const enableLogging = true;
/**
 * Defines error types for the SignalR connection
 */
export type SignalRErrorType =
	| 'CONNECTION_ERROR'
	| 'DISCONNECTION_ERROR'
	| 'INVOCATION_ERROR'
	| 'HANDLER_ERROR'
	| 'RECONNECTION_ERROR';

/**
 * Structured error object for SignalR operations
 */
export type SignalRError = {
	type: SignalRErrorType;
	message: string;
	timestamp: Date;
	details?: unknown;
};

/**
 * Options for configuring a SignalR connection
 */
export type SignalRConnectionOptions = {
	/** Hub URL to connect to */
	hubUrl: string;
	/** Optional authentication token callback method */
	authTokenCallback: () => Promise<string>;
	/** Auto-reconnect intervals in milliseconds */
	reconnectIntervals?: number[];
	/** Whether to enable detailed logging */
	/** Whether to auto-connect when the class is instantiated */
	autoConnect?: boolean;
	/** Whether to use WebSockets transport only */
	useWebSocketsOnly?: boolean;
	/** Whether to skip negotiation phase (useful for CORS issues) */
	skipNegotiation?: boolean;
	/** Optional callback for handling errors */
	onError?: (error: SignalRError) => void;
};

/**
 * SignalR connection state manager
 */
export class SignalRConnection {
	// State properties using Svelte 5 $state rune
	connection = $state<HubConnection | null>(null);
	connectionStatus = $state<
		'Connected' | 'Connecting' | 'Reconnecting' | 'Disconnected' | 'Failed'
	>('Disconnected');

	// Error handling state
	errors = $state<SignalRError[]>([]);
	latestError = $state<SignalRError | null>(null);

	// Connection metrics
	connectionAttempts = $state(0);
	reconnectionAttempts = $state(0);
	lastConnectedAt = $state<Date | null>(null);

	// Derived state for easy UI checks
	hasError = $derived(this.latestError !== null);
	canReconnect = $derived(
		this.connectionStatus === 'Disconnected' || this.connectionStatus === 'Failed'
	);

	// Configuration options
	#options: SignalRConnectionOptions;
	#maxErrorsStored = 8;
	#handlers = $state(new Set<string>());

	constructor(options: SignalRConnectionOptions) {
		this.#options = {
			reconnectIntervals: [0, 2000, 10000, 30000],
			autoConnect: false,
			useWebSocketsOnly: true,
			skipNegotiation: false,
			...options
		};

		if (browser && this.#options.autoConnect) {
			this.connect();
		}
	}

	/**
	 * Connect to the SignalR hub
	 */
	async connect(): Promise<void> {
		if (!browser) return;

		if (this.connection && this.connection.state !== HubConnectionState.Disconnected) {
			return;
		}

		try {
			this.connectionStatus = 'Connecting';
			this.connectionAttempts++;

			const builder = new HubConnectionBuilder().withUrl(this.#options.hubUrl, {
				skipNegotiation: this.#options.skipNegotiation,
				transport: this.#options.useWebSocketsOnly ? HttpTransportType.WebSockets : undefined,
				accessTokenFactory: this.#options.authTokenCallback
			});

			// Add logging if enabled
			if (enableLogging) {
				builder.configureLogging(LogLevel.Information);
			} else {
				builder.configureLogging(LogLevel.None);
			}

			// Add reconnect logic
			if (this.#options.reconnectIntervals && this.#options.reconnectIntervals.length > 0) {
				builder.withAutomaticReconnect(this.#options.reconnectIntervals);
			}

			// Build the connection
			this.connection = builder.build();
			// Set up connection lifecycle event handlers
			this.#setupConnectionEvents();

			// Start the connection
			await this.connection.start();

			this.connectionStatus = 'Connected';
			this.lastConnectedAt = new Date();

			// Clear reconnection attempts on successful connection
			this.reconnectionAttempts = 0;
		} catch (err) {
			this.connectionStatus = 'Failed';
			this.#addError('CONNECTION_ERROR', 'Failed to establish connection', err);
			throw err;
		}
	}

	/**
	 * Disconnect from the SignalR hub
	 */
	async disconnect(): Promise<void> {
		if (!this.connection) return;

		try {
			await this.connection.stop();
			this.connectionStatus = 'Disconnected';
		} catch (err) {
			this.#addError('DISCONNECTION_ERROR', 'Error disconnecting from SignalR hub', err);
			// No need to rethrow as disconnection errors are usually non-critical
		}
	}

	/**
	 * Send a message to a specific method on the SignalR hub
	 * @template TArgs - Array of argument types for the method
	 * @param methodName - The name of the server method to call
	 * @param args - The arguments to pass to the method
	 */
	async send<TArgs extends unknown[]>(methodName: string, ...args: TArgs): Promise<void> {
		if (!this.connection || this.connection.state !== HubConnectionState.Connected) {
			const error = new Error('Cannot send message: No active connection');
			this.#addError(
				'INVOCATION_ERROR',
				`Cannot invoke method '${methodName}': No active connection`,
				{ methodName, args }
			);
			throw error;
		}

		try {
			await this.connection.invoke(methodName, ...args);
		} catch (err) {
			this.#addError('INVOCATION_ERROR', `Error invoking method '${methodName}'`, err);
			throw err;
		}
	}

	/**
	 * Listen for messages from a specific method on the SignalR hub
	 * @template TArgs - Array of argument types from the hub method
	 * @param methodName - The name of the client method to register
	 * @param callback - The callback to execute when the method is invoked
	 */
	on<TArgs extends unknown[] = unknown[]>(
		methodName: string,
		callback: (...args: TArgs) => void
	): void {
		if (!this.connection) {
			const error = new Error('Cannot register handlers: No connection created');
			this.#addError(
				'HANDLER_ERROR',
				`Cannot register handler for '${methodName}': No connection created`,
				{ methodName }
			);
			throw error;
		}

		try {
			this.connection.on(methodName, callback);
			this.#handlers.add(methodName);
		} catch (err) {
			this.#addError('HANDLER_ERROR', `Error registering handler for method '${methodName}'`, err);
			throw err;
		}
	}

	/**
	 * Remove a handler for a specific method
	 * @param methodName - The name of the client method to unregister
	 * @param callback - Optional specific callback to remove
	 */
	off<TArgs extends unknown[] = unknown[]>(
		methodName: string,
		callback?: (...args: TArgs) => void
	): void {
		if (!this.connection) return;

		try {
			if (callback) {
				this.connection.off(methodName, callback);
			} else {
				this.connection.off(methodName);
			}

			// Update the handlers tracking
			if (!callback) {
				this.#handlers.delete(methodName);
			}
		} catch (err) {
			this.#addError('HANDLER_ERROR', `Error removing handler for method '${methodName}'`, err);
		}
	}

	/**
	 * Attempts to reconnect if disconnected
	 */
	async reconnect(): Promise<void> {
		if (
			this.connectionStatus === 'Connected' ||
			this.connectionStatus === 'Connecting' ||
			this.connectionStatus === 'Reconnecting'
		) {
			return;
		}

		this.reconnectionAttempts++;
		try {
			await this.connect();
		} catch (err) {
			this.#addError(
				'RECONNECTION_ERROR',
				`Manual reconnection attempt ${this.reconnectionAttempts} failed`,
				err
			);
		}
	}

	/**
	 * Clear error history
	 */
	clearErrors(): void {
		this.errors = [];
		this.latestError = null;
	}

	/**
	 * Check if the connection is active
	 */
	get isConnected(): boolean {
		return this.connectionStatus === 'Connected';
	}

	/**
	 * Get a list of registered handlers
	 */
	get registeredHandlers(): string[] {
		return [...this.#handlers];
	}

	// Private methods
	#setupConnectionEvents(): void {
		if (!this.connection) return;

		this.connection.onclose((error) => {
			this.connectionStatus = 'Disconnected';
			if (error) {
				this.#addError('CONNECTION_ERROR', 'Connection closed with error', error);
			}
		});

		this.connection.onreconnecting((error) => {
			this.connectionStatus = 'Reconnecting';
			this.reconnectionAttempts++;
			if (error) {
				this.#addError('RECONNECTION_ERROR', 'Attempting to reconnect', error);
			}
		});

		this.connection.onreconnected((connectionId) => {
			this.connectionStatus = 'Connected';
			this.lastConnectedAt = new Date();
		});
	}

	#addError(type: SignalRErrorType, message: string, details?: unknown): void {
		const error: SignalRError = {
			type,
			message,
			timestamp: new Date(),
			details
		};

		// Update latest error
		this.latestError = error;

		// Add to errors array with limit
		this.errors = [error, ...this.errors].slice(0, this.#maxErrorsStored);

		// Call error handler if provided
		if (this.#options.onError) {
			try {
				this.#options.onError(error);
			} catch {
				// Prevent errors in the error handler from causing issues
				// We intentionally suppress these errors to avoid infinite loops
			}
		}
	}
}
