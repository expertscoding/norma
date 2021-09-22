namespace EC.Norma.Entities
{
    public class ActionsPolicy
    {
        public int Id { get; set; }

        public int IdAction { get; set; }
        public virtual Action Action { get; set; }


        public int IdPolicy { get; set; }
        public virtual Policy Policy { get; set; }

    }
}
