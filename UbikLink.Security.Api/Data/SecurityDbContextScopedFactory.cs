using Microsoft.EntityFrameworkCore;
using UbikLink.Common.Api;

namespace UbikLink.Security.Api.Data
{
    public class SecurityDbContextScopedFactory(
        IDbContextFactory<SecurityDbContext> pooledFactory,
        ICurrentUser currentUser) : IDbContextFactory<SecurityDbContext>
    {
        private readonly IDbContextFactory<SecurityDbContext> _pooledFactory = pooledFactory;
        private readonly ICurrentUser? _currentUser = currentUser;

        public SecurityDbContext CreateDbContext()
        {
            //Modify here if you want to change the db connection based on the user tenant.
            //For now it's used with Query filter to separate the tenants but it can be changed based on specifc requirements.
           var context = _pooledFactory.CreateDbContext();
            context.CurrentUser = _currentUser ?? throw new NullReferenceException("A dbcontext cannot be pooled without a user context");
            return context;
        }
    }
}
