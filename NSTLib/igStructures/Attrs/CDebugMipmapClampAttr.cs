namespace NSTLib.igStructures.Attrs
{
    public class CDebugMipmapClampAttr : igVisualAttribute
    {
        [igField(16, typeof(igVec4fMetaField))]
        public igVec4f _clamp;
        [igField(32, typeof(igVec4fMetaField))]
        public igVec4f _clamp2;

        // _debug_mipmap_clampName
        // _debug_mipmap_clampHandle
        // _debug_mipmap_clampName2
        // _debug_mipmap_clampHandle2
    }
}
