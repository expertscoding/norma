using System;

namespace EC.Norma.Metadata
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class NormaActionAttribute : Attribute
    {
        public string Action { get; set; }

        public NormaActionAttribute(string action)
        {
            Action = action;
        }

    }
}
