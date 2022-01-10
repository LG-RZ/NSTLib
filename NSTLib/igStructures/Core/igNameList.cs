using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core
{
    public class igNameList : igTDataList<igNameList.Name>
    {
        public class Name : igMetaField
        {
            [igField(0, typeof(igStringMetaField))]
            public string _name;
            [igField(8, typeof(igUnsignedIntMetaField))]
            public uint _hash;

            public override int computeSize()
            {
                return 16;
            }
        }
    }
}
