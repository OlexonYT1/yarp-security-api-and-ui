import type { Actions } from './$types';
import type { RequestEvent } from '@sveltejs/kit';
import { getAuthToken } from '$lib/server/backend-token';
import { onboardMe } from '$lib/server/user';
import type { UserMeResult } from '$lib/shared-types/user-types';
import type { Session } from '$lib/server/session';
import { redirect } from '@sveltejs/kit';
import { fullLogout } from '$lib/server/current-user';

export async function load(event: RequestEvent) {
	const url = new URL(event.request.url);
	const activationCode = url.searchParams.get('a');
	const currentUser = event.locals.user as UserMeResult;
	const session = event.locals.session as Session;
	let isOnboarded = false;

	if (activationCode) {
		const userToken = await getAuthToken(currentUser.authId, session.expiresAt);
		if (userToken !== null) {
			isOnboarded = await onboardMe(activationCode, userToken?.accessToken);
		}
	}

	return {
		user: currentUser,
		isOnboarded: isOnboarded
	};
}

export const actions: Actions = {
	default: action
};

async function action(event: RequestEvent) {
	await fullLogout(event);

	return redirect(302, '/login/auth');
}
