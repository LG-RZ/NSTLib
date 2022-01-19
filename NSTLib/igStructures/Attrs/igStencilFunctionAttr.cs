namespace NSTLib.igStructures.Attrs
{
    public class igStencilFunctionAttr : igVisualAttribute
    {
        [igField(8, typeof(igIntMetaField))]
        public int _refVal;
        [igField(12, typeof(igStructMetaField<IG_GFX_STENCIL_FUNCTION>))]
        public IG_GFX_STENCIL_FUNCTION _func;
        [igField(16, typeof(igIntMetaField))]
        public int _writeMask;
        [igField(20, typeof(igIntMetaField))]
        public int _readMask;
        [igField(24, typeof(igStructMetaField<IG_GFX_STENCIL_OPERATION>))]
        public IG_GFX_STENCIL_OPERATION _stenFailOp;
        [igField(28, typeof(igStructMetaField<IG_GFX_STENCIL_OPERATION>))]
        public IG_GFX_STENCIL_OPERATION _stenPassZPassOp;
        [igField(32, typeof(igStructMetaField<IG_GFX_STENCIL_OPERATION>))]
        public IG_GFX_STENCIL_OPERATION _stenPassZFailOp;
    }
}
