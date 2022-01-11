using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core
{
    public class igHandleMetaField : igMetaField
    {
        [igField(0, typeof(igIntMetaField))]
        private int _handle;

        public override object readField(ExtendedBinaryReader reader)
        {
            igHandle handle = new igHandle();
            handle._index = (int)(_handle & ~0x80000000);
            if(_container._namedExternalList != null)
            {
                Tuple<uint, uint> namedHandle = _container._namedExternalList[handle._index];
                handle._type = _container._stringList[(int)namedHandle.Item1];
                handle._name = _container._stringList[(int)namedHandle.Item2];
                for (int i = 0; i < _container._objects.Items.Count; i++)
                {
                    var obj = _container._objects.Items[i]._metaName;
                    if (obj.Equals(handle._type))
                    {
                        handle._referencedObject = _container._objects.Items[i];
                        break;
                    }
                }
            }
            return base.readField(reader);
        }
    }

    public class igHandle
    {
        public int _index;
        public string _name;
        public string _type;
        public igObject _referencedObject;
    }
}
