// See https://svelte.dev/docs/kit/types#app.d.ts

import type { UserMeResult } from '$lib/server/user';

// for information about these interfaces
declare global {
	namespace App {
		// interface Error {}
		// interface Locals {}
		// interface PageData {}
		// interface PageState {}
		// interface Platform {}
	}
}

declare global {
	namespace App {
		// interface Error {}
		interface Locals {
			session: Session | null;
			user: UserMeResult | null;
		}
		// interface PageData {}
		// interface PageState {}
		// interface Platform {}
	}
}

export {};
