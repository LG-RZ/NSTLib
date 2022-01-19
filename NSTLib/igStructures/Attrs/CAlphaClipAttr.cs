namespace NSTLib.igStructures.Attrs
{
    public class CAlphaClipAttr : igVisualAttribute
    {
        [igField(8, typeof(igBooleanMetaField))]
        public bool _enable;
        [igField(12, typeof(igFloatMetaField))]
        public float _threshold;
    }
}
