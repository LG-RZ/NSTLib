namespace NSTLib.igStructures.Render
{
    public class igModelData : igObject
    {
        [igField(0, typeof(igStringMetaField))]
        public string _name;

        [igField(16, typeof(igVec3fMetaField))]
        public igVec3f _min;

        [igField(32, typeof(igVec3fMetaField))]
        public igVec3f _max;

        [igField(48, typeof(igReadObjectMetaField<igTDataList<igAnimatedTransform>>))]
        public igTDataList<igAnimatedTransform> _transforms;

        [igField(72, typeof(igReadObjectMetaField<igTDataList<int>>))]
        public igTDataList<int> _transformHierarchy;

        [igField(96, typeof(igReadObjectMetaField<igTDataList<igModelDrawCallData>>))]
        public igTDataList<igModelDrawCallData> _drawCalls;

        [igField(120, typeof(igReadObjectMetaField<igTDataList<int>>))]
        public igTDataList<int> _drawCallTransformIndices;

        [igField(144, typeof(igReadObjectMetaField<igTDataList<igAnimatedMorphWeightsTransform>>))]
        public igTDataList<igAnimatedMorphWeightsTransform> _morphWeightTransforms;

        [igField(168, typeof(igReadObjectMetaField<igTDataList<int>>))]
        public igTDataList<int> _blendMatrixIndices;
    }
}
