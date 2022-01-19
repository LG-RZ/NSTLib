namespace NSTLib.igStructures.Render
{
    public class igModelDrawCallData : igObject
    {
        [igField(0, typeof(igStringMetaField))]
        public string _name;
        [igField(16, typeof(igVec3fMetaField))]
        public igVec3f _min;
        [igField(32, typeof(igVec3fMetaField))]
        public igVec3f _max;
        [igField(48, typeof(igHandleNameMetaField))]
        public Tuple<string, string> _materialHandle;
        [igField(56, typeof(igObjectMetaField))]
        public igGraphicsVertexBuffer _graphicsVertexBuffer;
        [igField(64, typeof(igObjectMetaField))]
        public igGraphicsIndexBuffer _graphicsIndexBuffer;
        [igField(84, typeof(igUnsignedShortMetaField))]
        public ushort _blendVectorOffset;
        [igField(86, typeof(igUnsignedShortMetaField))]
        public ushort _blendVectorCount;
        [igField(88, typeof(igIntMetaField))]
        public int _morphWeightTransformIndex;
        [igField(92, typeof(igIntMetaField))]
        public int _primitiveCount;
    }
}
