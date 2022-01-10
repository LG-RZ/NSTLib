using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core.MetaFields
{
    public class igBooleanMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadBoolean();
        }

        public override void writeField(object @object, BinaryWriter writer)
        {
            writer.Write((bool)@object);
        }
    }
}
