namespace NSTLib.igStructures.Core.MetaFields
{
    public class igStructMetaField<T> : igMetaField
        where T : struct
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            Type type = typeof(T);
            return reader.ReadStruct(type.IsEnum ? Enum.GetUnderlyingType(type) : type);
        }
    }
}
