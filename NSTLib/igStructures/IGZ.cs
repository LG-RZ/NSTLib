using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures
{
    public class IGZ
    {
        #region Structures

        public struct Header
        {
            public int _magicnumber;
            public int _version;
            public int _checksum; // placeholder name
            public int _platform;
        }

        #endregion

        #region Reading

        public static IGZ Read(string fileName, Stream stream)
        {
            IGZ igz = new IGZ() { _fileName = fileName };
            using (ExtendedBinaryReader reader = new ExtendedBinaryReader(stream))
            {
                #region Parse Header

                igz._header = reader.ReadStruct<Header>();

                // Validate header
                if (igz._header._magicnumber != 0x49475A01)
                    throw new InvalidDataException($"Invalid magic number. Expected: 0x49475A01, Got: {igz._header._magicnumber}");

                if (igz._header._version != 0xA)
                    throw new Exception($"Unsupported version: {igz._header._version}");

                if (igz._header._platform != 0x6)
                    throw new Exception($"Unsupported platform: {igz._header._platform}");

                #endregion

                #region Fixup Sections

                int FixupSectionsCount = reader.ReadInt32();
                reader.BaseStream.Position += 4;
                int FixupSectionsOffset = reader.ReadInt32();
                int FixupSectionsLength = reader.ReadInt32();

                igz.ProcessFixupSections(reader, FixupSectionsCount, FixupSectionsOffset, FixupSectionsLength);

                #endregion

                #region External Data (has some relation to REXT)

                // COULD FAIL ON SOME FILES
                reader.BaseStream.Position = 0x38;
                uint DataOffset = reader.ReadUInt32();
                int DataLength = reader.ReadInt32();
                
                if(DataOffset != 0)
                {
                    reader.BaseStream.Position = DataOffset;
                    igz._data = reader.ReadBytes(DataLength);
                }

                #endregion

                #region Objects

                reader.BaseStream.Position = 0x28;
                reader.RelativePosition = reader.ReadUInt32();

                for (int i = 0; i < igz._fixupSections.Count; i++)
                {
                    switch (igz._fixupSections[i]._magicnumber)
                    {
                        case 0x4D414E4F: // ONAM
                            reader.BaseStream.Position = igz._fixupSections[i]._dataPointer;
                            reader.Position = reader.ReadUInt32();
                            igz._names = (igNameList)igObject.read(reader, igz);
                            break;
                    }
                }

                reader.Position = 0;
                igz._objects = igObjectList.readObjectsWithoutFields(reader, igz, igz._names);
                igz._objects.readFieldsOfObjects(reader);

                #endregion
            }

            return igz;
        }

        private void ProcessFixupSections(ExtendedBinaryReader reader, int Count, int Offset, int Length)
        {
            reader.BaseStream.Position = Offset;

            _fixupSections = new List<igIGZFixupSection>();

            for(int i = 0; i < Count; i++)
            {
                igIGZFixupSection section = new igIGZFixupSection();
                section._magicnumber = reader.ReadUInt32();
                section._count = reader.ReadInt32();
                section._length = reader.ReadInt32();
                section._headerSize = reader.ReadInt32();
                section._dataPointer = reader.BaseStream.Position + (section._headerSize - 0x10);
                _fixupSections.Add(section);
                ProcessFixupSection(reader, section);
            }
        }

        private void ProcessFixupSection(ExtendedBinaryReader reader, igIGZFixupSection section)
        {
            long FixupSectionEndOffset = reader.BaseStream.Position + (section._length - 16);
            reader.BaseStream.Position = section._dataPointer;
            switch (section._magicnumber)
            {
                case 0x50454454: // TDEP
                    _dependencies = new List<Tuple<string, string>>();
                    for (int i = 0; i < section._count; i++)
                    {
                        _dependencies.Add(new Tuple<string, string>(reader.ReadNullTerminatedString(), reader.ReadNullTerminatedString()));
                    }
                    break;
                case 0x52545354: // TSTR
                    _stringList = new List<string>();
                    for (int i = 0; i < section._count; i++)
                    {
                        string str = reader.ReadNullTerminatedString();

                        if ((str.Length % 0x2) == 0)
                            reader.BaseStream.Position++;

                        _stringList.Add(str);
                    }
                    break;
                case 0x54454D54: // TMET
                    _typeNames = new List<string>();
                    for (int i = 0; i < section._count; i++)
                    {
                        string str = reader.ReadNullTerminatedString();

                        if ((str.Length % 0x2) == 0)
                            reader.BaseStream.Position++;

                        _typeNames.Add(str);
                    }
                    break;
                case 0x5A53544D: // MTSZ
                    _metaSizes = new List<uint>();
                    for (int i = 0; i < section._count; i++)
                    {
                        _metaSizes.Add(reader.ReadUInt32());
                    }
                    break;
                case 0x44495845: // EXID
                    _externalList = new List<Tuple<uint, uint>>();
                    for (int i = 0; i < section._count; i++)
                    {
                        _externalList.Add(new Tuple<uint, uint>(reader.ReadUInt32(), reader.ReadUInt32()));
                    }
                    break;
                case 0x4D4E5845: // EXNM
                    _namedExternalList = new List<Tuple<uint, uint>>();
                    for (int i = 0; i < section._count; i++)
                    {
                        _namedExternalList.Add(new Tuple<uint, uint>(reader.ReadUInt32(), reader.ReadUInt32() & ~0x80000000));
                    }
                    break;
            }
            reader.BaseStream.Position = FixupSectionEndOffset;
        }

        #endregion

        #region Methods

        public bool findObject(uint offset, out igObject @object)
        {
            if (_objects != null)
            {
                for (int i = 0; i < _objects.Items.Count; i++)
                {
                    if (_objects.Items[i]._offset.Equals(offset))
                    {
                        @object = _objects.Items[i];
                        return true;
                    }
                }
            }
            @object = null;
            return false;
        }

        #endregion

        #region Fields

        public string _fileName;

        private List<igIGZFixupSection> _fixupSections;

        public List<Tuple<string, string>> _dependencies;
        public List<string> _stringList;
        public List<string> _typeNames;
        public List<uint> _metaSizes;
        public List<Tuple<uint, uint>> _externalList;
        public List<Tuple<uint, uint>> _namedExternalList;

        public igObjectList _objects;
        public igNameList _names;

        public byte[] _data;

        private Header _header;

        #endregion
    }
}
