using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Math
{
    public class igVec3hMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadStruct<igVec3h>();
        }

        public override int computeSize()
        {
            return 0x6;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
    public struct igVec3h
    {
        public igVec3h(float X, float Y, float Z)
        {
            this.X = (Half)X;
            this.Y = (Half)Y;
            this.Z = (Half)Z;
        }

        public igVec3h(Half X, Half Y, Half Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Half X { get; set; }
        public Half Y { get; set; }
        public Half Z { get; set; }

        public static igVec3h Zero      = new igVec3h(0, 0, 0);
        public static igVec3h One       = new igVec3h(1, 1, 1);
        public static igVec3h Up        = new igVec3h(0, 1, 0);
        public static igVec3h Right     = new igVec3h(1, 0, 0);
        public static igVec3h Forward   = new igVec3h(0, 0, 1);

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", X, Y, Z);
        }
    }
}
