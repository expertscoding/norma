namespace EC.Norma.Core
{
    public class HeadQuartersRequirement : NormaRequirement

    {
        public HeadQuartersRequirement() : base()
        {
        }

        public HeadQuartersRequirement(string action, string resource) : base(action, resource)
        {
        }

        public HeadQuartersRequirement(string permission) : base(permission)
        {
        }
    }
}
