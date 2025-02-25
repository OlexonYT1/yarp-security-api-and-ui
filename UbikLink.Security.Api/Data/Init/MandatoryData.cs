using UbikLink.Common.Db;
using UbikLink.Security.Api.Data.Models;

namespace UbikLink.Security.Api.Data.Init
{
    public static class MandatoryData
    {
        public static Guid SystemUserId { get; } = new("dc820000-5dd4-0015-b13b-08dd55cc80f7");

        internal static async Task LoadAsync(SecurityDbContext ctx)
        {
            if ((await ctx.Users.FindAsync(SystemUserId)) == null)
            {
                var now = DateTime.UtcNow;
                var audit = new AuditData(now, SystemUserId, now, SystemUserId);

                var user = new UserModel()
                {
                    Id = SystemUserId,
                    AuthId = "internal",
                    Email = "noiterror@admin.com",
                    Firstname = "Hal",
                    Lastname = "9000",
                    IsMegaAdmin = false,
                    Version = SystemUserId,
                    AuditInfo = new(audit.CreatedAt, audit.CreatedBy, audit.ModifiedAt, audit.ModifiedBy)
                };

                await ctx.Users.AddAsync(user);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
