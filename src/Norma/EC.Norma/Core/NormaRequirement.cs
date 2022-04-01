using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Norma.Core
{
    public class NormaRequirement : IAuthorizationRequirement
    {
        public string Action { get; set; }

        public string Resource { get; set; }

        public string Permission { get; set; }

        public int Priority { get; set; }

        public NormaRequirement()
        {

        }

        public NormaRequirement(string action, string resource)
        {
            Action = action;
            Resource = resource;
        }

        public NormaRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
