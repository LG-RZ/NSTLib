using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core.MetaFields
{
    public class igObjectMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            reader.Position = reader.ReadInt64();
            return reader.Position != 0 ? igObject.getObject(reader, _container) : null;
        }

        public override void writeField(object @object, BinaryWriter writer)
        {
        }

        public override int computeSize()
        {
            return 8;
        }
    }
}