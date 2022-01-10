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
        public long _offset;

        public override object readField(ExtendedBinaryReader reader)
        {
            byte[] buffer = new byte[_size];
            Buffer.BlockCopy(_container._data, (int)(_offset & ~0x8000000), buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
