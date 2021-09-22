using System;

namespace EC.Norma.Entities
{
    public class Permission
    {

        public int Id { get; set; }

        public String Name { get; set; }

        public int IdAction { get; set; }
        public virtual Action Action { get; set; }

        public int IdResource { get; set; }
        public virtual Resource Resource { get; set; }
    }
}
