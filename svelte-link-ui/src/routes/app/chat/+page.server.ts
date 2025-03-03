import type { RequestEvent } from './$types';
import { type UserMeResult } from '$lib/shared-types/user-types';
import { BACKEND_PROXY_URL } from '$env/static/private';
import { getAuthToken } from '$lib/server/backend-token';
import type { Session } from '$lib/server/session';
import { error } from '@sveltejs/kit';

export async function load(event: RequestEvent) {
	const user: UserMeResult = event.locals.user;
	const session: Session = event.locals.session;
	const chatUrl: string = `${BACKEND_PROXY_URL}/chathub`;

	return {
		chatUrl: chatUrl,
		userName: user.firstname + ' ' + user.lastname
	};
}
