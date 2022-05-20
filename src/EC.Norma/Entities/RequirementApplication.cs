namespace EC.Norma.Entities
{
    public class RequirementApplication
    {
        public int Id { get; set; }

        public int IdRequirement { get; set; }
        public virtual Requirement Requirement { get; set; }

        public int IdApplication { get; set; }
        public virtual Application Application { get; set; }

        public bool IsDefault { get; set; }
    }
}
