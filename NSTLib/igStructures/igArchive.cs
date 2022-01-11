using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SevenZip.Compression.LZMA;
using Decoder = SevenZip.Compression.LZMA.Decoder;
using Encoder = SevenZip.Compression.LZMA.Encoder;

namespace NSTLib.igStructures
{
    // We can divide the IGA to about 4 parts:
    // 1 - Header.
    // 2 - Table of Contents (Divided to 3 parts: File IDs (Hashes), File Info, File Blocks (Compression stuff)).
    // 3 - File Data.
    // 4 - Name Table.

    public class igArchive
    {
        #region Structures

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ArchiveHeader
        {
            public int _magicnumber;
            public int _version;
            public int _tocSize;
            public int _numFiles;

            public int _sectorSize;

            public uint _hashSearchDivider;
            public uint _hashSearchSlop;

            public int _numLargeFileBlocks;
            public int _numMediumFileBlocks;
            public int _numSmallFileBlocks;

            public long _nameTableOffset;
            public int _nameTableSize;
            public int _flags;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileEntry
        {
            public uint _id;
            public FileInfo _info;
            public NameEntry _nameEntry;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FileInfo
        {
            public int _offset;
            public int _entryID;
            public int _size;
            public int24 _commandIndex;
            public byte _compressionFlag;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NameEntry
        {
            public string _buildPath;
            public string _name;
        }

        #endregion

        #region Constructor

        public igArchive()
        {
            _header = new ArchiveHeader()
            {
                _magicnumber        = 0x1A414749,
                _version            = 0xB,
                _sectorSize         = 0x800,
                _hashSearchDivider  = 0xFFFFFFFF,
                _hashSearchSlop     = 0x0,
                _flags              = 0x1
            };
        }

        public igArchive(Stream _stream) : this()
        {
            this._stream = _stream;
        }

        #endregion

        #region Reading

        public static void Initialize()
        {
            _init = true;

            if(!string.IsNullOrWhiteSpace(NSTSettings.GamePath))
            {
                availableNameEntries = new Dictionary<string, NameEntry[]>();

                foreach(string path in Directory.GetFiles(NSTSettings.GamePath + "\\archives", "*.pak"))
                {
                    using(ExtendedBinaryReader reader = new ExtendedBinaryReader(File.OpenRead(path)))
                    {
                        // Read header
                        var header = reader.ReadStruct<ArchiveHeader>();

                        // Go to the Name Table
                        reader.BaseStream.Position = header._nameTableOffset;

                        // Read buffer and name offsets
                        int[] nameOffsets = new int[header._numFiles];
                        byte[] buffer = reader.ReadBytes(header._nameTableSize);
                        Buffer.BlockCopy(buffer, 0, nameOffsets, 0, 4 * header._numFiles);

                        using(ExtendedBinaryReader nameTableReader = new ExtendedBinaryReader(new MemoryStream(buffer)))
                        {
                            availableNameEntries[path] = new NameEntry[header._numFiles];
                            for(int i = 0; i < availableNameEntries[path].Length; i++)
                            {
                                nameTableReader.BaseStream.Position = nameOffsets[i];
                                availableNameEntries[path][i]._buildPath = nameTableReader.ReadNullTerminatedString();
                                availableNameEntries[path][i]._name = nameTableReader.ReadNullTerminatedString();
                            }

                            nameTableReader.BaseStream.Flush();
                        }
                        reader.BaseStream.Flush();
                    }
                }
            }
        }

        public static igArchive Read(Stream stream)
        {
            igArchive archive = new igArchive(stream);

            using(ExtendedBinaryReader reader = new ExtendedBinaryReader(stream, Encoding.UTF8, true))
            {
                #region Parse Header

                archive._header = reader.ReadStruct<ArchiveHeader>();

                #endregion

                #region Parse TOC

                var fileCount = archive._header._numFiles;

                uint[] hashes;
                FileInfo[] info;
                using (ExtendedBinaryReader tocReader = new ExtendedBinaryReader(new MemoryStream(reader.ReadBytes(archive._header._tocSize)))) // TOC Buffer
                {
                    // Read hashes and File Info
                    hashes = tocReader.ReadArray<uint>(fileCount);
                    info = tocReader.ReadArrayUnsafe<FileInfo>(fileCount);

                    // Read file blocks
                    archive._mediumFileBlocks = tocReader.ReadArray<ushort>(archive._header._numMediumFileBlocks);
                    archive._smallFileBlocks = tocReader.ReadArray<byte>(archive._header._numSmallFileBlocks);
                }

                #endregion

                #region Parse Name Table

                reader.BaseStream.Position = archive._header._nameTableOffset;

                int[] nameOffsets = new int[fileCount];
                byte[] nameTableBuffer = reader.ReadBytes(archive._header._nameTableSize);
                Buffer.BlockCopy(nameTableBuffer, 0, nameOffsets, 0, fileCount * 4);
                NameEntry[] nameEntries = new NameEntry[fileCount];

                using(ExtendedBinaryReader nameTableReader = new ExtendedBinaryReader(new MemoryStream(nameTableBuffer)))
                {
                    for(int i = 0; i < fileCount; i++)
                    {
                        nameTableReader.BaseStream.Position = nameOffsets[i];
                        nameEntries[i]._buildPath = nameTableReader.ReadNullTerminatedString();
                        nameEntries[i]._name = nameTableReader.ReadNullTerminatedString();
                    }
                }

                #endregion

                #region Finalize

                archive._entries = new FileEntry[fileCount];

                for (int i = 0; i < fileCount; i++)
                {
                    archive._entries[i]._id = hashes[i];
                    archive._entries[i]._info = info[i];
                    archive._entries[i]._info._entryID >>= 8;
                    archive._entries[i]._nameEntry = nameEntries[i];
                }

                #endregion
            }

            return archive;
        }

        public Stream GetFileData(string name) => FindFile(name, out var file) ? GetFileData(file.Value) : null;
        public Stream GetFileData(uint hash) => FindFile(hash, out var file) ? GetFileData(file.Value) : null;
        public Stream GetFileData(FileEntry entry)
        {
            MemoryStream stream = new MemoryStream();
            WriteToStream(entry, stream);
            return stream;
        }

        public void WriteToStream(FileEntry entry, Stream stream)
        {
            switch(entry._info._compressionFlag)
            {
                case 0xFF: // NONE
                    using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true))
                    {
                        using (ExtendedBinaryReader reader = new ExtendedBinaryReader(_stream, Encoding.UTF8, true))
                        {
                            reader.BaseStream.Position = entry._info._offset;
                            writer.Write(reader.ReadBytes(entry._info._size));
                        }
                    }
                    break;
                case 0x20: // LZMA
                    using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true))
                    {
                        using (ExtendedBinaryReader reader = new ExtendedBinaryReader(_stream, Encoding.UTF8, true))
                        {
                            var CommandsRequired = ((entry._info._size + 0x7FFF) >> 0xF);

                            for(int i = 0; i < CommandsRequired; i++)
                            {
                                DecompressLZMABlock(entry._info._offset, entry._info._size, i, entry._info._commandIndex, writer, reader);
                            }
                        }
                    }
                    break;
                default:
                    throw new Exception("Unsupported compression type");
            }
        }

