using EC.Norma.Entities;
using System.Collections.Generic;
using System.Linq;
using Action = EC.Norma.Entities.Action;

namespace EC.Norma.Json
{
    public class NormaContext
    {
        public virtual List<Action> Actions { get; set; } = new List<EC.Norma.Entities.Action>();

        public virtual List<ActionsPolicy> ActionsPolicies { get; set; } = new List<ActionsPolicy>();

        public virtual List<Assignment> Assignments { get; set; } = new List<Assignment>();

        public virtual List<Permission> Permissions { get; set; } = new List<Permission>();

        public virtual List<PermissionsPolicy> PermissionsPolicies { get; set; } = new List<PermissionsPolicy>();

        public virtual List<Policy> Policies { get; set; } = new List<Policy>();

        public virtual List<Profile> Profiles { get; set; } = new List<Profile>();

        public virtual List<Resource> Resources { get; set; } = new List<Resource>();

        internal NormaContext(IEnumerable<EC.Norma.Json.Entities.Profile> jsonProfiles)
        {

            InitializeDefaults();

            Normalize(jsonProfiles);
        }

        private void InitializeDefaults()
        {
            Policies.Add(new Policy
            {
                Id = 1,
                Name = "HasPermission"
            }) ;

            Actions.Add(new Action
            {
                Id = 1,
                Name = "defaultAction"
            });

            Resources.Add(new Resource 
            { 
                Id=1,
                Name = "defaultResource"
            });

            var ap = new ActionsPolicy
            {
                Id=1,
                Action = Actions[0],
                Policy=Policies[0],
                IdPolicy = Policies[0].Id
            };

            ActionsPolicies.Add(ap);
            Actions[0].ActionPolicies = new List<ActionsPolicy> { ap };
            
        }

        private void Normalize(IEnumerable<Entities.Profile> jsonProfiles)
        {
            var i = 0;

            foreach(var profile in jsonProfiles)
            {
                i++;
                var pro = new Profile { Id = i, Name = profile.Name };
                Profiles.Add(pro);

                foreach(var permission in profile.Permissions)
                {
                    var per = Permissions.FirstOrDefault(p => p.Name == permission);

                    if (per == null)
                    {
                        per = new Permission 
                        { 
                            Id = Permissions.Count + 1,
                            Name = permission, 
                            Action= Actions[0], 
                            IdAction=1, 
                            Resource=Resources[0], 
                            IdResource=1
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
