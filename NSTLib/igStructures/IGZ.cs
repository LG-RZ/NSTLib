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

                igz._memoryPools = new List<Tuple<uint, int>>();

                reader.BaseStream.Position = 0x20;

                // BAD WAY TO GET POOLS BUT WHAT ELSE CAN I DO
                while (reader.ReadUInt32() != 0x8)
                {
                    reader.BaseStream.Position += 0x4;
                    uint Offset = reader.ReadUInt32();
                    if (Offset == 0)
                        break;
                    igz._memoryPools.Add(new Tuple<uint, int>(Offset, reader.ReadInt32()));
                }

                #region External Data

                reader.BaseStream.Position += 0x4;
                uint DataOffset = reader.ReadUInt32();
                
                if(DataOffset != 0)
                {
                    igz._dataOffset = DataOffset;
                }

                #endregion

                #region Objects

                if(igz._memoryPools.Count > 0)
                {
                    reader.RelativePosition = igz._memoryPools[0].Item1;

                    for (int i = 0; i < igz._fixupSections.Count; i++)
                    {
                        switch (igz._fixupSections[i]._magicnumber)
                        {
                            case 0x4D414E4F: // ONAM
                                reader.BaseStream.Position = igz._fixupSections[i]._dataPointer;
                                reader.Position = reader.ReadUInt32();
                                igz._names = (igNameList)igObject.read(reader, igz, 0);
                                break;
                        }
                    }

                    reader.Position = 0;
                    igz._objects = igObjectList.readObjectsWithoutFields(reader, igz, 0, igz._names);
                    igz._objects.readFieldsOfObjects(reader);
                }

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

        private unsafe void ProcessFixupSection(ExtendedBinaryReader reader, igIGZFixupSection section)
        {
            long FixupSectionEndOffset = reader.BaseStream.Position + (section._length - 0x10);
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
                    _stringsList = new List<string>();
                    for (int i = 0; i < section._count; i++)
                    {
                        string str = reader.ReadNullTerminatedString();

                        if ((str.Length % 0x2) == 0)
                            reader.BaseStream.Position++;

                        _stringsList.Add(str);
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
                case 0x42545652: // RVTB
                    _typeOffsets = new List<uint>();
                    uint[] data;

                    fixed (byte* buffer = reader.ReadBytes(section._length - section._headerSize))
                        data = ReadCompressedInts(this, section._count, buffer);

                    for(int i = 0; i < data.Length; i++)
                    {
                        reader.BaseStream.Position = 0x28 + ((data[i] >> 0x1b) * 0x10); // (int >> 0x1b) == objectLists Index
                        _typeOffsets.Add(reader.ReadUInt32() + (data[i] & 0x7ffffff));
                    }

                    break;
                case 0x54545352: // RSTT
                    _stringFieldOffsets = new List<uint>();

                    fixed (byte* buffer = reader.ReadBytes(section._length - section._headerSize))
                        data = ReadCompressedInts(this, section._count, buffer);

                    for (int i = 0; i < data.Length; i++)
                    {
                        reader.BaseStream.Position = 0x28 + ((data[i] >> 0x1b) * 0x10);
                        _stringFieldOffsets.Add(reader.ReadUInt32() + (data[i] & 0x7ffffff));
                    }

                    break;
                case 0x53464F52: // ROFS
                    _pointerFieldOffsets = new List<uint>();

                    fixed (byte* buffer = reader.ReadBytes(section._length - section._headerSize))
                        data = ReadCompressedInts(this, section._count, buffer);

                    for (int i = 0; i < data.Length; i++)
                    {
                        reader.BaseStream.Position = 0x28 + (((data[i] & 0xfbffffff) >> 0x1b) * 0x10);
                        uint Value1 = ((data[i] & 0xfbffffff) & 0x7ffffff) + reader.ReadUInt32();
                        _pointerFieldOffsets.Add(Value1);
                    }

                    break;
                case 0x44495052: // RPID
                    _externalFieldOffsets = new List<uint>();

                    fixed (byte* buffer = reader.ReadBytes(section._length - section._headerSize))
                        data = ReadCompressedInts(this, section._count, buffer);

                    for (int i = 0; i < data.Length; i++)
                    {
                        reader.BaseStream.Position = 0x28 + ((data[i] >> 0x1b) * 0x10); // (int >> 0x1b) == objectLists Index
                        _externalFieldOffsets.Add(reader.ReadUInt32() + (data[i] & 0x7ffffff)); // NOT SURE ABOUT THIS
                    }

                    break;
                case 0x54584552: // REXT
                    _handleFieldOffsets = new List<uint>();

                    fixed (byte* buffer = reader.ReadBytes(section._length - section._headerSize))
                        data = ReadCompressedInts(this, section._count, buffer);

                    for (int i = 0; i < data.Length; i++)
                    {
                        reader.BaseStream.Position = 0x28 + ((data[i] >> 0x1b) * 0x10); // (int >> 0x1b) == objectLists Index
                        _handleFieldOffsets.Add(reader.ReadUInt32() + (data[i] & 0x7ffffff)); // NOT SURE ABOUT THIS
                    }

                    break;
                case 0x444E4852: // RHND
                    _namedExternalFieldOffsets = new List<uint>();

                    fixed (byte* buffer = reader.ReadBytes(section._length - section._headerSize))
                        data = ReadCompressedInts(this, section._count, buffer);

                    for (int i = 0; i < data.Length; i++)
                    {
                        reader.BaseStream.Position = 0x28 + ((data[i] >> 0x1b) * 0x10); // (int >> 0x1b) == objectLists Index
                        _namedExternalFieldOffsets.Add(reader.ReadUInt32() + (data[i] & 0x7ffffff));
                    }

                    break;
                case 0x58454E52: // RNEX
                    _memPoolIndexOffsets = new List<uint>();

                    fixed (byte* buffer = reader.ReadBytes(section._length - section._headerSize))
                        data = ReadCompressedInts(this, section._count, buffer);

                    for (int i = 0; i < data.Length; i++)
                    {
                        reader.BaseStream.Position = 0x28 + ((data[i] >> 0x1b) * 0x10); // (int >> 0x1b) == objectLists Index
                        _memPoolIndexOffsets.Add(reader.ReadUInt32() + (data[i] & 0x7ffffff)); // NOT SURE ABOUT THIS
                    }

                    break;
            }
            reader.BaseStream.Position = FixupSectionEndOffset;
        }

        // Thanks to NefariousTechSupport for reverse engineering this function.
        private static unsafe uint[] ReadCompressedInts(IGZ igz, int Count, byte* Data)
        {
            uint[] Items = new uint[Count];

            bool shiftMoveOrMask = false;
            uint previousInt = 0;

            for (int i = 0; i < Count; i++)
            {
                uint currentByte;

                if (!shiftMoveOrMask)
                {
                    currentByte = (uint)*Data & 0xf;
                    shiftMoveOrMask = true;
                }
                else
                {
                    currentByte = (uint)(*Data >> 4);
                    Data++;
                    shiftMoveOrMask = false;
                }
                byte shiftAmount = 3;
                uint unpackedInt = currentByte & 7;
                while ((currentByte & 8) != 0)
                {
                    if (!shiftMoveOrMask)
                    {
                        currentByte = (uint)*Data & 0xf;
                        shiftMoveOrMask = true;
                    }
                    else
                    {
                        currentByte = (uint)(*Data >> 4);
                        Data++;
                        shiftMoveOrMask = false;
                    }
                    unpackedInt = unpackedInt | (currentByte & 7) << (byte)(shiftAmount & 0x1f);
                    shiftAmount += 3;
                }

                previousInt = (uint)(previousInt + (unpackedInt * 4) + (igz._header._version < 9 ? 4 : 0));
                Items[i] = previousInt;
            }

            return Items;
        }

        #endregion

        #region Writing

        public void Write(Stream stream)
        {
            throw new NotImplementedException();
        }

        // Thanks to NefariousTechSupport for reverse engineering this function which will make it possible to write igzs.
        private static byte[] CompressInts(int count, uint[] offsets, uint version)
        {
            List<byte> compressedData = new List<byte>();
            uint previousInt = 0x00;
            bool shiftMoveOrMask = false;
            byte currentByte = 0x00;
            int shiftAmount = 0x00;

            for (int i = 0; i < count; i++)
            {
                bool firstPass = true;
                uint deltaInt = (uint)((offsets[i] - previousInt) / 4 - (version < 0x09 ? 1 : 0));
                previousInt = offsets[i];
                while (true)
                {
                    byte delta = (byte)((deltaInt >> shiftAmount) & 0b0111);
                    uint remaining = (uint)((deltaInt >> shiftAmount) & ~0b0111);
                    if (remaining > 0 || delta > 0 || firstPass)
                    {
                        if (remaining != 0)
                        {
                            delta |= 0x08;
                        }
                        shiftAmount += 3;
                        if (shiftMoveOrMask)
                        {
                            currentByte |= (byte)(delta << 4);
                            compressedData.Add(currentByte);
                            currentByte = 0x00;
                        }
                        else
                        {
                            currentByte |= delta;
                        }
                        firstPass = false;
                        shiftMoveOrMask = !shiftMoveOrMask;
                    }
                    else
                    {
                        shiftAmount = 0;
                        previousInt = offsets[i];
                        break;
                    }
                }
            }
            if (shiftMoveOrMask)
            {
                compressedData.Add(currentByte);
            }

            return compressedData.ToArray();
        }

        #endregion

        #region Methods

        public bool findObject(uint offset, int memoryPoolIndex, out igObject @object)
        {
            if (_objects != null)
            {
                for (int i = 0; i < _objects.Items.Count; i++)
                {
                    if (_objects.Items[i]._memoryPoolIndex.Equals(memoryPoolIndex) &&_objects.Items[i]._offset.Equals(offset))
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
        public List<string> _stringsList;
        public List<string> _typeNames;
        public List<uint> _metaSizes;
        public List<Tuple<uint, uint>> _externalList;
        public List<Tuple<uint, uint>> _namedExternalList;
        public List<uint> _typeOffsets;
        public List<uint> _stringFieldOffsets;
        public List<uint> _pointerFieldOffsets;
        public List<uint> _externalFieldOffsets;
        public List<uint> _handleFieldOffsets;
        public List<uint> _namedExternalFieldOffsets;
        public List<uint> _memPoolIndexOffsets;
        public List<Tuple<uint, int>> _memoryPools;
        public uint _dataOffset;

        public igObjectList _objects;
        public igNameList _names;

        public byte[] _data;

        private Header _header;

        #endregion
    }
}
