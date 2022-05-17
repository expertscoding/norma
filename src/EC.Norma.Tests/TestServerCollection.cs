using Xunit;

namespace EC.Norma.Tests
{
    [CollectionDefinition("TestServer collection")]
    public class TestServerCollection : ICollectionFixture<NormaTestsFixtureWithDefaultRequirement<Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [CollectionDefinition("TestServer collection Without Default Requirements")]
    public class TestServerCollection2 : ICollectionFixture<NormaTestsFixtureWithoutDefaultRequirement<Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }}
