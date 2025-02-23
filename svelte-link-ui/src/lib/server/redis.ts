import { Redis } from 'ioredis';
import { REDIS_HOST, REDIS_PORT } from '$env/static/private';

const redis = new Redis({
	port: Number(REDIS_PORT),
	host: REDIS_HOST
});

export { redis };
