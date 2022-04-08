using EC.Norma.Core;
using EC.Norma.Entities;
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

        public ICollection<Requirement> GetRequirementsForActionResource(string actionName, string resourceName)
        {
            var permissions = db.Permissions
                .Where(p => p.Action.Name == actionName && p.Resource.Name == resourceName)
                .Select(p => p.Id);

            var list = db.PermissionsRequirements.Where(pp => permissions.Contains(pp.IdPermission))
                        .Select(pp => pp.Requirement);


            list = list.Union(db.ActionsRequirements.Where(ap => ap.Action.Name == actionName).Select(ap => ap.Requirement));

            return list.ToList();
        }


        public ICollection<Requirement> GetRequirementsForPermission(string permissionName)
        {
            var list = db.PermissionsRequirements.Where(pp => pp.Permission.Name == permissionName)
                        .Select(pp => pp.Requirement);

            var actions = db.Permissions.Where(p => p.Name == permissionName).Select(p => p.Action);

            list = list.Union(db.ActionsRequirements.Where(ap => actions.Contains(ap.Action)).Select(ap => ap.Requirement));

            return list.ToList();
        }
    }
}
