using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KuromeKuroKit_WPF.Helpers
{
    public static class TypeHelper
    {
        public static Dictionary<Type, string> TypeNames { get; } = new Dictionary<Type, string>();
        public static string CachedNameOf<T>() 
        {
            Type type = typeof(T);
            if (TypeNames.ContainsKey(type))
            {
                return TypeNames[type];
            }
            string name = type.Name;
            TypeNames.Add(type, name);
            return name;
        }
    }
}
