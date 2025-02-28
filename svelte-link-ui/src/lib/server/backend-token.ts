import { redis } from './redis';
import { keycloak } from './oauth';
import CryptoJS from 'crypto-js';
import { TOKEN_STORE_SECRET } from '$env/static/private';

export type AuthToken = {
	key: string;
	idToken: string;
	accessToken: string;
	refreshToken: string;
	expiresUtc: Date;
	expiresRefreshUtc: Date;
};

export async function createAuthTokenInCache(
	key: string,
	idToken: string,
	accessToken: string,
	refreshToken: string,
	expiresUtc: Date,
	expiresRefreshUtc: Date
): Promise<AuthToken> {
	const tokens: AuthToken = {
		key,
		idToken,
		accessToken,
		refreshToken,
		expiresUtc,
		expiresRefreshUtc
	};

	const keyForCache = TOKEN_STORE_SECRET;

	if (!keyForCache)
		throw new Error('TOKEN_STORE_SECRET environment variable is not set or is empty');

	await redis.set(
		`jsapp-token:${key}`,
		CryptoJS.AES.encrypt(JSON.stringify(tokens), keyForCache).toString(),
		'EX',
		Math.floor((tokens.expiresRefreshUtc.getTime() - Date.now()) / 1000)
	);

	return tokens;
}

export async function removeAuthTokenFromCache(key: string): Promise<void> {
	await redis.del(`jsapp-token:${key}`);
}

export async function getAuthToken(key: string, sessionExpiresAt: Date): Promise<AuthToken | null> {
	const cache = await redis.get(`jsapp-token:${key}`);

	if (cache === null) {
		return null;
	}

	if (!TOKEN_STORE_SECRET)
		throw new Error('TOKEN_STORE_SECRET environment variable is not set or is empty');

	const tokens = CryptoJS.AES.decrypt(cache, TOKEN_STORE_SECRET).toString(CryptoJS.enc.Utf8);

	const result = JSON.parse(tokens);

	const expiresUtcDate = new Date(result.expiresUtc);
	if (Date.now() + 60000 > expiresUtcDate.getTime()) {
		return await refreshTokenAndCache(result.refreshToken, result.key, sessionExpiresAt);
	}

	return result;
}

//Refresh with race codition protection
export async function refreshTokenAndCache(
	refreshToken: string,
	authId: string,
	sessionExpiresAt: Date
): Promise<AuthToken | null> {
	//Lock with redis
	const lockKey = `lock:jsapp-token:${authId}`;
	const lockAcquired = await redis.set(lockKey, 'locked', 'EX', 60, 'NX');
	if (lockAcquired) {
		try {
			const response = await keycloak.refreshAccessToken(refreshToken);
			const tokens = await createAuthTokenInCache(
				authId,
				response.idToken(),
				response.accessToken(),
				response.refreshToken(),
				response.accessTokenExpiresAt(),
				sessionExpiresAt
			);
			return tokens;
		} catch {
			return null;
		} finally {
			await redis.del(lockKey);
		}
	} else {
		await new Promise((resolve) => setTimeout(resolve, 100));
		return await refreshTokenAndCache(refreshToken, authId, sessionExpiresAt);
	}
}
