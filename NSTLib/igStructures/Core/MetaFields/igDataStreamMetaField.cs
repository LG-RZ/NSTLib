using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core.MetaFields
{
    // Actual Name of the MetaField is unknown
    public class igDataStreamMetaField : igMetaField
    {
        [igField(0, typeof(igIntMetaField))]
        public int _size;
        [igField(4, typeof(igIntMetaField))]
        public int _unknown;
        [igField(8, typeof(igLongMetaField))]
        public long _dataOffset;

        public override object readField(ExtendedBinaryReader reader)
        {
            long RelativePosition = reader.RelativePosition;

            reader.RelativePosition = _container._dataOffset;
            reader.Position = _dataOffset & 0x7ffffff;
            byte[] buffer = reader.ReadBytes(_size);

            reader.RelativePosition = RelativePosition;
            return buffer;
        }
    }
}
