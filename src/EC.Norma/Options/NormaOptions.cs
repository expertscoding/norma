namespace EC.Norma.Options
{
    public class NormaOptions
    {
        public NoPermissionsBehaviour NoPermissionAction { get; set; } = NoPermissionsBehaviour.Success;

        public MissingRequirementBehaviour MissingRequirementAction { get; set; } = MissingRequirementBehaviour.ThrowException;

        public string AdministratorRoleName { get; set; } = "Administrator";

        /// <summary>
        /// This property is the cache expiration time in seconds. Default value is 300 seconds (5 min)
        /// </summary>
        public int CacheExpiration { get; set; } = 300;

        public string ApplicationKey { get; set; } = string.Empty;

        public string ProfileClaim { get; set; } = "role";
    }
}
