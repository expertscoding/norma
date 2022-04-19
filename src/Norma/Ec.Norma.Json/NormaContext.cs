using EC.Norma.Entities;
using System.Collections.Generic;
using System.Linq;
using Action = EC.Norma.Entities.Action;

namespace EC.Norma.Json
{
    public class NormaContext
    {
        public virtual List<Action> Actions { get; set; } = new List<EC.Norma.Entities.Action>();

        public virtual List<ActionRequirement> ActionsRequirements { get; set; } = new List<ActionRequirement>();

        public virtual List<Assignment> Assignments { get; set; } = new List<Assignment>();

        public virtual List<Permission> Permissions { get; set; } = new List<Permission>();

        public virtual List<PermissionRequirement> PermissionsRequirements { get; set; } = new List<PermissionRequirement>();

        public virtual List<Requirement> Requirements { get; set; } = new List<Requirement>();

        public virtual List<Profile> Profiles { get; set; } = new List<Profile>();

        public virtual List<Resource> Resources { get; set; } = new List<Resource>();

        public virtual List<Application> Applications { get; set; }

        public virtual List<Module> Modules { get; set; }

        public virtual List<PriorityGroup> PriorityGroups { get; set; }

        public virtual List<RequirementPriorityGroup> RequirementsPriorityGroups { get; set; }

        public virtual List<RequirementApplication> RequirementsApplications { get; set; }

        internal NormaContext( IEnumerable<EC.Norma.Json.Entities.Profile> jsonProfiles )
        {

            InitializeDefaults();

            Normalize(jsonProfiles);
        }

        private void InitializeDefaults()
        {
            PriorityGroups.Add(new PriorityGroup()
            {
                Id = 1,
                Name = "PriorityGroup0",
                Priority = 1
            });

            Applications.Add(new Application()
            {
                Id = 1,
                Name = "defaultApp"
            });

            Modules.Add(new Module()
            {
                Id = 1,
                Name = "defaultModule",
                Application = Applications[0],
                IdApplication = Applications[0].Id,
            });

            Requirements.Add(new Requirement
            {
                Id = 1,
                Name = "HasPermission"
            });

            RequirementsPriorityGroups.Add(new RequirementPriorityGroup()
            {
                Id = 1,
                PriorityGroup = PriorityGroups[0],
                Requirement = Requirements[0],
                IdPriorityGroup = PriorityGroups[0].Id,
                IdRequirement = Requirements[0].Id
            });

            RequirementsApplications.Add(new RequirementApplication()
            {
                Id = 1,
                Requirement = Requirements[0],
                IdRequirement = Requirements[0].Id,
                Application = Applications[0],
                IdApplication = Applications[0].Id,
                IsDefault = false
            });

            Actions.Add(new Action
            {
                Id = 1,
                Name = "defaultAction",
                Module = Modules[0],
                IdModule = Modules[0].Id
            });

            Resources.Add(new Resource
            {
                Id = 1,
                Name = "defaultResource"
            });

            var ap = new ActionRequirement
            {
                Id = 1,
                Action = Actions[0],
                Requirement = Requirements[0],
                IdRequirement = Requirements[0].Id
            };

            ActionsRequirements.Add(ap);
            Actions[0].ActionRequirements = new List<ActionRequirement> { ap };


            for (var i = 0 ; i < Requirements.Count ; i++)
            {
                Requirements[i].RequirementsPriorityGroups = RequirementsPriorityGroups.Where(x => x.IdRequirement == Requirements[i].Id).ToArray();
                Requirements[i].RequirementsApplications = RequirementsApplications.Where(x => x.IdRequirement == Requirements[i].Id).ToArray();
            }
        }

        private void Normalize( IEnumerable<Entities.Profile> jsonProfiles )
        {
            var i = 0;

            foreach (var profile in jsonProfiles)
            {
                i++;
                var pro = new Profile { Id = i, Name = profile.Name };
                Profiles.Add(pro);

                foreach (var permission in profile.Permissions)
                {
                    var per = Permissions.FirstOrDefault(p => p.Name == permission);

                    if (per == null)
                    {
                        per = new Permission
                        {
                            Id = Permissions.Count + 1,
                            Name = permission,
                            Action = Actions[0],
                            IdAction = 1,
                            Resource = Resources[0],
                            IdResource = 1
                        };

                        Permissions.Add(per);
                    }

                    Assignments.Add(new Assignment
                    {
                        Id = Assignments.Count + 1,
                        IdPermission = per.Id,
                        IdProfile = pro.Id,
                        Permission = per,
                        Profile = pro
                    });
                }
            }
        }
    }
}
