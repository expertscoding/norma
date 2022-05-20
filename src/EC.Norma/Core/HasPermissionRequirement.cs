namespace EC.Norma.Core
{
    public class HasPermissionRequirement : NormaRequirement
    {
        public HasPermissionRequirement() : base()
        {
        }

        public HasPermissionRequirement(string action, string resource) : base(action, resource)
        {
        }

        public HasPermissionRequirement(string permission) : base(permission)
        {
        }
    }
}
