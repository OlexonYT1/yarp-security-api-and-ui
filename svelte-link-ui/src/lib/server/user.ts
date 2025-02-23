import { redis } from './redis';
import { Guid } from 'guid-ts';
import { BACKEND_PROXY_URL } from '$env/static/private';

export async function createUserInCache(user: UserMeResult, expires: Date): Promise<UserMeResult> {
	await redis.set(
		`jsapp-user:${user.authId}`,
		JSON.stringify(user),
		'EX',
		Math.floor((expires.getTime() - Date.now()) / 1000)
	);

	await redis.sadd(`jsapp-users-tenant-${user.selectedTenantId}`, `jsapp-user:${user.authId}`);
	await redis.sadd(`jsapp-users`, `jsapp-user:${user.authId}`);
	return user;
}

export async function getUser(authId: string, accessToken: string): Promise<UserMeResult | null> {
	const user = await getUserFromCache(authId);

	if (user !== null) {
		return user;
	}

	// Fetch the user from the backend and put it in the cache
	const response = await fetch(`${BACKEND_PROXY_URL}/security/api/v1/me/authinfo`, {
		headers: {
			Authorization: `Bearer ${accessToken}`
		}
	});

	if (!response.ok) {
		return null;
	}
	const userFromBackend: UserMeResult = await response.json();

	//Create user in cache for next time
	await createUserInCache(userFromBackend, new Date(Date.now() + 3600 * 1000 * 24)); // Cache for 1 day

	return userFromBackend;
}

export async function getUserFromCache(authId: string): Promise<UserMeResult | null> {
	const user = await redis.get(`jsapp-user:${authId}`);

	if (user === null) {
		return null;
	}

	const result = JSON.parse(user);
	return result;
}

export async function removeAllUsersFromCacheByTenantId(tenantId: string): Promise<void> {
	console.log('Cleaning cache for tenant', tenantId);
	// Get all user keys associated with the tenantId
	const userKeys = await redis.smembers(`jsapp-users-tenant-${tenantId}`);

	// Use a pipeline to execute multiple commands in a single round-trip
	const pipeline = redis.pipeline();

	// Delete each user key and remove it from the global user set
	for (const userKey of userKeys) {
		pipeline.del(userKey);
		pipeline.srem(`jsapp-users`, userKey);
	}

	// Remove the tenant's user set
	pipeline.del(`jsapp-users-tenant-${tenantId}`);

	// Execute the pipeline
	await pipeline.exec();
	console.log('Cache cleaned for tenant', tenantId);
}

export async function removeUserFromCache(
	userAuthId: string,
	tenantId: string | null
): Promise<void> {
	console.log('Removing user from cache', userAuthId);
	await redis.del(`jsapp-user:${userAuthId}`);
	await redis.srem(`jsapp-users`, `jsapp-user:${userAuthId}`);
	if (tenantId !== null) {
		await redis.srem(`jsapp-users-tenant-${tenantId}`, `jsapp-user:${userAuthId}`);
	}
	console.log('User removed from cache', userAuthId);
}

export async function removeAllUsersFromCache(): Promise<void> {
	console.log('Cleaning all cache entries and members');

	// Use a pipeline to execute multiple commands in a single round-trip
	const pipeline = redis.pipeline();

	// Define patterns for keys and sets to be removed
	const patterns = [
		'jsapp-users', // Global user set
		'jsapp-users-tenant-*', // Tenant user sets
		'jsapp-user:*' // User keys
	];

	// Find keys matching the patterns and add delete commands to the pipeline
	for (const pattern of patterns) {
		const keys = await redis.keys(pattern);
		for (const key of keys) {
			pipeline.del(key);
		}
	}

	// Execute the pipeline
	await pipeline.exec();
	console.log('All cache entries and members cleaned');
}

export interface UserMeResult {
	id: Guid;
	authId: string;
	firstname: string;
	lastname: string;
	email: string;
	isActivatedInSelectedSubscription: boolean;
	isMegaAdmin: boolean;
	ownerOfSubscriptionsIds: string[];
	selectedTenantId: string | null;
	isSubOwnerOfTheSelectedTenant: boolean;
	selectedTenantAuthorizations: AuthorizationLightResult[];
	selectedTenantRoles: RoleLightResult[];
	version: string;
}

export interface AuthorizationLightResult {
	id: string;
	code: string;
}

export interface RoleLightResult {
	id: string;
	code: string;
}
