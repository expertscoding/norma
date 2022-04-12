namespace EC.Norma.Entities
{
    public class ActionsRequirement
    {
        public int Id { get; set; }

        public int IdAction { get; set; }
        public virtual Action Action { get; set; }


        public int IdRequirement { get; set; }
        public virtual Requirement Requirement { get; set; }

    }
}
