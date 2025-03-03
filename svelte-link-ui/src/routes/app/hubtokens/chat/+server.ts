import { error } from '@sveltejs/kit';
import type { RequestHandler } from './$types';
import { BACKEND_PROXY_URL } from '$env/static/private';
import { getAuthToken } from '$lib/server/backend-token';
import type { UserMeResult } from '$lib/shared-types/user-types';
import type { Session } from '$lib/server/session';

export const GET: RequestHandler = async ({ params, locals }) => {
	const user: UserMeResult = locals.user;
	const session: Session = locals.session;
	const tokens = await getAuthToken(user.authId, session.expiresAt);

	if (!tokens) {
		error(401, {
			message: 'Cannot connect to the server'
		});
	}

	//TODO: set the type of hub token you want to retrieve
	const response = await fetch(`${BACKEND_PROXY_URL}/security/api/v1/hubtoken`, {
		headers: {
			Authorization: `Bearer ${tokens.accessToken}`
		}
	});

	if (!response.ok) {
		error(400, 'Cannot connect to the server, pls retry or refresh later.');
	}

	const token: string = await response.json();
	return new Response(token, {
		headers: { 'Content-Type': 'text/plain' }
	});
};
