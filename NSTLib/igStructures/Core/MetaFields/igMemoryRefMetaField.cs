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
        protected int _memSize;
        [igField(7, typeof(igBooleanMetaField))]
        protected bool _active;
        [igField(8, typeof(igLongMetaField))]
        protected long _dataOffset;

        public override object readField(ExtendedBinaryReader reader)
        {
            if((_dataOffset & 0x8000000) != 0)
            {
                bool isLastPool = _container._memoryPools.Count <= (_memoryPoolIndex + 1);
                long PositionRelative = reader.RelativePosition;
                uint Offset = (uint)(_dataOffset & 0x7ffffff);

                reader.RelativePosition = _container._memoryPools[isLastPool ? _memoryPoolIndex : _memoryPoolIndex + 1].Item1;
                reader.Position = Offset;

                byte[] buffer = new byte[_memSize];
                reader.BaseStream.Read(buffer, 0, buffer.Length);

                reader.RelativePosition = PositionRelative;

                return buffer;
            }
            else
            {
                reader.Position = _dataOffset;
                byte[] buffer = new byte[_memSize];
                reader.BaseStream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}