        #region Decompression

        public void DecompressLZMABlock(int offset, int size, int index, int commandIndex, BinaryWriter writer, BinaryReader reader)
        {
            int BlockMask = size > 0x3F800 ? 0x8000 : 0x80;
            int BlockCommand = size > 0x3F800 ? _mediumFileBlocks[commandIndex + index] : _smallFileBlocks[commandIndex + index];

            reader.BaseStream.Position = offset + (0x800 * (BlockCommand & ~BlockMask));

            if((BlockCommand & BlockMask) == 0)
            {
                writer.Write(reader.ReadBytes(0x8000));
            }
            else
            {
                ushort CompressedSize = reader.ReadUInt16();
                byte[] DecoderProperties = reader.ReadBytes(5);

                Decoder decoder = new Decoder();
                decoder.SetDecoderProperties(DecoderProperties);
                decoder.Code(reader.BaseStream, writer.BaseStream, CompressedSize, System.Math.Min(0x8000, size - (0x8000 * index)), null);
            }
        }

        #endregion

        #endregion

        #region Methods

        public bool FindFile(string name, out FileEntry? file)
        {
            for (int i = 0; i < _entries.Length; i++)
            {
                if (_entries[i]._nameEntry._name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    file = _entries[i];
                    return true;
                }
            }
            file = null;
            return false;
        }

        public bool FindFile(uint Hash, out FileEntry? file)
        {
            for (int i = 0; i < _entries.Length; i++)
            {
                if (_entries[i]._id == Hash)
                {
                    file = _entries[i];
                    return true;
                }
            }
            file = null;
            return false;
        }

        #endregion

        #region Static Members

        public static Dictionary<string, NameEntry[]> availableNameEntries;
        private static bool _init;

        #endregion

        #region Fields

        private ArchiveHeader _header;

        public readonly Stream _stream;

        public ushort[] _mediumFileBlocks;
        public byte[] _smallFileBlocks;
        public FileEntry[] _entries;

        #endregion
    }
}
