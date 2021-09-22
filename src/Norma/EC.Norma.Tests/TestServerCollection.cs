using Xunit;

namespace EC.Norma.Tests
{
    [CollectionDefinition("TestServer collection")]
    public class TestServerCollection : ICollectionFixture<NormaTestsFixture<Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}