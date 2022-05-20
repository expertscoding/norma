using System.Collections.Generic;

namespace EC.Norma.Entities
{
    public class Requirement
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<RequirementPriorityGroup> RequirementsPriorityGroups { get; set; }

        public virtual ICollection<RequirementApplication> RequirementsApplications { get; set; }

        public bool IsDefault { get; set; } = false;
    }
}
