namespace EC.Norma.Options
{
    public class NormaOptions
    {
        public NoPermissionsBehaviour NoPermissionAction { get; set; } = NoPermissionsBehaviour.Success;

        public MissingRequirementBehaviour MissingRequirementAction { get; set; } = MissingRequirementBehaviour.ThrowException;

        public string AdministratorRoleName { get; set; } = "Administrator";
    }
}
