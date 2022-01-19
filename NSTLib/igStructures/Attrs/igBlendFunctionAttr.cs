namespace NSTLib.igStructures.Attrs
{
    public class igBlendFunctionAttr : igVisualAttribute
    {
        [igField(8, typeof(igIntMetaField))]
        public IG_GFX_BLENDING_FUNCTION _src;
        [igField(12, typeof(igIntMetaField))]
        public IG_GFX_BLENDING_FUNCTION _dest;
        [igField(16, typeof(igIntMetaField))]
        public IG_GFX_BLENDING_EQUATION _eq;

        /*
        _unused             : unknown
        _blendConstant      : unknown
        _blendStage         : unknown
        _blendA             : unknown
        _blendB             : unknown
        _blendC             : unknown
        _blendD             : unknown
        */
    }
}
