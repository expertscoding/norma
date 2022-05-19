using System;

namespace EC.Norma.Metadata
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class NormaPermissionAttribute:Attribute
    {
        public string Permission { get; set; }

        public NormaPermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }
}
