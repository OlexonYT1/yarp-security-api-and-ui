<script lang="ts">
	import { onMount, onDestroy, type Snippet } from 'svelte';
	import { Button } from '$lib/components/ui/button';
	import {
		SignalRConnection,
		type SignalRConnectionOptions
	} from '$lib/components/communication/signalr.svelte';

	let { authTokenCallback, chatUrl, userName, messageItem } = $props<{
		authTokenCallback: () => Promise<string>;
		chatUrl: string;
		userName: string;
		messageItem?: Snippet<[text: string]>;
	}>();

	const options: SignalRConnectionOptions = {
		hubUrl: chatUrl,
		authTokenCallback,
		autoConnect: false,
		skipNegotiation: true,
		useWebSocketsOnly: true,
		onError: (error) => {
			console.log('SignalR error:', error);
		}
	};

	const chat = new SignalRConnection(options);

	// State
	let messageInput = $state('');
	let messages = $state.raw<string[]>([]);
	let messagesContainer: HTMLDivElement;
	let connectionColorClass = $derived(
		chat.connectionStatus === 'Connected'
			? 'bg-green-500'
			: chat.connectionStatus === 'Connecting' || chat.connectionStatus === 'Reconnecting'
				? 'bg-yellow-500'
				: 'bg-red-500'
	);

	// Compo events
	onMount(async () => {
		await chat.connect();
		chat.on('ReceiveMessage', (user: string, message: string) => {
			addMessage(user, message);
			scrollToBottom();
		});
	});

	onDestroy(() => {
		if (chat) {
			chat.disconnect();
		}
	});

	//Functions
	function addMessage(user: string, message: string) {
		// Update the raw array by reassignment
		const newMessages = [...messages, `${user}: ${message}`];
		messages = newMessages;
	}

	async function sendMessage() {
		if (!messageInput.trim() || !chat.isConnected || !chat) return;

		await chat.send<[string, string]>('SendMessage', userName, messageInput);
		messageInput = '';
	}

	function scrollToBottom() {
		if (messagesContainer) {
			setTimeout(() => {
				messagesContainer.scrollTop = messagesContainer.scrollHeight;
			}, 0);
		}
	}
</script>

<div class="mx-auto max-w-2xl p-4">
	<h1 class="mb-4 text-2xl font-bold">Chat</h1>

	<!-- Connection status -->
	<div class="mb-4 flex items-center gap-2">
		<div class={`h-3 w-3 rounded-full ${connectionColorClass}`}></div>
		<span>{chat.connectionStatus}</span>
	</div>

	<!-- Messages display -->
	<div
		bind:this={messagesContainer}
		class="mb-4 h-80 overflow-y-auto rounded-md border bg-card p-4"
	>
		{#if messages.length === 0}
			<p class="text-muted-foreground italic">No messages yet</p>
		{:else}
			{#each messages as message}
				{#if messageItem}
					{@render messageItem(message)}
				{:else}
					<div class="mb-2 border-b border-muted pb-2">
						<p>{message}</p>
					</div>
				{/if}
			{/each}
		{/if}
	</div>

	<!-- Message input -->
	<div class="flex gap-2">
		<form
			onsubmit={(e) => {
				e.preventDefault();
				sendMessage();
			}}
			class="flex w-full gap-2"
		>
			<input
				id="messageInput"
				type="text"
				bind:value={messageInput}
				placeholder="Type your message..."
				class="flex-1 rounded-md border p-2"
				disabled={!chat.isConnected}
			/>
			<Button type="submit" disabled={!chat.isConnected}>Send</Button>
		</form>
	</div>
</div>
