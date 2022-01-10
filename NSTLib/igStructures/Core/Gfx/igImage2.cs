using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core.Gfx
{
    public class igImage2 : igObject
    {
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
    }
}
