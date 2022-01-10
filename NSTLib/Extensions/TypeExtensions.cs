using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.Extensions
{
    public static class TypeExtensions
    {
        public static FieldInfo[] GetAllFields(this Type type, BindingFlags flags)
        {
            HashSet<FieldInfo> fields = new HashSet<FieldInfo>(new FieldInfoComparer());

            Type currentType = type;
            while(currentType.BaseType != null)
            {
                fields.UnionWith(currentType.GetFields(flags));
                currentType = currentType.BaseType;
            }
            return fields.ToArray();
        }

        private class FieldInfoComparer : IEqualityComparer<FieldInfo>
        {
            public bool Equals(FieldInfo x, FieldInfo y)
            {
                return x.DeclaringType == y.DeclaringType && x.Name == y.Name;
            }

            public int GetHashCode(FieldInfo obj)
            {
                return obj.Name.GetHashCode() ^ obj.DeclaringType.GetHashCode();
            }
        }
    }
}
