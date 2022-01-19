namespace NSTLib.igStructures.Attrs
{
    public class igDecalAttr : igVisualAttribute
    {
        [igField(8, typeof(igFloatMetaField))]
        public float _decalOffset;
        [igField(12, typeof(igFloatMetaField))]
        public float _decalSlope;
    }
}
