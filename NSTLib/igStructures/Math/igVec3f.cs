using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Math
{
    public class igVec3fMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadStruct<igVec3f>();
        }

        public override int computeSize()
        {
            return 0xC;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
    public struct igVec3f
    {
        public igVec3f(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public static igVec3f Zero      = new igVec3f(0, 0, 0);
        public static igVec3f One       = new igVec3f(1, 1, 1);
        public static igVec3f Up        = new igVec3f(0, 1, 0);
        public static igVec3f Right     = new igVec3f(1, 0, 0);
        public static igVec3f Forward   = new igVec3f(0, 0, 1);

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", X, Y, Z);
        }
    }
}
