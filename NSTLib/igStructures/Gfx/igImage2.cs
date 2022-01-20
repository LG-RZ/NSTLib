using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Gfx
{
    public class igImage2 : igObject
    {
        [igField(0, typeof(igStringMetaField))]
        public string _imageName;
        [igField(8, typeof(igShortMetaField))]
        public short _width;
        [igField(10, typeof(igShortMetaField))]
        public short _height;
        [igField(12, typeof(igShortMetaField))]
        public short _depth;
        [igField(14, typeof(igShortMetaField))]
        public short _levelCount;
        [igField(16, typeof(igIntMetaField))]
        public int _imageCount;
        [igField(24, typeof(igEXIDMetaField))]
        public metaimages _format;
        [igField(40, typeof(igDataStreamMetaField))]
        public byte[] _data;

        #region DDS Data

        private static Dictionary<metaimages, bool> compressed = new Dictionary<metaimages, bool>()
        {
            [metaimages.dxt1_dx11]      = true,
            [metaimages.dxt3_dx11]      = true,
            [metaimages.dxt5_dx11]      = true,
            [metaimages.bc5_dx11]       = true,
            [metaimages.r8g8b8a8_dx11]  = false,
        };

        private static Dictionary<metaimages, uint> fourCC = new Dictionary<metaimages, uint>()
        {
            [metaimages.dxt1_dx11]      = 0x31545844,
            [metaimages.dxt3_dx11]      = 0x33545844,
            [metaimages.dxt5_dx11]      = 0x35545844,
            [metaimages.bc5_dx11]       = 0x32495441,
        };

        private static Dictionary<metaimages, int> rgbBitCount = new Dictionary<metaimages, int>()
        {
            [metaimages.r8g8b8a8_dx11] = 0x20,
        };

        private static Dictionary<metaimages, uint> rBitMask = new Dictionary<metaimages, uint>()
        {
            [metaimages.r8g8b8a8_dx11] = 0xFF,
        };

        private static Dictionary<metaimages, uint> gBitMask = new Dictionary<metaimages, uint>()
        {
            [metaimages.r8g8b8a8_dx11] = 0xFF00,
        };

        private static Dictionary<metaimages, uint> bBitMask = new Dictionary<metaimages, uint>()
        {
            [metaimages.r8g8b8a8_dx11] = 0xFF0000,
        };
        private static Dictionary<metaimages, uint> aBitMask = new Dictionary<metaimages, uint>()
        {
            [metaimages.r8g8b8a8_dx11] = 0xFF000000,
        };

        #endregion

        public void getDDSData(Stream stream)
        {
            using(BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write(0x20534444); // Magic

                #region DDS_HEADER

                bool hasMipMaps = (_levelCount - 1 > 0);
                bool hasDepth = (_depth - 1 > 0);

                writer.Write(0x7C); // Structure Size
                writer.Write(0x1 | 0x2 | 0x4 | 0x1000 | (hasMipMaps ? 0x20000 : 0x0) | 0x80000 | (hasDepth ? 0x800000 : 0x0)); // Flags
                writer.Write((int)_height); // Height
                writer.Write((int)_width); // Width
                writer.Write(_data.Length); // Pitch or Linear Size
                writer.Write((int)_depth); // Depth
                writer.Write((int)_levelCount); // Mip Map Count
                writer.Write(new byte[44]); // Reserved

                #region DDS_PIXELFORMAT

                bool isCompressed = compressed[_format];

                writer.Write(0x20); // Structure Size
                writer.Write(isCompressed ? 0x4 : (0x1 | 0x40)); // Flags
                writer.Write(isCompressed ? fourCC[_format] : 0); // Four CC
                writer.Write(isCompressed ? 0 : rgbBitCount[_format]); // RGB Bit Count
                writer.Write(isCompressed ? 0 : rBitMask[_format]); // Red Bit Mask
                writer.Write(isCompressed ? 0 : gBitMask[_format]); // Green Bit Mask
                writer.Write(isCompressed ? 0 : bBitMask[_format]); // Blue Bit Mask
                writer.Write(isCompressed ? 0 : aBitMask[_format]); // Alpha Bit Mask

                #endregion

                writer.Write((hasMipMaps || hasDepth ? 0x8 : 0x0) | (hasMipMaps ? 0x400000 : 0x0) | 0x1000); // Caps
                writer.Write(0); // Caps2
                writer.Write(0); // Caps3
                writer.Write(0); // Caps4
                writer.Write(0); // Reserved

                #endregion

                writer.Write(_data); // Data
            }
        }
    }
}
