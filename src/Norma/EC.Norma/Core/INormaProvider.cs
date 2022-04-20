using EC.Norma.Entities;
using System.Collections.Generic;

namespace EC.Norma.Core
{
    public interface INormaProvider
    {
        ICollection<Requirement> GetRequirementsForPermission(string permissionName);

        ICollection<Requirement> GetRequirementsForActionResource(string actionName, string resourceName);

        ICollection<Requirement> GetDefaultRequirements();

        ICollection<Permission> GetPermissions(string action, string resource);

        ICollection<Permission> GetPermissions(string permissionName);

        ICollection<Assignment> GetAssignmentsForRoles(string permissionName, IEnumerable<string> profiles);
    }
}
