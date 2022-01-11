using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core
{
    public class igObjectList : igTDataList<igObject>
    {
        public static igObjectList readObjectsWithoutFields(ExtendedBinaryReader reader, IGZ container, igNameList names = null)
        {
            igObjectList igObjectList = (igObjectList)readWithoutFields(reader, container);
            igObjectList.baseReadFields(reader);
            igObjectList.Items = new List<igObject>();

            long[] offsets = new long[igObjectList.getCount()];
            Buffer.BlockCopy(igObjectList.getRawData(), 0, offsets, 0, offsets.Length * 8);

            for (int i = 0; i < igObjectList.getCount(); i++)
            {
                if (offsets[i] != 0)
                {
                    reader.Position = offsets[i];
                    igObjectList.Items.Add(getObject(reader, igObjectList._container, false));
                    if (names != null)
                        igObjectList.Items[i]._metaName = names.Items[i]._name;
                }
            }

            return igObjectList;
        }

        public void readFieldsOfObjects(ExtendedBinaryReader reader)
        {
            for(int i = 0; i < Items.Count; i++)
            {
                reader.Position = Items[i]._offset + 16;
                Items[i].readFields(reader);
            }
        }

        public override void readFields(ExtendedBinaryReader reader)
        {
            base.readFields(reader);
        }
    }
}
