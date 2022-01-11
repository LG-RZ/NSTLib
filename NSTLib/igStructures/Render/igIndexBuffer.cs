namespace NSTLib.igStructures.Render
{
    public class igIndexBuffer : igObject
    {
        [igField(0, typeof(igUnsignedIntMetaField))]
        public uint _indexCount;
        [igField(8, typeof(igIntArrayMetaField))]
        public int[] _indexCountArray;
        [igField(24, typeof(igDataStreamMetaField))]
        public byte[] _data;
        [igField(48, typeof(igIntMetaField))]
        public IG_GFX_DRAW _primitiveType;
    }
}