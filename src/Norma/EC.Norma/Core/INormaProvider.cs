using EC.Norma.Entities;
using System.Collections.Generic;

namespace EC.Norma.Core
{
    public interface INormaProvider
    {
        ICollection<Policy> GetPoliciesForPermission(string permissionName);

        ICollection<Policy> GetPoliciesForActionResource(string actionName, string resourceName);

        ICollection<Permission> GetPermissions(string action, string resource);

        ICollection<Permission> GetPermissions(string permissionName);

        ICollection<Assignment> GetAssignmentsForRoles(string permissionName, IEnumerable<string> profiles);
    }
}
