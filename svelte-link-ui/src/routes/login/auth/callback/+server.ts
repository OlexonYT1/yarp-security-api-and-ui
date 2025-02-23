import { generateSessionToken, createSession, setSessionTokenCookie } from '$lib/server/session';
import { keycloak } from '$lib/server/oauth';
import { getUser } from '$lib/server/user';
import { createAuthTokenInCache } from '$lib/server/backend-token';
import { decodeIdToken } from 'arctic';
import type { OAuth2Tokens } from 'arctic';
import { getUserFromCache } from '$lib/server/user';
import type { RequestEvent } from '@sveltejs/kit';
import { ObjectParser } from '@pilcrowjs/object-parser';

export async function GET(event: RequestEvent): Promise<Response> {
	const code = event.url.searchParams.get('code');
	const state = event.url.searchParams.get('state');
	const storedState = event.cookies.get('keycloak_oauth_state') ?? null;
	const codeVerifier = event.cookies.get('keycloak_code_verifier') ?? null;

	if (code === null || state === null || storedState === null || codeVerifier === null) {
		return new Response(null, {
			status: 400
		});
	}
	if (state !== storedState) {
		return new Response(null, {
			status: 400
		});
	}

	let tokens: OAuth2Tokens;
	try {
		tokens = await keycloak.validateAuthorizationCode(code, codeVerifier);
	} catch (e) {
		// Invalid code or client credentials
		return new Response(null, {
			status: 400
		});
	}
	const claims = decodeIdToken(tokens.idToken());
	const claimsParser = new ObjectParser(claims);
	const keycloakUserId = claimsParser.getString('sub');

	// Try to get the user from cache or the backend
	const existingUser = await getUser(keycloakUserId, tokens.accessToken());

	if (existingUser !== null) {
		//Session and cookie
		const sessionToken = generateSessionToken();
		const session = await createSession(sessionToken, existingUser.authId);
		await setSessionTokenCookie(event, sessionToken, session.expiresAt);

		//Store token in cache
		await createAuthTokenInCache(
			existingUser.authId,
			tokens.idToken(),
			tokens.accessToken(),
			tokens.refreshToken(),
			tokens.accessTokenExpiresAt(),
			session.expiresAt
		);

		return new Response(null, {
			status: 302,
			headers: {
				Location: '/app'
			}
		});
	} else {
		return new Response(null, {
			status: 401
		});
	}

	// TODO: Use that for onboarding (maybe)
	// const user = await createUser(keycloakUserId, username);

	// const sessionToken = generateSessionToken();
	// const session = await createSession(sessionToken, user.id);
	// await setSessionTokenCookie(sessionToken, session.expiresAt);
	// return new Response(null, {
	// 	status: 302,
	// 	headers: {
	// 		Location: "/"
	// 	}
	// });
}
