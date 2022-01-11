using System.Runtime.InteropServices;

namespace NSTLib.igStructures.Render
{
    [StructLayout(LayoutKind.Explicit, Size = 0xC)]
    public struct igVertexElement
    {
        public igVertexElement(IG_VERTEX_USAGE usage, byte method, IG_VERTEX_TYPE type, byte usageIndex)
        {
            _usage = usage;
            _method = method;
            _type = type;
            _usageIndex = usageIndex;
            _offset = 0;
        }

        [FieldOffset(0)] public IG_VERTEX_TYPE _type;
        [FieldOffset(3)] public byte _method;
        [FieldOffset(4)] public IG_VERTEX_USAGE _usage;
        [FieldOffset(5)] public byte _usageIndex;
        [FieldOffset(8)] public int _offset;
    }
}
