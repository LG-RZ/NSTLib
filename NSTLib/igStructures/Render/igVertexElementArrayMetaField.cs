using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Render
{
    public class igVertexElementArrayMetaField : igMemoryRefMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadArray<igVertexElement>(_memSize / 0xC);
        }
    }
}
