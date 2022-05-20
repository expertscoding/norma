using System;

namespace EC.Norma.Metadata
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ByPassNormaAttribute : Attribute
    {
    }
}
