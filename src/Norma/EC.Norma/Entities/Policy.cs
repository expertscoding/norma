
using System.Collections.Generic;

namespace EC.Norma.Entities
{
    public class Policy
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<PolicyPriorityGroup> PoliciesPriorityGroups { get; set; }
    }
}
