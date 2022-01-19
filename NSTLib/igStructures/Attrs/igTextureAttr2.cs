namespace NSTLib.igStructures.Attrs
{
    public class igTextureAttr2 : igVisualAttribute
    {
        [igField(16, typeof(igIntMetaField))]
        public IG_GFX_TEXTURE_FILTER _magFilter;
        [igField(20, typeof(igIntMetaField))]
        public IG_GFX_TEXTURE_FILTER _minFilter;
        [igField(24, typeof(igIntMetaField))]
        public IG_GFX_TEXTURE_WRAP _wrapS;
        [igField(28, typeof(igIntMetaField))]
        public IG_GFX_TEXTURE_WRAP _wrapT;
        [igField(32, typeof(igIntMetaField))]
        public IG_GFX_MULTISAMPLE_TYPE _msaaType;
        [igField(64, typeof(igHandleNameMetaField))]
        public igHandle _imageHandle;
        [igField(72, typeof(igIntMetaField))]
        public IG_GFX_TEXTURE_WRAP _wrapR;
    }
}
