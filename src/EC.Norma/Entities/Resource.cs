namespace EC.Norma.Entities
{
    public class Resource
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int IdModule { get; set; }
        public virtual Module Module { get; set; }
    }
}
