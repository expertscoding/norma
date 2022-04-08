﻿using EC.Norma.Entities;
using System.Collections.Generic;
using System.Linq;
using Action = EC.Norma.Entities.Action;

namespace EC.Norma.Json
{
    public class NormaContext
    {
        public virtual List<Action> Actions { get; set; } = new List<EC.Norma.Entities.Action>();

        public virtual List<ActionsRequirement> ActionsRequirements { get; set; } = new List<ActionsRequirement>();

        public virtual List<Assignment> Assignments { get; set; } = new List<Assignment>();

        public virtual List<Permission> Permissions { get; set; } = new List<Permission>();

        public virtual List<PermissionsRequirement> PermissionsRequirements { get; set; } = new List<PermissionsRequirement>();

        public virtual List<Requirement> Requirements { get; set; } = new List<Requirement>();

        public virtual List<Profile> Profiles { get; set; } = new List<Profile>();

        public virtual List<Resource> Resources { get; set; } = new List<Resource>();

        internal NormaContext(IEnumerable<EC.Norma.Json.Entities.Profile> jsonProfiles)
        {

            InitializeDefaults();

            Normalize(jsonProfiles);
        }

        private void InitializeDefaults()
        {
            Requirements.Add(new Requirement
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

            var ap = new ActionsRequirement
            {
                Id=1,
                Action = Actions[0],
                Requirement=Requirements[0],
                IdRequirement = Requirements[0].Id
            };

            ActionsRequirements.Add(ap);
            Actions[0].ActionRequirements = new List<ActionsRequirement> { ap };
            
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
