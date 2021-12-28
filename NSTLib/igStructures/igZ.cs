using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures
{
    public class igZ
    {
        #region Structures

        public struct Header
        {
            public int _magicnumber;
            public int _version;
            public int _checksum; // placeholder name
            public int _platform;
        }

        #endregion
    }
}
