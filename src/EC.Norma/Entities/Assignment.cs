namespace EC.Norma.Entities
{
    public class Assignment
    {
        public int Id { get; set; }

        public int IdPermission { get; set; }
        public virtual Permission Permission { get; set; }

        public int IdProfile { get; set; }
        public virtual Profile Profile { get; set; }
    }
}
