using System;
using EC.Norma.Entities;
using EC.Norma.EF;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Action = EC.Norma.Entities.Action;
using System.Linq;
using EC.Norma.TestUtils;
using Microsoft.AspNetCore.Hosting;

namespace EC.Norma.Tests
{
    public abstract class NormaTestsFixture<T> : IDisposable where T : class
    {
        public WebApplicationFactory<T> WebAppFactory { get; set; }

        protected NormaTestsFixture(string dbName = nameof(NormaTestsFixture<T>))
        {
            WebAppFactory = (new WebApplicationFactory<T>()).WithWebHostBuilder(builder => builder.UseSetting("dbName", dbName).UseStartup<T>());

            CreateTestData();
        }

        public void CreateTestData()
        {
            var db = WebAppFactory.Services.GetRequiredService<NormaContext>();

            #region Priority Groups
            var priorityGroupId1 = Sequencer.GetId();
            var group1 = new PriorityGroup() { Id = priorityGroupId1, Name = "Group priority 1", Priority = 1 };
            db.PriorityGroups.Add(group1);


            var priorityGroupId2 = Sequencer.GetId();
            var group2 = new PriorityGroup() { Id = priorityGroupId2, Name = "Group priority 2", Priority = 2 };
            db.PriorityGroups.Add(group2);
            #endregion


            #region Applications
            var application1 = new Application { Id = Sequencer.GetId(), Name = "application1", Key = "application1" };
            db.Applications.Add(application1);

            var application2 = new Application { Id = Sequencer.GetId(), Name = "application2", Key = "application2" };
            db.Applications.Add(application1);
            #endregion


            #region Requirements
            var requirementHasPermissionsId = Sequencer.GetId();
            var requirementHasPermission = new Requirement { Id = requirementHasPermissionsId, Name = "HasPermission"};
            var rpgHasPermissions = new RequirementPriorityGroup() { Id = Sequencer.GetId(), Requirement = requirementHasPermission, PriorityGroup = group2, IdRequirement = requirementHasPermission.Id, IdPriorityGroup = group2.Id };       
            db.RequirementsPriorityGroups.Add(rpgHasPermissions);
            requirementHasPermission.RequirementsPriorityGroups.Add(rpgHasPermissions);

            var raHasPermissions = new RequirementApplication() { Id = Sequencer.GetId(), Requirement = requirementHasPermission, Application = application1, IdRequirement = requirementHasPermission.Id, IdApplication = application1.Id, IsDefault = true };
            db.RequirementsApplications.Add(raHasPermissions);
            requirementHasPermission.RequirementsApplications.Add(raHasPermissions);

            db.Requirements.Add(requirementHasPermission);


            var requirementAdminId = Sequencer.GetId();
            var requirementAdmin = new Requirement { Id = requirementAdminId, Name = "IsAdmin" };
            var rpgAdmin = new RequirementPriorityGroup() { Id = Sequencer.GetId(), Requirement = requirementAdmin, PriorityGroup = group1, IdRequirement = requirementAdmin.Id, IdPriorityGroup = group1.Id };
            db.RequirementsPriorityGroups.Add(rpgAdmin);
            db.Requirements.Add(requirementAdmin);


            var requirementWithOutClass = new Requirement { Id = Sequencer.GetId(), Name = "NoClass" };
            db.Requirements.Add(requirementWithOutClass);

            var requirementWithOutConfiguredClass = new Requirement { Id = Sequencer.GetId(), Name = "NonConfigured" };
            db.Requirements.Add(requirementWithOutConfiguredClass);
            #endregion


            #region Modules
            var module1 = new Module { Id = Sequencer.GetId(), Name = "module1", Application = application1, IdApplication = application1.Id };
            db.Modules.Add(module1);

            var module2 = new Module { Id = Sequencer.GetId(), Name = "module2", Application = application2, IdApplication = application2.Id };
            db.Modules.Add(module2);
            #endregion


            #region Resources
            var resource = new Resource { Id = Sequencer.GetId(), Name = TestController.Name, Module = module1, IdModule = module1.Id };
            db.Resources.Add(resource);


            var resource2 = new Resource { Id = Sequencer.GetId(), Name = TestController.Name, Module = module2, IdModule = module2.Id };
            db.Resources.Add(resource2);
            #endregion



            // PlainAction
            ConfigureAction(db, nameof(TestController.PlainAction), requirementHasPermission, resource, "User", true, module1);

            // AnnotatedAction Action -> The action name is redefined to List
            ConfigureAction(db, "List", requirementHasPermission, resource, "User", false, module1);

            // WithoutPermissions Action
            ConfigureAction(db, nameof(TestController.WithoutConfiguredRequirement), requirementWithOutConfiguredClass, null, null, false, module1);

            // WithoutPermissions Action
            ConfigureAction(db, nameof(TestController.WithoutRequirement), requirementWithOutClass, null, null, false, module1);

            //PlainActionApplication2
            ConfigureAction(db, nameof(TestController.PlainActionApplication2), requirementHasPermission, resource, "User", true, module2);

            // PlainAction Module2
            ConfigureAction(db, nameof(TestController.PlainAction), requirementHasPermission, resource2, "User", true, module2);

            // Two Requirements action 
            ConfigureAction(db, "TwoRequirementsAction", requirementHasPermission, resource, "User", false, module1);

            // Two Requirements action: configuring second requirement and profile 
            ConfigureActionWithSecondRequirement(db, "TwoRequirementsAction", requirementAdmin, resource, "Admin");


            db.SaveChanges();
        }


        protected void ConfigureAction( NormaContext db, string actionName, Requirement requirement, Resource resource, string profileName, bool assign, Module module )
        {
            // PlainAction
            var action = new Action { Id = Sequencer.GetId(), Name = actionName, Module = module, IdModule = module.Id };
            db.Actions.Add(action);

            db.ActionsRequirements.Add(new ActionRequirement { Id = Sequencer.GetId(), Action = action, IdAction = action.Id, Requirement = requirement, IdRequirement = requirement.Id });

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

        private void AddProfileToContext( NormaContext db, Permission permission, Profile profile )
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

        protected Profile GetOrCreateProfile( NormaContext db, string profileName )
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

        protected void ConfigureActionWithSecondRequirement( NormaContext db, string actionName, Requirement requirement, Resource resource, string profileName )
        {
            db.SaveChanges();

            var action = db.Actions.Single(x => x.Name == actionName);

            db.ActionsRequirements.Add(new ActionRequirement { Id = Sequencer.GetId(), Action = action, IdAction = action.Id, Requirement = requirement, IdRequirement = requirement.Id });

            var profile = GetOrCreateProfile(db, profileName);

            var permission = db.Permissions.Single(x => x.Name == $"{action.Name}-{resource.Name}");

            if (profile != null)
            {
                AddProfileToContext(db, permission, profile);
            }
        }

        public void Dispose()
        {
            WebAppFactory?.Dispose();
        }
    }
}
