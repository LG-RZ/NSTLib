using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core
{
    public abstract class igMetaField : igObject
    {
        public virtual object readField(ExtendedBinaryReader reader)
        {
            return null;
        }

        public virtual void writeField(object @object, BinaryWriter writer)
        {
        }

        public virtual int computeSize()
        {
            return 0;
        }
    }
}
