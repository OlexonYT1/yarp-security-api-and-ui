//Can be used client and server side...
export type UserMeResult = {
	id: string;
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
	isEmailVerified: boolean;
	version: string;
};

export type RoleLightResult = {
	id: string;
	code: string;
};

export type AuthorizationLightResult = {
	id: string;
	code: string;
};
