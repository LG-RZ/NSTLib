namespace NSTLib.igStructures.Render
{
    public class igVertexFormat : igObject
    {
        [igField(0, typeof(igIntMetaField))]
        public int _vertexSize;
        [igField(8, typeof(igVertexElementArrayMetaField))]
        public igVertexElement[] _elements;
    }
}