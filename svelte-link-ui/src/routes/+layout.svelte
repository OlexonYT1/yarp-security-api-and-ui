<script lang="ts">
	import { ModeWatcher } from 'mode-watcher';
	import '../app.css';
	let { children } = $props();
</script>

<svelte:head>
	<title>Ubik link</title>
</svelte:head>

<ModeWatcher />
{@render children?.()}
