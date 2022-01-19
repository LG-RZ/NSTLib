using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core
{
    public class igTDataList<T> : igDataList
    {
        #region Fields

        public List<T> Items;

        #endregion

        #region Methods

        public override void readFields(ExtendedBinaryReader reader)
        {
            base.readFields(reader);

            Items = new List<T>();

            Type genericType = typeof(T);

            if(typeof(igMetaField).IsAssignableFrom(genericType))
            {
                using (ExtendedBinaryReader dataReader = new ExtendedBinaryReader(new MemoryStream(getRawData())))
                {
                    for (int i = 0; i < getCount(); i++)
                    {
                        igMetaField field = (igMetaField)Activator.CreateInstance(genericType);
                        field._container = _container;

                        long Position = dataReader.BaseStream.Position + field.computeSize();
                        field.readFields(dataReader);
                        Items.Add((T)(object)field);

                        dataReader.BaseStream.Position = Position;
                    }
                }
            }
            else if(typeof(igObject).IsAssignableFrom(genericType))
            {
                long[] offsets = new long[getCount()];
                Buffer.BlockCopy(getRawData(), 0, offsets, 0, offsets.Length * 8);

                for (int i = 0; i < getCount(); i++)
                {
                    if (offsets[i] != 0)
                    {
                        if ((offsets[i] & 0x8000000) != 0)
                        {
                            bool isLastPool = _container._memoryPools.Count <= (_memoryPoolIndex + 1);
                            long PositionRelative = reader.RelativePosition;
                            uint Offset = (uint)(offsets[i] & 0x7ffffff);

                            reader.RelativePosition = _container._memoryPools[isLastPool ? _memoryPoolIndex : _memoryPoolIndex + 1].Item1;
                            reader.Position = Offset;

                            var @object = getObject(reader, _container, isLastPool ? _memoryPoolIndex : _memoryPoolIndex + 1);
                            if (@object is T)
                                Items.Add((T)(object)@object);

                            reader.RelativePosition = PositionRelative;
                        }
                        else
                        {
                            reader.Position = offsets[i];
                            var @object = getObject(reader, _container, _memoryPoolIndex);
                            if (@object is T)
                                Items.Add((T)(object)@object);
                        }
                    }
                }
            }
            else if (genericType.IsValueType)
            {
                genericType = genericType.IsEnum ? Enum.GetUnderlyingType(genericType) : genericType;

                using (ExtendedBinaryReader dataReader = new ExtendedBinaryReader(new MemoryStream(getRawData())))
                    for (int i = 0; i < getCount(); i++)
                    {
                        Items.Add((T)dataReader.ReadStruct(genericType));
                    }
            }
        }

        #endregion
    }
}
