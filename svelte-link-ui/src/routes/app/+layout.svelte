<script lang="ts">
	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import AppSidebar from '$lib/components/sidebar-app.svelte';
	import HeaderApp from '$lib/components/header-app.svelte';
	let { children } = $props();
</script>

<div class="[--header-height:calc(theme(spacing.10))]">
	<Sidebar.Provider class="flex flex-col">
		<HeaderApp />
		<div class="flex flex-1">
			<AppSidebar />
			<Sidebar.Inset>
				<main class="top-(--header-height) !h-[calc(100svh-var(--header-height))] p-2">
					{@render children?.()}
				</main>
			</Sidebar.Inset>
		</div>
	</Sidebar.Provider>
</div>
