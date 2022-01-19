using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core
{
    public class igObjectList : igTDataList<igObject>
    {
        public static igObjectList readObjectsWithoutFields(ExtendedBinaryReader reader, IGZ container, int memoryPoolIndex, igNameList names = null)
        {
            igObjectList igObjectList = (igObjectList)readWithoutFields(reader, container, memoryPoolIndex);
            igObjectList.baseReadFields(reader);
            igObjectList.Items = new List<igObject>();

            long[] offsets = new long[igObjectList.getCount()];
            Buffer.BlockCopy(igObjectList.getRawData(), 0, offsets, 0, offsets.Length * 8);

            for (int i = 0; i < igObjectList.getCount(); i++)
            {
                if (offsets[i] != 0)
                {
                    if((offsets[i] & 0x8000000) != 0)
                    {
                        bool isLastPool = container._memoryPools.Count <= (memoryPoolIndex + 1);
                        long PositionRelative = reader.RelativePosition;
                        uint Offset = (uint)(offsets[i] & 0x7ffffff);

                        reader.RelativePosition = container._memoryPools[isLastPool ? memoryPoolIndex : memoryPoolIndex + 1].Item1;
                        reader.Position = Offset;

                        igObjectList.Items.Add(getObject(reader, igObjectList._container, isLastPool ? memoryPoolIndex : memoryPoolIndex + 1, false));
                        if (names != null)
                            igObjectList.Items[i]._metaName = names.Items[i]._name;

                        reader.RelativePosition = PositionRelative;
                    }
                    else
                    {
                        reader.RelativePosition = container._memoryPools[memoryPoolIndex].Item1;

                        reader.Position = offsets[i];
                        igObjectList.Items.Add(getObject(reader, igObjectList._container, memoryPoolIndex, false));
                        if (names != null)
                            igObjectList.Items[i]._metaName = names.Items[i]._name;
                    }
                }
            }

            return igObjectList;
        }

        public void readFieldsOfObjects(ExtendedBinaryReader reader)
        {
            for(int i = 0; i < Items.Count; i++)
            {
                long PositionRelative = reader.RelativePosition;

                reader.RelativePosition = _container._memoryPools[Items[i]._memoryPoolIndex].Item1;
                reader.Position = Items[i]._offset + 16;
                Items[i].readFields(reader);

                reader.RelativePosition = PositionRelative;
            }
        }

        public override void readFields(ExtendedBinaryReader reader)
        {
            base.readFields(reader);
        }
    }
}
