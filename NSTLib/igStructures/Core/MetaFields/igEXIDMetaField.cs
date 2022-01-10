using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core.MetaFields
{
    // Actual Name of the MetaField is unknown
    public class igEXIDMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return _container._externalList[reader.ReadInt32()].Item1;
        }

        public override int computeSize()
        {
            return 4;
        }
    }
}
