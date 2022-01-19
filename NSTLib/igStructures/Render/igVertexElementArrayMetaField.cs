using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Render
{
    public class igVertexElementArrayMetaField : igMemoryRefMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            if ((_dataOffset & 0x8000000) != 0)
            {
                bool isLastPool = _container._memoryPools.Count <= (_memoryPoolIndex + 1);
                long PositionRelative = reader.RelativePosition;
                uint Offset = (uint)(_dataOffset & 0x7ffffff);

                reader.RelativePosition = _container._memoryPools[isLastPool ? _memoryPoolIndex : _memoryPoolIndex + 1].Item1;
                reader.Position = Offset;

                try
                {
                    return reader.ReadArray<igVertexElement>(_memSize / 0xC);
                }
                finally
                {
                    reader.RelativePosition = PositionRelative;
                }
            }
            else
            {
                reader.Position = _dataOffset;
                return reader.ReadArray<igVertexElement>(_memSize / 0xC);
            }
        }
    }
}
