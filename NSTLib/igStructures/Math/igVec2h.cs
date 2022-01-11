using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Math
{
    public class igVec2hMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadStruct<igVec2h>();
        }

        public override int computeSize()
        {
            return 0x4;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
    public struct igVec2h
    {
        public igVec2h(float X, float Y)
        {
            this.X = (Half)X;
            this.Y = (Half)Y;
        }

        public igVec2h(Half X, Half Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public Half X { get; set; }
        public Half Y { get; set; }

        public static igVec2h Zero  = new igVec2h(0, 0);
        public static igVec2h One   = new igVec2h(1, 1);
        public static igVec2h Up    = new igVec2h(0, 1);
        public static igVec2h Right = new igVec2h(1, 0);

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }
    }
}
