using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Sg
{
    public class igFxMaterial : igObject
    {
        [igField(48, typeof(igStringMetaField))]
        public string _fxFilename;

        [igField(56, typeof(igObjectMetaField))]
        public igCachedAttrListList _instanceAttrs;

        [igField(64, typeof(igHandleNameMetaField))]
        public Tuple<string, string> _effectHandle;

        //[igField(72, typeof(igObjectMetaField))]
        //public igGraphicsMaterial _graphicsMaterial;

        //[igField(80, typeof(???))]
        //public ??? _procVertexFormat;

        [igField(84, typeof(igIntMetaField))]
        public int _textureCoordCount;
    }
}
