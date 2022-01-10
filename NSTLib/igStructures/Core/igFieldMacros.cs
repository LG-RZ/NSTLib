using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures
{
    [AttributeUsage(AttributeTargets.Field)]
    public class igFieldAttribute : Attribute
    {
        public int FieldOffset;
        public Type MetaFieldType;

        public igFieldAttribute(int Offset, Type FieldType)
        {
            this.FieldOffset = Offset;
            this.MetaFieldType = FieldType;
        }
    }
}
