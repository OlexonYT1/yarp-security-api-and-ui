namespace UbikLink.IntegrationTests
{
    enum TokenType
    {
        MegaAdmin,
        User,
        OtherTenant,
        Inactive
    }

    [Collection("AspireApp collection")]
    public abstract class BaseIntegrationTest(AspireFixture fixture) : IDisposable
    {
        internal AspireFixture AspireApp { get; private set; } = fixture;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
