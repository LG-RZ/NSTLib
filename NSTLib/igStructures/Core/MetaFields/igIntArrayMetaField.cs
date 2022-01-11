using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core.MetaFields
{
    public class igIntArrayMetaField : igMemoryRefMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            byte[] buffer = (byte[])base.readField(reader);
            int[] array = new int[buffer.Length / 4];
            Buffer.BlockCopy(buffer, 0, array, 0, buffer.Length);
            return array;
        }
    }
}
