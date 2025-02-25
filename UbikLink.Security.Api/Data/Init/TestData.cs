using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.EntityFrameworkCore;
using UbikLink.Common.Db;
using UbikLink.Security.Api.Data.Models;

namespace UbikLink.Security.Api.Data.Init
{
    public static class TestData
    {
        public static string AuthIdAdmin { get; } = "6c890000-5dd4-0015-be9a-08dd4d05b0b6";
        public static string AuthIdUser1 { get; } = "6c890000-5dd4-0015-c011-08dd4d05b0b6";
        public static string AuthIdUser2 { get; } = "6c890000-5dd4-0015-c012-08dd4d05b0b6";
        public static string AuthIdUserInactive { get; } = "6c890000-5dd4-0015-c013-08dd4d05b0b6";

        public static Guid AuthorizationId1 { get; } = new Guid("b8330000-3c36-7456-2656-08dd23694236");
        public static Guid AuthorizationId2 { get; } = new Guid("b8330000-3c36-7456-282b-08dd23694236");
        public static Guid AuthorizationId3 { get; } = new Guid("dca80000-5d20-0015-2a54-08dd4b4740ff");
        public static Guid AuthorizationId4 { get; } = new Guid("dca80000-5d20-0015-2a51-08dd4b4740ff");
        public static Guid AuthorizationId5 { get; } = new Guid("dca80000-5d20-0015-2a50-08dd4b4740ff");
        public static Guid AuthorizationId6 { get; } = new Guid("dca80000-5d20-0015-2a4f-08dd4b4740ff");
        public static Guid AuthorizationIdToDel { get; } = new Guid("04450000-3c36-7456-f591-08dd2cea2e4f");
        public static Guid AuthorizationIdToDel2 { get; } = new Guid("e4a40000-088f-d0ad-aa35-08dd2fe445d7");
        public static Guid AuthorizationIdToDel3 { get; } = new Guid("b4660000-088f-d0ad-bc75-08dd2fe5753f");
        public static Guid RoleId1 { get; } = new Guid("b8330000-3c36-7456-282c-08dd23694236");
        public static Guid RoleId2 { get; } = new Guid("28950000-5dd4-0015-35e1-08dd34839105");
        public static Guid RoleIdToDel1 { get; } = new Guid("28950000-5dd4-0015-35e2-08dd34839105");
        public static Guid RoleIdToDel2 { get; } = new Guid("28950000-5dd4-0015-35e3-08dd34839105");
        public static Guid RoleIdToDel3 { get; } = new Guid("28950000-5dd4-0015-35e4-08dd34839105");
        public static Guid RoleAuhtorizationId1 { get; } = new Guid("28950000-5dd4-0015-35e5-08dd34839105");
        public static Guid RoleAuhtorizationId2 { get; } = new Guid("28950000-5dd4-0015-35e6-08dd34839105");
        public static Guid RoleAuhtorizationId3 { get; } = new Guid("28950000-5dd4-0015-35e7-08dd34839105");
        public static Guid RoleAuhtorizationId4 { get; } = new Guid("28950000-5dd4-0015-35e8-08dd34839105");
        public static Guid RoleAuhtorizationId5 { get; } = new Guid("dca80000-5d20-0015-2a53-08dd4b4740ff");
        public static Guid RoleAuhtorizationId6 { get; } = new Guid("dca80000-5d20-0015-2a52-08dd4b4740ff");
        public static Guid RoleAuhtorizationId7 { get; } = new Guid("dca80100-5d20-0015-2a50-08dd4b4740ff");
        public static Guid RoleAuhtorizationId8 { get; } = new Guid("dca80000-5d20-0015-2753-08dd4b4740ff");
        public static Guid SubscriptionId1 { get; } = new Guid("60340000-3c36-7456-ead0-08dd22b9308d");
        public static Guid SubscriptionId2 { get; } = new Guid("60340000-3c36-7456-ecac-08dd22b9308d");
        public static Guid SubscriptionId3 { get; } = new Guid("28950100-5dd4-0015-35e9-08dd34839105");
        public static Guid SubscriptionUserId1 { get; } = new Guid("28950200-5dd4-0015-35e9-08dd34839105");
        public static Guid SubscriptionUserId2 { get; } = new Guid("28950200-5dd4-0015-35e7-08dd34839105");
        public static Guid SubscriptionUserId3 { get; } = new Guid("28950100-5dd4-0015-35e8-08dd34839105");
        public static Guid SubscriptionUserId4 { get; } = new Guid("28950100-5dd4-0015-35e4-08dd34839105");
        public static Guid TenantUserId1 { get; } = new Guid("28950200-5dd4-0015-35e6-08dd34839105");
        public static Guid TenantUserId2 { get; } = new Guid("28950200-5dd4-0015-35e4-08dd34839105");
        public static Guid TenantUserId3 { get; } = new Guid("28950100-5dd4-0015-35ea-08dd34839105");
        public static Guid TenantUserId4 { get; } = new Guid("28950100-5dd4-0015-35e6-08dd34839105");
        public static Guid TenantUserToDeleteId1 { get; } = new Guid("28950100-5dd4-0015-35e2-08dd34839105");
        public static Guid TenantUserToDeleteId2 { get; } = new Guid("28950100-5dd4-0015-35e1-08dd34839105");
        public static Guid TenantUserToDeleteId3 { get; } = new Guid("28950000-5dd4-0015-344d-08dd34839105");
        public static Guid TenantUserToDeleteId4 { get; } = new Guid("28950000-5dd4-0015-35dd-08dd34839105");
        public static Guid AdminUserId { get; } = new Guid("38570000-3c36-7456-e14f-08dd21df7ef2");
        public static Guid UserId1 { get; } = new Guid("40930000-3c36-7456-b7cb-08dd22ba01fe");
        public static Guid UserId2 { get; } = new Guid("40930000-3c36-7456-b921-08dd22ba01fe");
        public static Guid UserId3 { get; } = new Guid("28950100-5dd4-0015-35e5-08dd34839105");
        public static Guid UserIdInactivated { get; } = new Guid("ac6a0000-3c36-7456-a5d3-08dd28136db1");
        public static Guid TenantId1 { get; } = new Guid("38570000-3c36-7456-e31e-08dd21df7ef2");
        public static Guid TenantId2 { get; } = new Guid("38570000-3c36-7456-e321-08dd21df7ef2");
        public static Guid TenantId3 { get; } = new Guid("28950200-5dd4-0015-35e1-08dd34839105");
        public static Guid TenantId4 { get; } = new Guid("28950100-5dd4-0015-35e7-08dd34839105");
        public static Guid TenantToDeleteId1 { get; } = new Guid("28950100-5dd4-0015-35e3-08dd34839105");
        public static Guid TenantToDeleteId2 { get; } = new Guid("28950000-5dd4-0015-35df-08dd34839105");
        public static Guid TenantUserRoleId1 { get; } = new Guid("28950100-5dd4-0015-35e0-08dd34839105");

