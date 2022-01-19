namespace NSTLib.igStructures.Attrs
{
    public class igTextureBindAttr2 : igVisualAttribute
    {
        [igField(8, typeof(igObjectMetaField))]
        public igTextureAttr2 _texture;
        [igField(12, typeof(igUnsignedIntMetaField))]
        public uint _placeholderTextureId;
        [igField(16, typeof(igIntMetaField))]
        public int _unitID;
    }
}
