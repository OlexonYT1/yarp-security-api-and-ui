import { TokenBucket } from '$lib/server/rate-limit';
import {
	validateSessionToken,
	setSessionTokenCookie,
	deleteSessionTokenCookie,
	invalidateSession
} from '$lib/server/session';
import { getAuthToken, removeAuthTokenFromCache } from '$lib/server/backend-token';
import { getUser } from '$lib/server/user';
import { RabbitMessageBus } from '$lib/server/messaging';
import { sequence } from '@sveltejs/kit/hooks';
import { type Handle, type ServerInit } from '@sveltejs/kit';
import { Guid } from 'guid-ts';
import {
	removeAllUsersFromCacheByTenantId,
	removeAllUsersFromCache,
	removeUserFromCache
} from './lib/server/user';

const bucket = new TokenBucket<string>(100, 1);
const LOGIN_URL = '/login';

export const init: ServerInit = async () => {
	// Register all the RabbitMQ stuff (for cache cleaning)
	const rabbit = new RabbitMessageBus();
	rabbit.initConnection();

	// Add consumers
	addCleanCacheConsumerForTenantUpdated(rabbit);
	addCleanCacheConsumerForRoleDeleted(rabbit);
	addCleanCacheConsumerForRoleUpdated(rabbit);
	addCleanCacheConsumerForUserRequestSent(rabbit);
	addCleanCacheConsumerForTenantDeleted(rabbit);

	// Parse and dispose RabbitMQ resources on shutdown
	onShutdownRabbit(rabbit);
};

const rateLimitHandle: Handle = async ({ event, resolve }) => {
	// Note: Assumes X-Forwarded-For will always be defined.
	const clientIP = event.request.headers.get('X-Forwarded-For');
	if (clientIP === null) {
		return resolve(event);
	}
	const cost = event.request.method === 'GET' || event.request.method === 'OPTIONS' ? 1 : 3;
	if (!bucket.consume(clientIP, cost)) {
		return new Response('Too many requests', {
			status: 429
		});
	}
	return resolve(event);
};

const authHandle: Handle = async ({ event, resolve }) => {
	if (event.url.pathname.startsWith('/app')) {
		const token = event.cookies.get('session');
		if (!token) {
			return redirectToLogin(event);
		}
		const session = await validateSessionToken(token);
		if (!session) {
			deleteSessionTokenCookie(event);
			return redirectToLogin(event);
		}

		const backendToken = await getAuthToken(session.userId, session.expiresAt);
		const user = backendToken ? await getUser(session.userId, backendToken.accessToken) : null;

		if (user) {
			await setSessionTokenCookie(event, token, session.expiresAt);
			event.locals.user = user;
			event.locals.session = session;
		} else {
			await invalidateSession(session.id, session.userId);
			await removeAuthTokenFromCache(session.userId);
			deleteSessionTokenCookie(event);
			return redirectToLogin(event);
		}
	}

	return resolve(event);
};

export const handle = sequence(rateLimitHandle, authHandle);

//Consumers
function addCleanCacheConsumerForUserRequestSent(messageBus: RabbitMessageBus) {
	messageBus.registerConsumer<CleanCacheForUserRequestSent>(
		'svelte-ui-clean-cache-user-request-sent',
		'UbikLink.Security.Contracts.Users.Events:CleanCacheForUserRequestSent',
		async (payload) => {
			await removeUserFromCache(payload.authId.toString(), payload.tenantId?.toString() ?? null);
		}
	);
}

function addCleanCacheConsumerForTenantDeleted(messageBus: RabbitMessageBus) {
	messageBus.registerConsumer<CleanCacheTenantDeleted>(
		'svelte-ui-clean-cache-tenant-deleted',
		'UbikLink.Security.Contracts.Tenants.Events:CleanCacheTenantDeleted',
		async (payload) => {
			await removeAllUsersFromCacheByTenantId(payload.tenantId.toString());
		}
	);
}

function addCleanCacheConsumerForTenantUpdated(messageBus: RabbitMessageBus) {
	messageBus.registerConsumer<CleanCacheTenantUpdated>(
		'svelte-ui-clean-cache-tenant-updated',
		'UbikLink.Security.Contracts.Tenants.Events:CleanCacheTenantUpdated',
		async (payload) => {
			await removeAllUsersFromCacheByTenantId(payload.tenantId.toString());
		}
	);
}

function addCleanCacheConsumerForRoleDeleted(messageBus: RabbitMessageBus) {
	messageBus.registerConsumer<CleanCacheRoleDeleted>(
		'svelte-ui-clean-cache-role-deleted',
		'UbikLink.Security.Contracts.Roles.Events:CleanCacheRoleDeleted',
		async (payload) => {
			await removeAllUsersFromCache();
		}
	);
}

function addCleanCacheConsumerForRoleUpdated(messageBus: RabbitMessageBus) {
	messageBus.registerConsumer<CleanCacheRoleUpdated>(
		'svelte-ui-clean-cache-role-updated',
		'UbikLink.Security.Contracts.Roles.Events:CleanCacheRoleUpdated',
		async (payload) => {
			await removeAllUsersFromCache();
		}
	);
}

function redirectToLogin(event: any): Response {
	event.locals.session = null;
	event.locals.user = null;
	return new Response(null, {
		status: 302,
		headers: { location: LOGIN_URL }
	});
}

function onShutdownRabbit(rabbit: RabbitMessageBus) {
	process.on('SIGINT', async () => {
		await shutdownRabbit(rabbit);
		process.exit(0);
	});

	process.on('SIGTERM', async () => {
		await shutdownRabbit(rabbit);
		process.exit(0);
	});
}

async function shutdownRabbit(rabbit: RabbitMessageBus) {
	for (const consumer of rabbit.consumers) {
		await consumer.close();
	}
	await rabbit.connection?.close();
	console.log('RabbitMQ consumers stopped and connection closed');
}

interface CleanCacheForUserRequestSent {
	userId: Guid;
	tenantId?: Guid | null;
	authId: string;
}

interface CleanCacheTenantUpdated {
	tenantId: Guid;
}

interface CleanCacheTenantDeleted {
	tenantId: Guid;
}

interface CleanCacheRoleDeleted {
	roleId: Guid;
	tenantId?: Guid | null;
}

//For the moment, we only manage system roles so we clean the complete cache
//But if one day we will have tenant specific roles, we will need to clean only the cache for the tenant
interface CleanCacheRoleUpdated {
	roleId: Guid;
	tenantId?: Guid | null;
}
