namespace NSTLib.igStructures.Attrs
{
    public class igTextureMatrixAttr : igVisualAttribute
    {
        [igField(16, typeof(igMatrix44fMetaField))]
        public igMatrix44f _m;
        [igField(80, typeof(igIntMetaField))]
        public int _unitID;
        [igField(84, typeof(igShortMetaField))]
        public short _outputElementCount;
        [igField(88, typeof(igBooleanMetaField))]
        public bool _isProjective;
        [igField(92, typeof(igStringMetaField))]
        public string _textureMatrixName;
    }
}
