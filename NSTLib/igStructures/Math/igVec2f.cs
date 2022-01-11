using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Math
{
    public class igVec2fMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadStruct<igVec2f>();
        }

        public override int computeSize()
        {
            return 0x8;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    public struct igVec2f
    {
        public igVec2f(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public float X { get; set; }
        public float Y { get; set; }

        public static igVec2f Zero  = new igVec2f(0, 0);
        public static igVec2f One   = new igVec2f(1, 1);
        public static igVec2f Up    = new igVec2f(0, 1);
        public static igVec2f Right = new igVec2f(1, 0);

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }
    }
}
