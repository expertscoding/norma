using System;

namespace EC.Norma.Tests
{
    public class NormaTestsFixtureWithDefaultRequirement<T> : NormaTestsFixture<T>, IDisposable where T : class
    {
        public NormaTestsFixtureWithDefaultRequirement() :base(nameof(NormaTestsFixtureWithDefaultRequirement<T>))
        {
            
        }
    }
}
