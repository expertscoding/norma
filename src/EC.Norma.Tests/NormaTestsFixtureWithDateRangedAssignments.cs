using System;
using EC.Norma.EF;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EC.Norma.TestUtils;
using System.Collections.Generic;
using EC.Norma.Entities;

namespace EC.Norma.Tests
{
    public class NormaTestsFixtureWithDateRangedAssignments<T> : NormaTestsFixtureBase<T>, IDisposable where T : class
    {
        public NormaTestsFixtureWithDateRangedAssignments() : base(nameof(NormaTestsFixtureWithDateRangedAssignments<T>))
        {
            EditTestData();
        }

        public void EditTestData()
        {
            var db = WebAppFactory.Services.GetRequiredService<NormaContext>();

            var actionId = Sequencer.GetId();
            var action = new Entities.Action { Id = actionId, Name = $"{nameof(TestController.PlainAction)}" };
            db.Actions.Add(action);

            var requirement = db.Requirements.First();//.Where(r => r.Name == "NoClass").First();
            db.ActionsRequirements.Add(new ActionRequirement { Id = Sequencer.GetId(), IdAction = actionId, IdRequirement = requirement.Id, Requirement = requirement });

            var resource = db.Resources.First();
            var permission = new Permission { Id = Sequencer.GetId(), Name = "PlainAction-Test", IdAction = action.Id, Action = action, IdResource = resource.Id, Resource = resource };
            db.Permissions.Add(permission);

            var profile1 = new Profile { Id = Sequencer.GetId(), Name = "Outdated Range Admin" };
            var profile2 = new Profile { Id = Sequencer.GetId(), Name = "With Range User" };
            var profile3 = new Profile { Id = Sequencer.GetId(), Name = "Without Range User" };
            db.Profiles.Add(profile1);
            db.Profiles.Add(profile2);
            db.Profiles.Add(profile3);

            CreateAssignment(db, profile1, permission, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(-1));
            CreateAssignment(db, profile2, permission, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(1));
            CreateAssignment(db, profile3, permission, null, null);
            db.SaveChanges();
        }

        private static void CreateAssignment(NormaContext db, Profile profile, Permission permission, DateTime? startDate, DateTime? endDate)
        {
            var assignment = new Assignment
            {
                Id = Sequencer.GetId(),
                IdProfile = profile.Id,
                Profile = profile,
                Permission = permission,
                IdPermission = permission.Id,
                EndDate = endDate,
                StartDate = startDate
            };

            db.Assignments.Add(assignment);
        }
    }
}
