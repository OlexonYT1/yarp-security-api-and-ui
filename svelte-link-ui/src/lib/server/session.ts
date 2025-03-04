import { redis } from './redis';
import { encodeBase32LowerCaseNoPadding, encodeHexLowerCase } from '@oslojs/encoding';
import { sha256 } from '@oslojs/crypto/sha2';
import type { RequestEvent } from '@sveltejs/kit';

export function generateSessionToken(): string {
	const bytes = new Uint8Array(20);
	crypto.getRandomValues(bytes);
	const token = encodeBase32LowerCaseNoPadding(bytes);
	return token;
}

export async function createSession(token: string, userId: string): Promise<Session> {
	const sessionId = encodeHexLowerCase(sha256(new TextEncoder().encode(token)));
	const session: Session = {
		id: sessionId,
		userId,
		expiresAt: new Date(Date.now() + 1000 * 60 * 60 * 24 * 30)
	};
	await redis.set(
		`session:${session.id}`,
		JSON.stringify({
			id: session.id,
			user_id: session.userId,
			expires_at: Math.floor(session.expiresAt.getTime() / 1000)
		}),
		'EX',
		Math.floor((session.expiresAt.getTime() - Date.now()) / 1000)
	);
	await redis.sadd(`user_sessions:${userId}`, sessionId);

	return session;
}

export async function validateSessionToken(token: string): Promise<SessionValidationResult> {
	const sessionId = encodeHexLowerCase(sha256(new TextEncoder().encode(token)));
	const item = await redis.get(`session:${sessionId}`);
	if (item === null) {
		return null;
	}

	const result = JSON.parse(item);
	const session: Session = {
		id: result.id,
		userId: result.user_id,
		expiresAt: new Date(result.expires_at * 1000)
	};

	if (Date.now() >= session.expiresAt.getTime()) {
		await redis.del(`session:${sessionId}`);
		await redis.srem(`user_sessions:${session.userId}`, sessionId);
		return null;
	}
	if (Date.now() >= session.expiresAt.getTime() - 1000 * 60 * 60 * 24 * 15) {
		session.expiresAt = new Date(Date.now() + 1000 * 60 * 60 * 24 * 30);
		await redis.set(
			`session:${session.id}`,
			JSON.stringify({
				id: session.id,
				user_id: session.userId,
				expires_at: Math.floor(session.expiresAt.getTime() / 1000)
			}),
			'EX',
			Math.floor(session.expiresAt.getTime() / 1000)
		);
	}
	return session;
}

export async function invalidateSession(sessionId: string, userId: string): Promise<void> {
	await redis.del(`session:${sessionId}`);
	await redis.srem(`user_sessions:${userId}`, sessionId);
}

export async function invalidateAllSessions(userId: string): Promise<void> {
	const sessionIds = await redis.smembers(`user_sessions:${userId}`);
	if (sessionIds.length < 1) {
		return;
	}

	const pipeline = redis.pipeline();

	for (const sessionId of sessionIds) {
		pipeline.unlink(`session:${sessionId}`);
	}
	pipeline.unlink(`user_sessions:${userId}`);

	await pipeline.exec();
}

export async function setSessionTokenCookie(
	event: RequestEvent,
	token: string,
	expiresAt: Date
): Promise<void> {
	event.cookies.set('session', token, {
		httpOnly: true,
		path: '/',
		secure: import.meta.env.PROD,
		sameSite: 'lax',
		expires: expiresAt
	});
}

export function deleteSessionTokenCookie(event: RequestEvent): void {
	event.cookies.set('session', '', {
		httpOnly: true,
		path: '/',
		secure: import.meta.env.PROD,
		sameSite: 'lax',
		maxAge: 0
	});
}

export type Session = {
	id: string;
	userId: string;
	expiresAt: Date;
};

type SessionValidationResult = Session | null;
