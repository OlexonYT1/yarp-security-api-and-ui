import { generateSessionToken, createSession, setSessionTokenCookie } from '$lib/server/session';
import { keycloak } from '$lib/server/oauth';
import {
	getUser,
	type RegisterUserCommand,
	registerUser,
	type UserRegisterResult
} from '$lib/server/user';
import { createAuthTokenInCache } from '$lib/server/backend-token';
import { decodeIdToken } from 'arctic';
import type { OAuth2Tokens } from 'arctic';
import type { RequestEvent } from '@sveltejs/kit';
import { ObjectParser } from '@pilcrowjs/object-parser';
import { REGISTER_USER_AUTHORIZATION_KEY } from '$env/static/private';

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

	if (existingUser === null) {
		// Try to unboard the user and
		const user: RegisterUserCommand = {
			authId: keycloakUserId,
			email: claimsParser.getString('email'),
			firstname: claimsParser.getString('given_name'),
			lastname: claimsParser.getString('family_name'),
			authorizationKey: REGISTER_USER_AUTHORIZATION_KEY
		};

		const registerUserResult = await registerUser(user, tokens.accessToken());

		if (registerUserResult === null) {
			return new Response(null, {
				status: 401
			});
		}
		const newUser = await getUser(keycloakUserId, tokens.accessToken());

		if (newUser == null) {
			return new Response(null, {
				status: 401
			});
		}

		return new Response(null, {
			status: 302,
			headers: {
				Location: '/register'
			}
		});
	}

	//Standard redirect case
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
}
