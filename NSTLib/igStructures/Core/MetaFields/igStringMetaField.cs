using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core.MetaFields
{
    public class igStringMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return _container._stringList[reader.ReadInt32()];
        }

        public override void writeField(object @object, BinaryWriter writer)
        {
            writer.Write(_container._stringList.IndexOf((string)@object));
        }
    }
}
