namespace EC.Norma.Core
{
    public class IsAdminRequirement : NormaRequirement
    {
        public IsAdminRequirement() : base(null)
        {
            
        }

        public IsAdminRequirement(string action, string resource) : base(action, resource)
        {
            
        }

        public IsAdminRequirement(string permission) : base(permission)
        {
            
        }
    }
}