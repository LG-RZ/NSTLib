namespace NSTLib.igStructures.Attrs
{
    public class igShaderConstantData : igObject
    {
        [igField(0, typeof(igStringMetaField))]
        public string _handle;
        [igField(4, typeof(igBooleanMetaField))]
        public bool _enabled;
    }
}
