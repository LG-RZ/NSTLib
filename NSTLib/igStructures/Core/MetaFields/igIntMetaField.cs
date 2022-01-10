using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core.MetaFields
{
    public class igShortMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadInt16();
        }

        public override void writeField(object @object, BinaryWriter writer)
        {
            writer.Write((short)@object);
        }
    }

    public class igUnsignedShortMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadUInt16();
        }

        public override void writeField(object @object, BinaryWriter writer)
        {
            writer.Write((ushort)@object);
        }
    }

    public class igIntMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadInt32();
        }

        public override void writeField(object @object, BinaryWriter writer)
        {
            writer.Write((int)@object);
        }
    }

    public class igUnsignedIntMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        public override void writeField(object @object, BinaryWriter writer)
        {
            writer.Write((uint)@object);
        }
    }

    public class igLongMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadInt64();
        }

        public override void writeField(object @object, BinaryWriter writer)
        {
            writer.Write((long)@object);
        }
    }

    public class igUnsignedLongMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadUInt64();
        }

        public override void writeField(object @object, BinaryWriter writer)
        {
            writer.Write((ulong)@object);
        }
    }
}