        internal static async Task LoadAsync(SecurityDbContext ctx)
        {
            //List<Guid> ids = new List<Guid>();
            //for (var i = 0; i < 100; i++)
            //{
            //    ids.Add(NewId.NextGuid());
            //}

            if (!ctx.Authorizations.Any())
            {
                var authorizations = new AuthorizationModel[]
                {
                        new()
                        {
                            Id = AuthorizationId1,
                            Code = "tenant:read",
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                            Description = "Can read its tenant",
                            Version = AuthorizationId1
                        },
                        new()
                        {
                            Id = AuthorizationId2,
                            Code = "role:write",
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                            Description = "Can create/modify role for a tenant.",
                            Version = AuthorizationId2
                        },
                         new()
                        {
                            Id = AuthorizationIdToDel,
                            Code = "tenant:test",
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                            Description = "To be deleted",
                            Version = AuthorizationIdToDel
                        },
                         new()
                        {
                            Id = AuthorizationIdToDel2,
                            Code = "tenant:test2",
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                            Description = "To be deleted",
                            Version = AuthorizationIdToDel2
                        },
                         new()
                        {
                            Id = AuthorizationIdToDel3,
                            Code = "tenant:test3",
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                            Description = "To be deleted",
                            Version = AuthorizationIdToDel3
                        },
                         new()
                        {
                            Id = AuthorizationId3,
                            Code = "user:read",
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                            Description = "Can read users.",
                            Version = AuthorizationId3
                        },
                         new()
                        {
                            Id = AuthorizationId4,
                            Code = "tenant-user-role:write",
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                            Description = "Can write user roles in a tenant.",
                            Version = AuthorizationId4
                        },
                         new()
                        {
                            Id = AuthorizationId5,
                            Code = "tenant-role:read",
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                            Description = "Can read roles for a tenant.",
                            Version = AuthorizationId5
                        }
                };
                await ctx.Authorizations.AddRangeAsync(authorizations);
                await ctx.SaveChangesAsync();
            }

            if (!ctx.Roles.Any())
            {
                var roles = new RoleModel[]
                {
                        new()
                        {
                            Id = RoleId1,
                            Code = "TenantManager",
                            Description = "Can manage tenant information and user roles.",
                            Version = RoleId1,
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                        },
                        new()
                        {
                            Id = RoleId2,
                            Code = "TenantViewer",
                            Description = "Can read tenant information and see user roles.",
                            Version = RoleId2,
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                        },
                        new()
                        {
                            Id = RoleIdToDel1,
                            Code = "TenantManagerToDel",
                            Description = "To be deleted",
                            Version = RoleIdToDel1,
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                        },
                        new()
                        {
                            Id = RoleIdToDel2,
                            Code = "TenantViewerToDel",
                            Description = "To be deleted",
                            Version = RoleIdToDel2,
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                        },
                        new()
                        {
                            Id = RoleIdToDel3,
                            Code = "TenantViewerToDel2",
                            Description = "To be deleted",
                            Version = RoleIdToDel3,
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                        }
                };

                await ctx.Roles.AddRangeAsync(roles);
                await ctx.SaveChangesAsync();
            }

            if (!ctx.RolesAuthorizations.Any())
            {
                var now = DateTime.UtcNow;
                var audit = new AuditData(now, AdminUserId, now, AdminUserId);

                var rolesAuthorizations = new RoleAuthorizationModel[]
                {
                    new()
                    {
                        Id = RoleAuhtorizationId1,
                        RoleId = RoleId1,
                        AuthorizationId = AuthorizationId1,
                        Version = RoleAuhtorizationId1,
                        AuditInfo = new(audit.CreatedAt, audit.CreatedBy, audit.ModifiedAt, audit.ModifiedBy)
                    },
                    new()
                    {
                        Id = RoleAuhtorizationId2,
                        RoleId = RoleId1,
                        AuthorizationId = AuthorizationId2,
                        Version = RoleAuhtorizationId2,
                        AuditInfo = new(audit.CreatedAt, audit.CreatedBy, audit.ModifiedAt, audit.ModifiedBy)
                    },
                    new()
                    {
                        Id = RoleAuhtorizationId3,
                        RoleId = RoleIdToDel1,
                        AuthorizationId = AuthorizationId1,
                        Version = RoleAuhtorizationId3,
                        AuditInfo = new(audit.CreatedAt, audit.CreatedBy, audit.ModifiedAt, audit.ModifiedBy)
                    },
                    new()
                    {
                        Id = RoleAuhtorizationId4,
                        RoleId = RoleIdToDel1,
                        AuthorizationId = AuthorizationId2,
                        Version = RoleAuhtorizationId4,
                        AuditInfo = new(audit.CreatedAt, audit.CreatedBy, audit.ModifiedAt, audit.ModifiedBy)
                    },
                    new()
                    {
                        Id = RoleAuhtorizationId5,
                        RoleId = RoleId1,
                        AuthorizationId = AuthorizationId3,
                        Version = RoleAuhtorizationId5,
                        AuditInfo = new(audit.CreatedAt, audit.CreatedBy, audit.ModifiedAt, audit.ModifiedBy)
                    },
                    new()
                    {
                        Id = RoleAuhtorizationId6,
                        RoleId = RoleId1,
                        AuthorizationId = AuthorizationId4,
                        Version = RoleAuhtorizationId6,
                        AuditInfo = new(audit.CreatedAt, audit.CreatedBy, audit.ModifiedAt, audit.ModifiedBy)
                    },
                    new()
                    {
                        Id = RoleAuhtorizationId7,
                        RoleId = RoleId1,
                        AuthorizationId = AuthorizationId5,
                        Version = RoleAuhtorizationId7,
                        AuditInfo = new(audit.CreatedAt, audit.CreatedBy, audit.ModifiedAt, audit.ModifiedBy)
                    },

                };
                await ctx.RolesAuthorizations.AddRangeAsync(rolesAuthorizations);
                await ctx.SaveChangesAsync();

            }

            if (!ctx.Users.Any())
            {
                var now = DateTime.UtcNow;
                var audit = new AuditData(now, AdminUserId, now, AdminUserId);

                var users = new UserModel[]
                {
                        new()
                        {
                            Id = AdminUserId,
                            AuthId = AuthIdAdmin,
                            Email = "admin@admin.com",
                            Firstname = "Admin",
                            Lastname = "Admin",
                            IsMegaAdmin = true,
                            Version = AdminUserId,
                            AuditInfo = new(audit.CreatedAt, audit.CreatedBy, audit.ModifiedAt, audit.ModifiedBy)
                        },
                        new()
                        {
                            Id = UserId2,
                            AuthId = AuthIdUser2,
                            Email = "user2@admin.com",
                            Firstname = "User",
                            Lastname = "2",
                            IsMegaAdmin = false,
                            SelectedTenantId = null,
                            Version = UserId2,
                            AuditInfo = new(audit.CreatedAt, audit.CreatedBy, audit.ModifiedAt, audit.ModifiedBy)
                        },
                        new()
                        {
                            Id = UserId1,
                            AuthId = AuthIdUser1,
                            Email = "user1@admin.com",
                            Firstname = "User",
                            Lastname = "1",
                            IsMegaAdmin = false,
                            SelectedTenantId = null,
                            Version = UserId1,
                            AuditInfo = new(audit.CreatedAt, audit.CreatedBy, audit.ModifiedAt, audit.ModifiedBy)
                        },
                        new()
                        {
                            Id = UserId3,
                            AuthId = "notinauth",
                            Email = "user3@admin.com",
                            Firstname = "User",
                            Lastname = "3",
                            IsMegaAdmin = false,
                            SelectedTenantId = null,
                            Version = UserId3,
                            AuditInfo = new(audit.CreatedAt, audit.CreatedBy, audit.ModifiedAt, audit.ModifiedBy)
                        },
                        new() //Not really used anymore because the user is not active at the top lvl (managed by you oauth provider, but inactivity is managed at subscription lvl)
                        {
                            Id = UserIdInactivated,
                            AuthId = AuthIdUserInactive,
                            Email = "userinactive@ubiko.ch",
                            Firstname = "User",
                            Lastname = "Inactive",
                            IsMegaAdmin = false,
                            SelectedTenantId = null,
                            Version = UserIdInactivated,
                            AuditInfo = new(audit.CreatedAt, audit.CreatedBy, audit.ModifiedAt, audit.ModifiedBy)
                        },
                };

                await ctx.Users.AddRangeAsync(users);
                await ctx.SaveChangesAsync();
            }

            if (!ctx.Subscriptions.Any())
            {
                var subscriptions = new SubscriptionModel[]
                {
                        new()
                        {
                            Id = SubscriptionId1,
                            Label = "Subscription 1",
                            PlanName = "Plan pro",
                            IsActive = true,
                            MaxTenants = 7,
                            MaxUsers=3,
                            Version = SubscriptionId1
                        },
                        new()
                        {
                            Id = SubscriptionId2,
                            Label = "Subscription 2",
                            PlanName = "Plan free",
                            IsActive = true,
                            MaxTenants = 1,
                            MaxUsers=2,
                            Version = SubscriptionId2
                        },
                        new()
                        {
                            Id = SubscriptionId3,
                            Label = "Subscription 3",
                            PlanName = "Plan pro",
                            IsActive = true,
                            MaxTenants = 2,
                            MaxUsers=3,
                            Version = SubscriptionId3
                        }
                };
                await ctx.Subscriptions.AddRangeAsync(subscriptions);
                await ctx.SaveChangesAsync();
            }

            if (!ctx.SubscribtionsUsers.Any())
            {
                var subscriptionsUsers = new SubscriptionUserModel[]
                {
                    new()
                    {
                        Id = SubscriptionUserId1,
                        UserId = UserId1,
                        SubscriptionId = SubscriptionId1,
                        IsActivated = true,
                        IsOwner = true,
                        Version = SubscriptionUserId1,
                        AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                    },
                    new()
                    {
                        Id = SubscriptionUserId2,
                        UserId = UserId2,
                        SubscriptionId = SubscriptionId2,
                        IsActivated = true,
                        Version = SubscriptionUserId2,
                        AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                    },
                    new()
                    {
                        Id = SubscriptionUserId3,
                        UserId = UserId1,
                        SubscriptionId = SubscriptionId3,
                        IsActivated = true,
                        Version = SubscriptionUserId3,
                        AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                    },
                    new()
                    {
                        Id = SubscriptionUserId4,
                        UserId = UserId3,
                        SubscriptionId = SubscriptionId1,
                        IsActivated = true,
                        Version = SubscriptionUserId4,
                        AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                    },
                };

                await ctx.SubscribtionsUsers.AddRangeAsync(subscriptionsUsers);
                await ctx.SaveChangesAsync();
            }

            if (!ctx.Tenants.Any())
            {
                var tenants = new TenantModel[]
                {
                        new()
                        {
                            Id = TenantId1,
                            IsActivated = true,
                            Label = "Tenant 1",
                            Version = TenantId1,
                            SubscriptionId = SubscriptionId1,
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId)
                        },
                        new()
                        {
                            Id = TenantId2,
                            IsActivated = true,
                            Label = "Tenant 2",
                            SubscriptionId = SubscriptionId2,
                            Version = TenantId2,
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId)
                        },
                         new()
                        {
                            Id = TenantId3,
                            IsActivated = true,
                            Label = "Tenant 3",
                            SubscriptionId = SubscriptionId1,
                            Version = TenantId3,
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId)
                        },
                         new()
                        {
                            Id = TenantId4,
                            IsActivated = true,
                            Label = "Tenant 4",
                            SubscriptionId = SubscriptionId3,
                            Version = TenantId4,
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId)
                        },
                         new()
                        {
                            Id = TenantToDeleteId1,
                            IsActivated = true,
                            Label = "Tenant to delete",
                            SubscriptionId = SubscriptionId1,
                            Version = TenantToDeleteId1,
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId)
                        },
                          new()
                        {
                            Id = TenantToDeleteId2,
                            IsActivated = true,
                            Label = "Tenant to delete 2",
                            SubscriptionId = SubscriptionId1,
                            Version = TenantToDeleteId2,
                            AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId)
                        }
                };

                await ctx.Tenants.AddRangeAsync(tenants);
                await ctx.SaveChangesAsync();

                //Selected tenant
                await ctx.Users
                    .Where(u => u.Id == UserId1)
                    .ExecuteUpdateAsync(u => u
                    .SetProperty(user => user.SelectedTenantId, TenantId1));

                await ctx.Users
                    .Where(u => u.Id == UserId2)
                    .ExecuteUpdateAsync(u => u
                    .SetProperty(user => user.SelectedTenantId, TenantId2));
            }


            if (!ctx.TenantsUsers.Any())
            {
                var tenantsUsers = new TenantUserModel[]
                {
                    new()
                    {
                        Id = TenantUserId1,
                        UserId = UserId1,
                        TenantId = TenantId1,
                        Version = TenantUserId1,
                        AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                    },
                    new()
                    {
                        Id = TenantUserId2,
                        UserId = UserId2,
                        TenantId = TenantId2,
                        Version = TenantUserId2,
                        AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                    },
                    new()
                    {
                        Id = TenantUserId3,
                        UserId = UserId1,
                        TenantId = TenantId3,
                        Version = TenantUserId3,
                        AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                    },
                    new()
                    {
                        Id = TenantUserId4,
                        UserId = UserId1,
                        TenantId = TenantId4,
                        Version = TenantUserId4,
                        AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                    },
                    new()
                    {
                        Id = TenantUserToDeleteId1,
                        UserId = UserId1,
                        TenantId = TenantToDeleteId1,
                        Version = TenantUserToDeleteId1,
                        AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                    },
                    new()
                    {
                        Id = TenantUserToDeleteId2,
                        UserId = UserId3,
                        TenantId = TenantToDeleteId1,
                        Version = TenantUserToDeleteId2,
                        AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                    },
                    new()
                    {
                        Id = TenantUserToDeleteId3,
                        UserId = UserId1,
                        TenantId = TenantToDeleteId2,
                        Version = TenantUserToDeleteId3,
                        AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                    },
                    new()
                    {
                        Id = TenantUserToDeleteId4,
                        UserId = UserId3,
                        TenantId = TenantToDeleteId2,
                        Version = TenantUserToDeleteId4,
                        AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                    }
                };

                await ctx.TenantsUsers.AddRangeAsync(tenantsUsers);
                await ctx.SaveChangesAsync();
            }

            if (!ctx.TenantsUsersRoles.Any())
            {
                var tenantsUsersRoles = new TenantUserRoleModel[]
                {
                    new() {
                        Id = TenantUserRoleId1,
                        TenantUserId = TenantUserId1,
                        RoleId = RoleId1,
                        Version = TenantUserRoleId1,
                        AuditInfo = new(DateTime.UtcNow, AdminUserId, DateTime.UtcNow, AdminUserId),
                    }
                };

                await ctx.TenantsUsersRoles.AddRangeAsync(tenantsUsersRoles);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
