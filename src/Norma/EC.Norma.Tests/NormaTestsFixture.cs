using System;
using System.Threading;
using EC.Norma.Entities;
using EC.Norma.EF;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Action = EC.Norma.Entities.Action;

namespace EC.Norma.Tests
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

            var policy = new Policy { Id = Sequencer.GetId(), Name = "HasPermission" };
            db.Policies.Add(policy);

            var policyWithOutClass = new Policy { Id = Sequencer.GetId(), Name = "NoClass" };
            db.Policies.Add(policyWithOutClass);

            var policyWithOutConfiguredClass = new Policy { Id = Sequencer.GetId(), Name = "NonConfigured" };
            db.Policies.Add(policyWithOutConfiguredClass);

            var resource = new Resource { Id = Sequencer.GetId(), Name = TestController.Name };
            db.Resources.Add(resource);

            // PlainAction
            ConfigureAction(db, nameof(TestController.PlainAction), policy, resource, "User", true);

            // AnnotatedAction Action -> The action name is redefined to List
            ConfigureAction(db, "List", policy, resource, "User", false);
            
            // WithoutPermissions Action
            ConfigureAction(db, nameof(TestController.WithoutConfiguredRequirement), policyWithOutConfiguredClass, null, null, false);

            // WithoutPermissions Action
            ConfigureAction(db, nameof(TestController.WithoutRequirement), policyWithOutClass, null, null, false);

            db.SaveChanges();
        }


        protected void ConfigureAction(NormaContext db, string actionName, Policy policy, Resource resource, string profileName, bool assign)
        {
            // PlainAction
            var action = new Action { Id = Sequencer.GetId(), Name = actionName };
            db.Actions.Add(action);

            db.ActionsPolicies.Add(new ActionsPolicy { Id = Sequencer.GetId(), Action = action, IdAction = action.Id, Policy = policy, IdPolicy = policy.Id });

            Profile profile = null;
            if (!string.IsNullOrWhiteSpace(profileName))
            {
                profile = new Profile { Id = Sequencer.GetId(), Name = profileName };
                db.Profiles.Add(profile);
            }

            Permission permission = null;
            if (resource != null)
            {
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
            }

            if (assign && profile != null && permission != null)
            {
                db.Assignments.Add(new Assignment
                {
                    Id = Sequencer.GetId(),
                    Permission = permission,
                    IdPermission = permission.IdResource,
                    Profile = profile,
                    IdProfile = profile.Id
                });
            }
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