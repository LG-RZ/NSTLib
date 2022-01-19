namespace NSTLib.igStructures.Attrs
{
    public class igCullFaceAttr : igVisualAttribute
    {
        [igField(8, typeof(igBooleanMetaField))]
        public bool _enabled;
        [igField(12, typeof(igIntMetaField))]
        public IG_GFX_CULL_FACE_MODE _mode;
    }
}
