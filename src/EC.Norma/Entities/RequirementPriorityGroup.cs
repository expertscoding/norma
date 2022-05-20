namespace EC.Norma.Entities
{
    public class RequirementPriorityGroup
    {
        public int Id { get; set; }

        public int IdPriorityGroup { get; set; }
        public virtual PriorityGroup PriorityGroup { get; set; }

        public int IdRequirement { get; set; }
        public virtual Requirement Requirement { get; set; }
    }
}
