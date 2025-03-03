import { redirect } from '@sveltejs/kit';
import { fullLogout } from '$lib/server/current-user';
import type { UserMeResult } from '$lib/shared-types/user-types';

import type { Actions, RequestEvent } from './$types';

export async function load(event: RequestEvent) {
	return {
		user: event.locals.user as UserMeResult
	};
}

export const actions: Actions = {
	default: action
};

async function action(event: RequestEvent) {
	await fullLogout(event);

	return redirect(302, '/login/auth');
}
