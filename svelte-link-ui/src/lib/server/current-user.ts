import { invalidateSession, deleteSessionTokenCookie } from './session';
import { getAuthToken, removeAuthTokenFromCache } from './backend-token';
import { logout } from './oauth';
import { removeUserFromCache, getUserFromCache } from './user';
import type { RequestEvent } from '@sveltejs/kit';

export async function fullLogout(event: RequestEvent): Promise<void> {
	const session = event.locals.session;
	await invalidateSession(session.id, session.userId);
	deleteSessionTokenCookie(event);
	//Logout from keycloak and clean tokens
	const tokens = await getAuthToken(session.userId, session.expiresAt);
	if (tokens !== null) {
		logout(tokens?.refreshToken);
		await removeAuthTokenFromCache(session.userId);
	}

	//Clean user
	const user = await getUserFromCache(session.userId);
	if (user !== null) {
		await removeUserFromCache(user.authId, user.selectedTenantId);
	}
}

export function checkIsLogged(event: RequestEvent): boolean {
	if (event.locals.session === null || event.locals.user === null) {
		return false;
	}
	return true;
}
