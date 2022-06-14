using EC.Norma.Core;
using EC.Norma.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace EC.Norma.EF.Providers
{
    public class EFNormaProvider : INormaProvider
    {
        private readonly NormaContext db;

        public EFNormaProvider(NormaContext ctx)
        {
            db = ctx;
        }

        public ICollection<Assignment> GetAssignmentsForRoles(string permissionName, IEnumerable<string> profiles)
        {
            return db.Assignments.Where(a => a.Permission.Name == permissionName && profiles.Contains(a.Profile.Name)).ToList();
        }

        public ICollection<Permission> GetPermissions(string action, string resource)
        {
            return db.Permissions.Where(p => p.Action.Name == action && p.Resource.Name == resource).ToList();
        }

        public ICollection<Permission> GetPermissions(string permissionName)
        {
            return db.Permissions.Where(p => p.Name == permissionName).ToList();
        }

        public ICollection<Requirement> GetRequirementsForAction(string action)
        {
            return db.ActionsRequirements.Where(ap => ap.Action.Name == action).Select(ap => ap.Requirement).ToList();
        }

        public ICollection<Requirement> GetRequirementsForActionResource( string actionName, string resourceName )
        {
            var permissions = db.Permissions
                .Where(p => p.Action.Name == actionName && p.Resource.Name == resourceName)
                .Select(p => p.Id).ToList();

            var list = db.PermissionsRequirements.AsNoTracking()
                        .Include(x => x.Requirement.RequirementsApplications)
                        .Include(x => x.Requirement.RequirementsPriorityGroups)
                        .ThenInclude(x => x.PriorityGroup)
                        .Where(x => permissions.Contains(x.IdPermission))
                        .Select(x => x.Requirement).ToList();

            var requirements = db.ActionsRequirements.AsNoTracking()
                .Include(x => x.Requirement.RequirementsApplications)
                .Include(x => x.Requirement.RequirementsPriorityGroups)
                .ThenInclude(x => x.PriorityGroup)
                .Where(x => x.Action.Name == actionName)
                .Select(x => x.Requirement).ToList();
            list.AddRange(requirements);

            return list;
        }


        public ICollection<Requirement> GetRequirementsForPermission(string permissionName)
        {
            var list = db.PermissionsRequirements.AsNoTracking()
                        .Include(x => x.Requirement.RequirementsApplications)
                        .Include(x => x.Requirement.RequirementsPriorityGroups)
                        .ThenInclude(x => x.PriorityGroup)
                        .Where(x => x.Permission.Name == permissionName)
                        .Select(x => x.Requirement).ToList();

            var actions = db.Permissions.Where(p => p.Name == permissionName).Select(p => p.Action);

            var requirements = db.ActionsRequirements.AsNoTracking()
                        .Include(x => x.Requirement.RequirementsApplications)
                        .Include(x => x.Requirement.RequirementsPriorityGroups)
                        .ThenInclude(x => x.PriorityGroup)
                        .Where(x => actions.Contains(x.Action))
                        .Select(x => x.Requirement).ToList();

            list.AddRange(requirements);

            return list;
        }

        public ICollection<Requirement> GetDefaultRequirements()
        {
            var list = db.RequirementsApplications.AsNoTracking()
                        .Include(x => x.Requirement.RequirementsApplications)
                        .Include(x => x.Requirement.RequirementsPriorityGroups)
                        .ThenInclude(x => x.PriorityGroup)
                        .Select(x => x.Requirement);

            return list.ToList();
        }
    }
}
