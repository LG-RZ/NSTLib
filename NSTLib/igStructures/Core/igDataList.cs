using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Core
{
    public class igDataList : igObject
    {
        #region Fields

        [igField(0, typeof(igIntMetaField))]
        private int _count;
        [igField(4, typeof(igIntMetaField))]
        public int _capacity;
        [igField(8, typeof(igMemoryRefMetaField))]
        public byte[] _data;

        #endregion

        #region Methods

        public int getCount()
        {
            return _count;
        }

        public int getCapacity()
        {
            return _capacity;
        }

        public byte[] getRawData()
        {
            return _data;
        }

        #endregion
    }
}
