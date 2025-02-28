<script lang="ts">
	import Button from '$lib/components/ui/button/button.svelte';
	import type { PageData } from './$types';
	import { enhance } from '$app/forms';

	export let data: PageData;
	import { onMount } from 'svelte';

	onMount(() => {
		if (data.isOnboarded || data.user.isEmailVerified) {
			setTimeout(() => {
				window.location.href = '/app';
			}, 2500);
		}
	});
</script>

{#if data.isOnboarded || data.user.isEmailVerified}
	<p>You are already verified or we are finalizing your onboarding, pls wait to be redirected...</p>
{:else}
	<p>Your are registred, we are only missing your email confirmation.</p>
	<p>
		In dev you can follow this link <a data-sveltekit-reload href="/register?a=dev-activationcode">
			<Button>Activate your account</Button>
		</a>
	</p>
	<p>In case of issue you can try to logout and login again...</p>
	<form method="post" use:enhance>
		<Button type="submit">Sign out</Button>
	</form>
{/if}
