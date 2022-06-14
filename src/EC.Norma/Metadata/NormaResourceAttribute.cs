using System;

namespace EC.Norma.Metadata
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class NormaResourceAttribute : Attribute
    {
        public string Resource { get; set; }

        public NormaResourceAttribute(string resource)
        {
            Resource = resource;
        }
    }
}
