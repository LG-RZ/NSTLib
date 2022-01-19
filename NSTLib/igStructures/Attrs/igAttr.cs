namespace NSTLib.igStructures.Attrs
{
    public class igAttr : igObject
    {
        [igField(0, typeof(igShortMetaField))]
        public short _cachedUnitID;
        [igField(2, typeof(igShortMetaField))]
        public short _cachedAttrIndex;
        [igField(4, typeof(igBooleanMetaField))]
        public bool _readOnlyCopy;
    }
}
