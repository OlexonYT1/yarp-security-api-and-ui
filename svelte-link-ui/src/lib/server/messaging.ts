import { RABBIT_MQ_CONNECTION } from '$env/static/private';
import { Connection, type Consumer } from 'rabbitmq-client';
import { Buffer } from 'buffer';

export class RabbitMessageBus {
	private _connection: Connection | null = null;
	private _consumers: Consumer[] = [];

	constructor(connection: Connection | null = null) {
		if (connection) {
			this._connection = connection;
		} else {
			this._connection = this.rabbitMQConnection();
		}
	}

	get connection(): Connection | null {
		return this._connection;
	}

	get consumers(): Consumer[] {
		return this._consumers;
	}

	public initConnection(): void {
		if (!this._connection) {
			this._connection = this.rabbitMQConnection();
		}
	}

	private rabbitMQConnection(): Connection {
		const con = new Connection(RABBIT_MQ_CONNECTION);

		con.on('error', (err) => {
			console.error('RabbitMQ connection error', err);
		});
		con.on('connection', () => {
			console.log('Connection successfully (re)established');
		});

		return con;
	}

	public registerConsumer<T>(
		queueName: string,
		sourceName: string,
		handler: (payload: T) => Promise<void>
	) {
		if (!this._connection) {
			throw new Error('Connection not initialized');
		}
		const sub = this._connection.createConsumer(
			{
				queue: queueName,
				queueOptions: { durable: true },
				qos: { prefetchCount: 2 },
				exchanges: [{ exchange: queueName, type: 'fanout' }],
				exchangeBindings: [
					{
						source: sourceName,
						destination: queueName,
						routingKey: '*'
					}
				],
				queueBindings: [{ exchange: queueName, routingKey: '*' }],
				consumerTag: queueName
			},
			async (msg) => {
				try {
					const message: MasstransitMessage = msg as MasstransitMessage;
					const payload = this.parseMessageBody<T>(message);
					await handler(payload);
				} catch (error) {
					console.error('Error processing message', error);
				}
			}
		);

		sub.on('error', (err) => {
			console.error('Consumer error (user-events)', err);
		});

		this._consumers.push(sub);
	}

	private parseMessageBody<T>(msg: MasstransitMessage): T {
		const bodyString = msg.body.toString('utf-8');
		const envelope: MasstransitEnvelope<T> = JSON.parse(bodyString);
		return envelope.message;
	}
}

type MasstransitMessage = {
	consumerTag: string;
	deliveryTag: number;
	redelivered: boolean;
	exchange: string;
	routingKey: string;
	contentType: string;
	headers: {
		'MT-Activity-Id': string;
		TenantId: string;
		UserId: string;
		publishId: string;
	};
	deliveryMode: number;
	priority: number;
	messageId: string;
	durable: boolean;
	body: Buffer;
};

type MasstransitEnvelope<T> = {
	messageId: string;
	requestId: string | null;
	correlationId: string | null;
	conversationId: string;
	initiatorId: string | null;
	sourceAddress: string;
	destinationAddress: string;
	responseAddress: string | null;
	faultAddress: string | null;
	messageType: string[];
	message: T;
	expirationTime: string | null;
	sentTime: string;
	headers: {
		TenantId: string;
		UserId: string;
		'MT-Activity-Id': string;
	};
	host: {
		machineName: string;
		processName: string;
		processId: number;
		assembly: string;
		assemblyVersion: string;
		frameworkVersion: string;
		massTransitVersion: string;
		operatingSystemVersion: string;
	};
};
