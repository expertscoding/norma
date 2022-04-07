using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Norma.Entities
{
    public class Module
    {
        public int Id { get; set; }

        public String Name { get; set; }

        public int IdApplication { get; set; }
        public virtual Application Application  { get; set; }
    }
}
