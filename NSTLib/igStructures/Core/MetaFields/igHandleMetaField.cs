using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core.MetaFields
{
    public class igHandleMetaField : igMetaField
    {
        [igField(0, typeof(igIntMetaField))]
        private int _handle;

        public override object readField(ExtendedBinaryReader reader)
        {
            if (_container._namedExternalList != null)
            {
                Tuple<uint, uint> namedHandle = _container._namedExternalList[(int)(_handle & ~0x80000000)];
                string name = _container._stringsList[(int)namedHandle.Item1];
                for (int i = 0; i < _container._objects.Items.Count; i++)
                {
                    var obj = _container._objects.Items[i]._metaName;
                    if (obj.Equals(name))
                    {
                        return _container._objects.Items[i];
                    }
                }
            }
            return null;
        }
    }
}
