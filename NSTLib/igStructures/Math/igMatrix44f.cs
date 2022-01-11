using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Math
{
    public class igMatrix44fMetaField : igMetaField
    {
        public override object readField(ExtendedBinaryReader reader)
        {
            return reader.ReadStruct<igMatrix44f>();
        }

        public override int computeSize()
        {
            return 0x40;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x40)]
    public struct igMatrix44f
    {
        public igMatrix44f(igVec4f Row0, igVec4f Row1, igVec4f Row2, igVec4f Row3)
        {
            this.Row0 = Row0;
            this.Row1 = Row1;
            this.Row2 = Row2;
            this.Row3 = Row3;
        }

        public igVec4f Row0 { get; set; }
        public igVec4f Row1 { get; set; }
        public igVec4f Row2 { get; set; }
        public igVec4f Row3 { get; set; }

        public static igMatrix44f Identity = new igMatrix44f(new igVec4f(1, 0, 0, 0),
                                                             new igVec4f(0, 1, 0, 0),
                                                             new igVec4f(0, 0, 1, 0),
                                                             new igVec4f(0, 0, 0, 1));

        public override string ToString()
        {
            return string.Format("({0},\r\n{1},\r\n{2},\r\n{3})", Row0, Row1, Row2, Row3);
        }
    }
}
