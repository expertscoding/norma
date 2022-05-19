using System;
using System.Threading;
using EC.Norma.Entities;
using EC.Norma.EF;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Action = EC.Norma.Entities.Action;

namespace EC.Norma.Options.Tests
{
    public class NormaTestsFixture<T> : IDisposable where T : class
    {
        public WebApplicationFactory<T> WebAppFactory { get; set; }
        
        public NormaTestsFixture()
        {
            WebAppFactory = new WebApplicationFactory<T>();

            CreateTestData();
        }

        public void CreateTestData()
        {
            var db = WebAppFactory.Services.GetService<NormaContext>();

            var requirement = new Requirement { Id = Sequencer.GetId(), Name = "HasPermission" };
            db.Requirements.Add(requirement);

            var requirementWithOutClass = new Requirement { Id = Sequencer.GetId(), Name = "NoClass" };
            db.Requirements.Add(requirementWithOutClass);

            var requirementWithOutConfiguredClass = new Requirement { Id = Sequencer.GetId(), Name = "NonConfigured" };
            db.Requirements.Add(requirementWithOutConfiguredClass);

            var resource = new Resource { Id = Sequencer.GetId(), Name = TestController.Name };
            db.Resources.Add(resource);

            // PlainAction
            var action = new Action { Id = Sequencer.GetId(), Name = nameof(TestController.PlainAction) };
            db.Actions.Add(action);
            
            db.ActionsRequirements.Add(new ActionRequirement { Id = Sequencer.GetId(), Action = action, IdAction = action.Id, Requirement = requirement, IdRequirement = requirement.Id });

            var profile = new Profile { Id = Sequencer.GetId(), Name = "User" };
            db.Profiles.Add(profile);

            var permission = new Permission
            {
                Id = Sequencer.GetId(),
                Name = $"{action.Name}-{resource.Name}",
                Action = action,
                IdAction = action.Id,
                Resource = resource,
                IdResource = resource.Id
            };
            db.Permissions.Add(permission);

            db.Assignments.Add(new Assignment
            {
                Id = Sequencer.GetId(), 
                Permission = permission, 
                IdPermission = permission.IdResource,
                Profile = profile, IdProfile = profile.Id
            });


            // AnnotatedAction Action -> The action name is redefined to List
            action = new Action { Id = Sequencer.GetId(), Name = "List" };
            db.Actions.Add(action);

            db.ActionsRequirements.Add(new ActionRequirement { Id = Sequencer.GetId(), Action = action, IdAction = action.Id, Requirement = requirement, IdRequirement = requirement.Id });

            permission = new Permission
            {
                Id = Sequencer.GetId(),
                Name = $"{action.Name}-{resource.Name}",
                Action = action,
                IdAction = action.Id,
                Resource = resource,
                IdResource = resource.Id
            };
            db.Permissions.Add(permission);


            // WithoutPermissions Action
            action = new Action { Id = Sequencer.GetId(), Name = nameof(TestController.WithoutConfiguredRequirement) };
            db.Actions.Add(action);

            db.ActionsRequirements.Add(new ActionRequirement { Id = Sequencer.GetId(), Action = action, IdAction = action.Id, Requirement = requirementWithOutConfiguredClass, IdRequirement = requirementWithOutConfiguredClass.Id });


            // WithoutPermissions Action
            action = new Action { Id = Sequencer.GetId(), Name = nameof(TestController.WithoutRequirement) };
            db.Actions.Add(action);

            db.ActionsRequirements.Add(new ActionRequirement { Id = Sequencer.GetId(), Action = action, IdAction = action.Id, Requirement = requirementWithOutClass, IdRequirement = requirementWithOutClass.Id });

            db.SaveChanges();
        }

        public void Dispose()
        {
            WebAppFactory?.Dispose();
        }
    }

    public static class Sequencer
    {
        private static int id;

        public static int GetId() => Interlocked.Increment(ref id);
    }
}
