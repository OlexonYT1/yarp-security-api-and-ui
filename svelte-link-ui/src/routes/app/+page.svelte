<script lang="ts">
	import { enhance } from '$app/forms';
	import type {
		UserMeResult,
		AuthorizationLightResult,
		RoleLightResult
	} from '$lib/shared-types/user-types';

	import type { PageProps } from './$types';
	import Button from '$lib/components/ui/button/button.svelte';

	const { data }: PageProps = $props();
	const user = data.user as UserMeResult;
</script>

<div class="flex justify-between">
	<h1 class="bg-secondary text-3xl font-bold underline">Hi, {user.firstname} {user.lastname}!</h1>
</div>
<p>You are connected, and here, you can look at your security info</p>
<ul>
	<li>ID: {user.id}</li>
	<li>Auth ID: {user.authId}</li>
	<li>First Name: {user.firstname}</li>
	<li>Last Name: {user.lastname}</li>
	<li>Email: {user.email}</li>
	<li>
		Is Activated In Selected Subscription: {user.isActivatedInSelectedSubscription ? 'Yes' : 'No'}
	</li>
	<li>Is Mega Admin: {user.isMegaAdmin ? 'Yes' : 'No'}</li>
	<li>Owner Of Subscriptions IDs: {user.ownerOfSubscriptionsIds.join(', ')}</li>
	<li>Selected Tenant ID: {user.selectedTenantId}</li>
	<li>Is Sub Owner Of The Selected Tenant: {user.isSubOwnerOfTheSelectedTenant ? 'Yes' : 'No'}</li>
	<li>
		Selected Tenant Authorizations: {user.selectedTenantAuthorizations
			.map((auth: AuthorizationLightResult) => auth.code)
			.join(', ')}
	</li>
	<li>
		Selected Tenant Roles: {user.selectedTenantRoles
			.map((role: RoleLightResult) => role.code)
			.join(', ')}
	</li>
	<li>Version: {user.version}</li>
</ul>

<form method="post" use:enhance>
	<Button type="submit">Sign out</Button>
</form>
