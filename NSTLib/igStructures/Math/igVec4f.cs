using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Math
{
    public class igVec4fMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadStruct<igVec4f>();
        }

        public override int computeSize()
        {
            return 0x10;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
    public struct igVec4f
    {
        public igVec4f(float X, float Y, float Z, float W)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;
        }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public static igVec4f Zero = new igVec4f(0, 0, 0, 0);
        public static igVec4f One  = new igVec4f(1, 1, 1, 1);

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", X, Y, Z, W);
        }
    }
}
