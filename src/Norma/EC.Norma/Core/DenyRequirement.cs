namespace EC.Norma.Core
{
    public class DenyRequirement : NormaRequirement
    {
        public DenyRequirement() : base(null)
        {
        }

        public DenyRequirement(string action, string resource) : base(action, resource)
        {
        }

        public DenyRequirement(string permission) : base(permission)
        {
        }
    }
}
