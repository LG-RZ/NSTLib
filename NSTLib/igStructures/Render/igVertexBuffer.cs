namespace NSTLib.igStructures.Render
{
    public class igVertexBuffer : igObject
    {
        [igField(0, typeof(igUnsignedIntMetaField))]
        public uint _vertexCount;
        [igField(8, typeof(igIntArrayMetaField))]
        public int[] _vertexCountArray;
        [igField(24, typeof(igDataStreamMetaField))]
        public byte[] _data;
        [igField(40, typeof(igObjectMetaField))]
        public igVertexFormat _format;
        [igField(48, typeof(igStructMetaField<IG_GFX_DRAW>))]
        public IG_GFX_DRAW _primitiveType;
    }
}