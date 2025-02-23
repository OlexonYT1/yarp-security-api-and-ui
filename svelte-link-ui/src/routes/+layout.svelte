<script lang="ts">
	import { ModeWatcher } from 'mode-watcher';
	import '../app.css';
	let { children } = $props();
</script>

<svelte:head>
	<title>Example secu</title>
</svelte:head>

<ModeWatcher />
<div class="flex flex-col gap-2 p-4">
	{@render children?.()}
</div>
