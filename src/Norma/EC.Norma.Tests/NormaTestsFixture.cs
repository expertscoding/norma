﻿using System;
using System.Threading;
using EC.Norma.Entities;
using EC.Norma.EF;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Action = EC.Norma.Entities.Action;
using System.Linq;

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

            var priorityGroupId1 = Sequencer.GetId();
            var group1 = new PriorityGroup() { Id = priorityGroupId1, Name = "Group priority 1", Priority = 1 };
            db.PriorityGroups.Add(group1);
            

            var priorityGroupId2 = Sequencer.GetId();
            var group2 = new PriorityGroup() { Id = priorityGroupId2, Name = "Group priority 2", Priority = 2 };
            db.PriorityGroups.Add(group2);


            var policyHasPermissionsId = Sequencer.GetId();
            var policyHasPermission = new Policy { Id = policyHasPermissionsId, Name = "HasPermission" };
            var ppgHasPermissions = new PolicyPriorityGroup() { Id = Sequencer.GetId(), Policy = policyHasPermission, PriorityGroup = group2, IdPolicy = policyHasPermission.Id, IdPriorityGroup = group2.Id };
            db.PoliciesPriorityGroups.Add(ppgHasPermissions);
            db.Policies.Add(policyHasPermission);


            var policyAdminId = Sequencer.GetId();
            var policyAdmin = new Policy { Id = policyAdminId, Name = "IsAdmin" }; 
            var ppgAdmin = new PolicyPriorityGroup() { Id = Sequencer.GetId(), Policy = policyAdmin, PriorityGroup = group1, IdPolicy = policyAdmin.Id, IdPriorityGroup = group1.Id };
            db.PoliciesPriorityGroups.Add(ppgAdmin);
            db.Policies.Add(policyAdmin);


            var policyWithOutClass = new Policy { Id = Sequencer.GetId(), Name = "NoClass" };
            db.Policies.Add(policyWithOutClass);

            var policyWithOutConfiguredClass = new Policy { Id = Sequencer.GetId(), Name = "NonConfigured" };
            db.Policies.Add(policyWithOutConfiguredClass);

            var resource = new Resource { Id = Sequencer.GetId(), Name = TestController.Name };
            db.Resources.Add(resource);

            // PlainAction
            ConfigureAction(db, nameof(TestController.PlainAction), policyHasPermission, resource, "User", true);

            // AnnotatedAction Action -> The action name is redefined to List
            ConfigureAction(db, "List", policyHasPermission, resource, "User", false);

            // WithoutPermissions Action
            ConfigureAction(db, nameof(TestController.WithoutConfiguredRequirement), policyWithOutConfiguredClass, null, null, false);

            // WithoutPermissions Action
            ConfigureAction(db, nameof(TestController.WithoutRequirement), policyWithOutClass, null, null, false);

            // Two policies action 
            ConfigureAction(db, "TwoPoliciesAction", policyHasPermission, resource, "User", false);

            // Two policies action: configuring second policy and profile 
            ConfigureActionWithSecondPolicy(db, "TwoPoliciesAction", policyAdmin, resource, "Admin");


            db.SaveChanges();
        }


        protected void ConfigureAction(NormaContext db, string actionName, Policy policy, Resource resource, string profileName, bool assign)
        {
            // PlainAction
            var action = new Action { Id = Sequencer.GetId(), Name = actionName };
            db.Actions.Add(action);

            db.ActionsPolicies.Add(new ActionsPolicy { Id = Sequencer.GetId(), Action = action, IdAction = action.Id, Policy = policy, IdPolicy = policy.Id });

            var profile = GetOrCreateProfile(db, profileName);

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
                AddProfileToContext(db, permission, profile);
            }
        }

        private void AddProfileToContext(NormaContext db, Permission permission, Profile profile)
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

        protected Profile GetOrCreateProfile(NormaContext db, string profileName)
        {
            if (!string.IsNullOrWhiteSpace(profileName))
            {
                var profile = db.Profiles.SingleOrDefault(x => x.Name == profileName);
                if (profile == null)
                {
                    profile = new Profile { Id = Sequencer.GetId(), Name = profileName };
                    db.Profiles.Add(profile);
                }
                return profile;
            }
            return null;
        }

        protected void ConfigureActionWithSecondPolicy(NormaContext db, string actionName, Policy policy, Resource resource, string profileName)
        {
            db.SaveChanges();

            var action = db.Actions.Single(x => x.Name == actionName);

            db.ActionsPolicies.Add(new ActionsPolicy { Id = Sequencer.GetId(), Action = action, IdAction = action.Id, Policy = policy, IdPolicy = policy.Id });
            
            var profile = GetOrCreateProfile(db, profileName);

            var permission = db.Permissions.Single(x => x.Name == $"{action.Name}-{resource.Name}");

            if (profile != null && permission != null)
            {
                AddProfileToContext(db, permission, profile);
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