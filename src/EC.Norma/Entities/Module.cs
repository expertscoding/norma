namespace EC.Norma.Entities
{
    public class Module
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int IdApplication { get; set; }
        public virtual Application Application  { get; set; }
    }
}
