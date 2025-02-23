import { KeyCloak } from 'arctic';
import {
	OIDC_ISSUER,
	OIDC_CLIENT_ID,
	OIDC_CLIENT_SECRET,
	OIDC_REDIRECT_URI
} from '$env/static/private';

export const keycloak = new KeyCloak(
	OIDC_ISSUER || '',
	OIDC_CLIENT_ID || '',
	OIDC_CLIENT_SECRET || '',
	OIDC_REDIRECT_URI || ''
);

export async function logout(refreshToken: string) {
	const params = {
		client_id: OIDC_CLIENT_ID || '',
		client_secret: OIDC_CLIENT_SECRET || '',
		grant_type: 'refresh_token'
	};

	const response = await fetch(`${OIDC_ISSUER}/protocol/openid-connect/logout`, {
		method: 'POST',
		headers: {
			'Content-Type': 'application/x-www-form-urlencoded'
		},
		body: new URLSearchParams({
			refresh_token: refreshToken,
			...params
		}),
		credentials: 'include'
	});

	if (!response.ok) {
		throw new Error(`Logout failed: ${response.statusText}`);
	}

	const text = await response.text();
	if (text) {
		return JSON.parse(text);
	} else {
		return {}; // or handle the empty response case as needed
	}
}
