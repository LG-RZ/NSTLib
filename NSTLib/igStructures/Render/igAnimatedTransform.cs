namespace NSTLib.igStructures.Render
{
    public class igAnimatedTransform : igObject
    {
        [igField(16, typeof(igMatrix44fMetaField))]
        public igMatrix44f _matrix;
        [igField(80, typeof(igObjectMetaField))]
        public igAnimatedTransformSource _source;
    }
}
