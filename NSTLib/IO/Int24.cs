using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.IO
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Int24
    {
        private byte _a;
        private byte _b;
        private byte _c;

        public static implicit operator int(Int24 sword)
        {
            int value = sword._a | (sword._b << 8) | (sword._c << 16);
            return (value & 0x0800000) > 0 ? (int)(value | 0xFF000000) : (int)(value & ~0xFF000000); 
        }

        public static implicit operator Int24(int integer) => new Int24() { _a = (byte)(integer >> 0), _b = (byte)(integer >> 8), _c = (byte)(integer >> 16) };

        public override string ToString()
        {
            int value = _a | (_b << 8) | (_c << 16);
            return ((value & 0x0800000) > 0 ? (int)(value | 0xFF000000) : (int)(value & ~0xFF000000)).ToString();
        }
    }
}
