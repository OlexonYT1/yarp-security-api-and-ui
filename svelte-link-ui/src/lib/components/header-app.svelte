<script lang="ts">
	import Logo from '$lib/assets/img/logo2.svg';
	import ToggleTheme from '$lib/components/toggle-theme.svelte';
	import { SidebarTrigger } from '$lib/components/ui/sidebar/index.js';
</script>

<header class="sticky top-0 z-50 flex w-full items-center border-b bg-background">
	<div class="flex w-full items-center justify-between px-4">
		<div class="flex items-center">
			<SidebarTrigger />
		</div>
		<a href="/app" title="Home">
			<div class="flex flex-row items-center gap-1">
				<img src={Logo} alt="Logo" class="h-10 w-10" />
				<div class="scroll-m-20 text-2xl font-extrabold tracking-tight">TestApp</div>
			</div>
		</a>
		<ToggleTheme />
	</div>
</header>
