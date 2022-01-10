using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core.MetaFields
{
    public class igMemoryRefMetaField : igMetaField
    {
        [igField(0, typeof(igIntMetaField))]
        private int _memSize;
        [igField(7, typeof(igBooleanMetaField))]
        private bool _active;
        [igField(8, typeof(igLongMetaField))]
        private long _offset;

        public override object readField(ExtendedBinaryReader reader)
        {
            reader.Position = _offset;
            byte[] buffer = new byte[_memSize];
            reader.BaseStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
