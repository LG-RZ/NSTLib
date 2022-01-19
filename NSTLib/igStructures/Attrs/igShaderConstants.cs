namespace NSTLib.igStructures.Attrs
{
    public class igShaderConstantBool : igShaderConstantData
    {
        [igField(16, typeof(igBooleanMetaField))]
        public bool _data;
    }

    public class igShaderConstantInt : igShaderConstantData
    {
        [igField(16, typeof(igIntMetaField))]
        public int _data;
    }
    public class igShaderConstantFloat : igShaderConstantData
    {
        [igField(16, typeof(igFloatMetaField))]
        public float _data;
    }

    public class igShaderConstantVector : igShaderConstantData
    {
        [igField(16, typeof(igVec4fMetaField))]
        public igVec4f _data;
    }
}
