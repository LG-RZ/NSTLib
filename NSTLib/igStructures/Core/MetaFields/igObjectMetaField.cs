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
            uint Offset = (uint)reader.ReadInt64();
            if((Offset & 0x8000000) != 0)
            {
                bool isLastPool = _container._memoryPools.Count <= (_memoryPoolIndex + 1);
                long PositionRelative = reader.RelativePosition;
                Offset &= 0x7ffffff;

                reader.RelativePosition = _container._memoryPools[isLastPool ? _memoryPoolIndex : _memoryPoolIndex + 1].Item1;
                reader.Position = Offset;

                igObject @object = igObject.getObject(reader, _container, isLastPool ? _memoryPoolIndex : _memoryPoolIndex + 1);
                reader.RelativePosition = PositionRelative;
                return @object;
            }
            else
            {
                reader.Position = Offset;
                return reader.Position != 0 ? igObject.getObject(reader, _container, _memoryPoolIndex) : null;
            }
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