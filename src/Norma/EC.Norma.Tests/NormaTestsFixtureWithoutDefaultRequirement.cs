using System;
using EC.Norma.EF;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EC.Norma.Tests
{
    public class NormaTestsFixtureWithoutDefaultRequirement<T> : NormaTestsFixture<T>, IDisposable where T : class
    {
        public NormaTestsFixtureWithoutDefaultRequirement() : base(nameof(NormaTestsFixture<T>))
        {
            EditTestData();
        }

        public void EditTestData()
        {
            var db = WebAppFactory.Services.GetRequiredService<NormaContext>();
            db.RequirementsApplications.Where(ra => ra.IsDefault).ForEachAsync(ra => ra.IsDefault = false).ConfigureAwait(false).GetAwaiter();
            db.SaveChanges();
        }
    }
}
