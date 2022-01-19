using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core.MetaFields
{
    public class igHandleNameMetaField : igMetaField
    {
        [igField(0, typeof(igIntMetaField))]
        private int _handle;

        public override object readField(ExtendedBinaryReader reader)
        {
            if (_container._namedExternalList != null)
            {
                Tuple<uint, uint> namedHandle = _container._namedExternalList[(int)(_handle & ~0x80000000)];
                return new Tuple<string, string>(_container._stringsList[(int)namedHandle.Item1], _container._stringsList[(int)namedHandle.Item2]);
            }
            return null;
        }
    }
}
