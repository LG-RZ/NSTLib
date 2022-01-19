namespace NSTLib.igStructures.Attrs
{
    public class igAlphaFunctionAttr : igVisualAttribute
    {
        [igField(8, typeof(igIntMetaField))]
        public IG_GFX_ALPHA_FUNCTION _func;
        [igField(12, typeof(igFloatMetaField))]
        public float _refValue;
    }
}
