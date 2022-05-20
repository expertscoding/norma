using System.Collections.Generic;

namespace EC.Norma.Json.Entities
{
    public class Profile
    {
        public string Name { get; set; }

        public ICollection<string> Permissions { get; set; } = new List<string>();

    }
}
