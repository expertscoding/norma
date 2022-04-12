namespace EC.Norma.Entities
{
    public class PermissionsRequirement
    {
        public int Id { get; set; }

        public int IdPermission { get; set; }
        public virtual Permission Permission { get; set; }


        public int IdRequirement { get; set; }
        public virtual Requirement Requirement { get; set; }

    }
}
