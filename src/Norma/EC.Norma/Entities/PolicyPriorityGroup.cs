namespace EC.Norma.Entities
{
    public class PolicyPriorityGroup
    {
        public int Id { get; set; }

        public int IdPriorityGroup { get; set; }
        public virtual PriorityGroup PriorityGroup { get; set; }


        public int IdPolicy { get; set; }
        public virtual Policy Policy { get; set; }

    }
}
