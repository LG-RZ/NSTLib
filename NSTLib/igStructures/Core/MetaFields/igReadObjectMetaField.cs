using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core.MetaFields
{
    // Custom
    public class igReadObjectMetaField<T> : igMetaField
        where T : igObject, new()
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            T @object = new T();
            @object._memoryPoolIndex = _memoryPoolIndex;
            @object._container = _container;
            @object.readFields(reader);
            return @object;
        }
    }
}
