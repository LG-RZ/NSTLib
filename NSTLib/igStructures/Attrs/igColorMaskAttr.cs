namespace NSTLib.igStructures.Attrs
{
    public class igColorMaskAttr : igVisualAttribute
    {
        [igField(8, typeof(igBooleanMetaField))] public bool _red;
        [igField(9, typeof(igBooleanMetaField))] public bool _green;
        [igField(10, typeof(igBooleanMetaField))] public bool _blue;
        [igField(11, typeof(igBooleanMetaField))] public bool _alpha;
    }
}
