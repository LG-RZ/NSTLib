using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures
{
    public class igIGZFixupSection
    {
        public uint _magicnumber;
        public int _count;
        public int _length;
        public int _headerSize;
        public long _dataPointer;
    }
}
