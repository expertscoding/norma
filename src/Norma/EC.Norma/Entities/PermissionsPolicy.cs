namespace EC.Norma.Entities
{
    public class PermissionsPolicy
    {
        public int Id { get; set; }

        public int IdPermission { get; set; }
        public virtual Permission Permission { get; set; }


        public int IdPolicy { get; set; }
        public virtual Policy Policy { get; set; }

    }
}
