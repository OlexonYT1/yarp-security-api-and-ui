<script lang="ts">
	import Chat from '$lib/components/communication/chat.svelte';
	import { getHubToken, type hubTokenType } from '$lib/components/communication/utils';

	const { data } = $props();
	const chatUrl = data.chatUrl as string;
	const userName = data.userName as string;
	const tokenType: hubTokenType = 'chat';
</script>

<Chat authTokenCallback={() => getHubToken(tokenType)} {chatUrl} {userName}>
	{#snippet messageItem(text)}
		<div class="message-container mb-3 rounded-md bg-muted p-2">
			<p class="font-medium">{text}</p>
			<span class="text-xs text-muted-foreground">{new Date().toLocaleTimeString()}</span>
		</div>
	{/snippet}
</Chat>
