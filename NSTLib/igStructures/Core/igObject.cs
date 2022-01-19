using NSTLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core
{
    public class igObject
    {
        #region Fields

        public IGZ _container;

        public string _metaName;
        public string _type;

        public uint _offset;
        public int _typeIndex;
        public int _memoryPoolIndex;

        #endregion

        #region Methods

        #region Initialize

        private static bool _init;
        private static Dictionary<string, Type> types;

        private static void initialize()
        {
            types = new Dictionary<string, Type>();

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(igObject).IsAssignableFrom(p)))
                types[type.Name] = type;

            _init = true;
        }

        #endregion

        public static igObject getObject(ExtendedBinaryReader reader, IGZ container, int memoryPoolIndex, bool readFields = true)
        {
            if (container.findObject((uint)reader.Position, memoryPoolIndex, out igObject @object))
                return @object;
            else
                return readFields ? read(reader, container, memoryPoolIndex) : readWithoutFields(reader, container, memoryPoolIndex);
        }

        public static igObject read(ExtendedBinaryReader reader, IGZ container, int memoryPoolIndex, string name = null)
        {
            igObject @object = readWithoutFields(reader, container, memoryPoolIndex, name);
            @object.readFields(reader);
            return @object;
        }

        public static igObject readWithoutFields(ExtendedBinaryReader reader, IGZ container, int memoryPoolIndex, string name = null)
        {
            if (!_init)
                initialize();

            int TypeIndex = (int)reader.ReadInt64();
            int Unknown = (int)reader.ReadInt64();

            igObject @object = new igObject();
            if(types.TryGetValue(container._typeNames[TypeIndex], out Type value))
            {
                @object = (igObject)Activator.CreateInstance(value);
            }

            @object._container = container;

            @object._metaName = name;
            @object._type = container._typeNames[TypeIndex];

            @object._offset = (uint)reader.Position - 16;
            @object._typeIndex = TypeIndex;
            @object._memoryPoolIndex = memoryPoolIndex;

            return @object;
        }

        public virtual void readFields(ExtendedBinaryReader reader)
        {
            baseReadFields(reader);
        }

        public void baseReadFields(ExtendedBinaryReader reader)
        {
            long Position = reader.Position;

            foreach (var field in GetType().GetAllFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                foreach (var attribute in field.GetCustomAttributes(false))
                {
                    if (attribute is igFieldAttribute fieldAttribute)
                    {
                        igMetaField metaField = (igMetaField)Activator.CreateInstance(fieldAttribute.MetaFieldType);
                        metaField._memoryPoolIndex = _memoryPoolIndex;
                        metaField._container = _container;
                        metaField._offset = (uint)(Position + fieldAttribute.FieldOffset);

                        reader.Position = metaField._offset;
                        metaField.readFields(reader);
                        reader.Position = metaField._offset;
                        object val = metaField.readField(reader);
                        field.SetValue(this, val);

                        break;
                    }
                }
            }
        }

        #endregion
    }
}
