using EC.Norma.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace EC.Norma.Core
{
    public class NonConfiguredRequirement : NormaRequirement
    {

        public NonConfiguredRequirement() : base()
        {

        }

        public NonConfiguredRequirement(String action, String resource) : base(action, resource)
        {

        }

        public NonConfiguredRequirement(String permission) : base(permission)
        {
        }

    }
}
