<script lang="ts">
	let { children } = $props();
</script>

<svelte:head>
	<title>Ubik link</title>
</svelte:head>

<div class="flex flex-col gap-1 p-2">
	{@render children?.()}
</div>
