using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.IO
{
    public static class MemoryUtils
    {
        public static T ToStruct<T>(byte[] data)
            where T : struct
        {
            GCHandle gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<T>(gcHandle.AddrOfPinnedObject());
            }
            finally
            {
                gcHandle.Free();
            }
        }

        public static object ToStruct(object structure, byte[] data)
        {
            GCHandle gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), (Type)structure);
            }
            finally
            {
                gcHandle.Free();
            }
        }

        public static byte[] StructToBytes<T>(T structure) where T : struct
        {
            byte[] data = new byte[Marshal.SizeOf<T>()];
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(structure, handle.AddrOfPinnedObject(), false);
                return data;
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
