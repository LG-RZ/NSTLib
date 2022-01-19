using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSTLib.igStructures.Gfx
{
    public class igVertexConversion
    {
        public static igVec4f unpackData(byte[] data, IG_VERTEX_TYPE type)
        {
            unsafe
            {
                igVec4f result = new igVec4f(0f, 0f, 0f, 0f);

                fixed(byte* dataPtr = data)
                switch (type)
                {
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_FLOAT1:
                        unpack_FLOAT1(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_FLOAT2:
                        unpack_FLOAT2(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_FLOAT3:
                        unpack_FLOAT3(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_FLOAT4:
                        unpack_FLOAT4(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UBYTE4N_COLOR:
                        unpack_UBYTE4N_COLOR(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UBYTE4N_COLOR_ARGB:
                        unpack_UBYTE4N_COLOR_ARGB(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UBYTE4N_COLOR_RGBA:
                        unpack_UBYTE4N_COLOR_RGBA(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UBYTE2N_COLOR_5650:
                        unpack_UBYTE2N_COLOR_5650(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UBYTE2N_COLOR_5551:
                        unpack_UBYTE2N_COLOR_5551(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UBYTE2N_COLOR_4444:
                        unpack_UBYTE2N_COLOR_4444(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_INT1:
                        unpack_INT1(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_INT2:
                        unpack_INT2(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_INT4:
                        unpack_INT4(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UINT1:
                        unpack_UINT1(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UINT2:
                        unpack_UINT2(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UINT4:
                        unpack_UINT4(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_INT1N:
                        unpack_INT1N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_INT2N:
                        unpack_INT2N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_INT4N:
                        unpack_INT4N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UINT1N:
                        unpack_UINT1N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UINT2N:
                        unpack_UINT2N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UINT4N:
                        unpack_UINT4N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UBYTE4:
                        unpack_UBYTE4(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UBYTE4_X4:
                        unpack_UBYTE4_X4(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_BYTE4:
                        unpack_BYTE4(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UBYTE4N:
                        unpack_UBYTE4N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_BYTE4N:
                        unpack_BYTE4N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_SHORT2:
                        unpack_SHORT2(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_SHORT4:
                        unpack_SHORT4(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_USHORT2:
                        unpack_USHORT2(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_USHORT4:
                        unpack_USHORT4(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_SHORT2N:
                        unpack_SHORT2N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_SHORT3N:
                        unpack_SHORT3N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_SHORT4N:
                        unpack_SHORT4N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_USHORT2N:
                        unpack_USHORT2N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_USHORT3N:
                        unpack_USHORT3N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_USHORT4N:
                        unpack_USHORT4N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UDEC3:
                        unpack_UDEC3(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_DEC3N:
                        unpack_DEC3N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_DEC3N_S11_11_10:
                        unpack_DEC3N_S11_11_10(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_HALF2:
                        unpack_HALF2(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_HALF4:
                        unpack_HALF4(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_BYTE3N:
                        unpack_BYTE3N(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_SHORT3:
                        unpack_SHORT3(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_USHORT3:
                        unpack_USHORT3(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UBYTE4_ENDIAN:
                        unpack_UBYTE4_ENDIAN(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UBYTE4_COLOR:
                        unpack_UBYTE4_COLOR(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_BYTE3:
                        unpack_BYTE3(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UBYTE2N_COLOR_5650_RGB:
                        unpack_UBYTE2N_COLOR_5650_RGB(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_UDEC3_OES:
                        unpack_UDEC3_OES(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_DEC3N_OES:
                        unpack_DEC3N_OES(&result, dataPtr);
                        break;
                    case IG_VERTEX_TYPE.IG_VERTEX_TYPE_SHORT4N_EDGE:
                            unpack_SHORT4N_EDGE(&result, dataPtr);
                        break;
                    }

                return result;
            }
        }

        #region FLOAT 1 -> 4

        public unsafe static void unpack_FLOAT1(igVec4f* result, byte* data)
        {
            result->X = *(float*)data;
            result->Y = 0f;
            result->Z = 0f;
            result->W = 0f;
        }

        public unsafe static void unpack_FLOAT2(igVec4f* result, byte* data)
        {
            result->X = *(float*)data;
            result->Y = *(float*)(data + 0x4);
            result->Z = 0f;
            result->W = 0f;
        }

        public unsafe static void unpack_FLOAT3(igVec4f* result, byte* data)
        {
            result->X = *(float*)data;
            result->Y = *(float*)(data + 0x4);
            result->Z = *(float*)(data + 0x8);
            result->W = 0f;
        }

        public unsafe static void unpack_FLOAT4(igVec4f* result, byte* data)
        {
            result->X = *(float*)data;
            result->Y = *(float*)(data + 0x4);
            result->Z = *(float*)(data + 0x8);
            result->W = *(float*)(data + 0xC);
        }

        #endregion

        #region COLOR

        public unsafe static void unpack_UBYTE4_COLOR(igVec4f* result, byte* data)
        {
            result->X = *data++;
            result->Y = *data++;
            result->Z = *data++;
            result->W = *data;
        }

        public unsafe static void unpack_UBYTE4N_COLOR(igVec4f* result, byte* data)
        {
            result->X = *data++ / 255.0f;
            result->Y = *data++ / 255.0f;
            result->Z = *data++ / 255.0f;
            result->W = *data / 255.0f;
        }

        public unsafe static void unpack_UBYTE4N_COLOR_ARGB(igVec4f* result, byte* data)
        {
            result->Z = *data++ / 255.0f;
            result->Y = *data++ / 255.0f;
            result->X = *data++ / 255.0f;
            result->W = *data / 255.0f;
        }

        public unsafe static void unpack_UBYTE4N_COLOR_RGBA(igVec4f* result, byte* data)
        {
            result->W = *data++ / 255.0f;
            result->Z = *data++ / 255.0f;
            result->Y = *data++ / 255.0f;
            result->X = *data / 255.0f;
        }

        public unsafe static void unpack_UBYTE2N_COLOR_5650(igVec4f* result, byte* data)
        {
            ushort @short = *(ushort*)data;

            result->X = (@short & 0x1f) / 31.0f;
            result->Y = (@short >> 0x5 & 0x3f) / 63.0f;
            result->Z = (@short >> 0xb & 0x1f) / 31.0f;
            result->W = 1f;
        }

        public unsafe static void unpack_UBYTE2N_COLOR_5551(igVec4f* result, byte* data)
        {
            ushort @short = *(ushort*)data;

            result->X = (@short & 0x1f) / 15.0f;
            result->Y = (@short >> 0x5 & 0x1f) / 15.0f;
            result->Z = (@short >> 0xa & 0x1f) / 15.0f;
            result->W = (@short >> 0xf & 0x1);
        }

        public unsafe static void unpack_UBYTE2N_COLOR_4444(igVec4f* result, byte* data)
        {
            ushort @short = *(ushort*)data;

            result->X = (@short & 0xf) / 15.0f;
            result->Y = (@short >> 0x4 & 0xf) / 15.0f;
            result->Z = (@short >> 0x8 & 0xf) / 15.0f;
            result->W = (@short >> 0xc & 0xf) / 15.0f;
        }

        public unsafe static void unpack_UBYTE2N_COLOR_5650_RGB(igVec4f* result, byte* data)
        {
            ushort @short = *(ushort*)data;

            result->Z = (@short & 0x1f) / 31.0f;
            result->Y = (@short >> 0x5 & 0x3f) / 63.0f;
            result->X = (@short >> 0xb & 0x1f) / 31.0f;
            result->W = 1f;
        }

        #endregion

        #region INT 1 -> 4

        public unsafe static void unpack_INT1(igVec4f* result, byte* data)
        {
            result->X = *(int*)data;
            result->Y = 0f;
            result->Z = 0f;
            result->W = 1f;
        }

        public unsafe static void unpack_INT2(igVec4f* result, byte* data)
        {
            result->X = *(int*)data;
            result->Y = *(int*)(data + 0x4);
            result->Z = 0f;
            result->W = 1f;
        }

        public unsafe static void unpack_INT4(igVec4f* result, byte* data)
        {
            result->X = *(int*)data;
            result->Y = *(int*)(data + 0x4);
            result->Z = *(int*)(data + 0x8);
            result->W = *(int*)(data + 0xC);
        }

        #endregion

        #region UINT 1 -> 4

        public unsafe static void unpack_UINT1(igVec4f* result, byte* data)
        {
            result->X = *(uint*)data;
            result->Y = 0f;
            result->Z = 0f;
            result->W = 1f;
        }

        public unsafe static void unpack_UINT2(igVec4f* result, byte* data)
        {
            result->X = *(uint*)data;
            result->Y = *(uint*)(data + 0x4);
            result->Z = 0f;
            result->W = 1f;
        }

        public unsafe static void unpack_UINT4(igVec4f* result, byte* data)
        {
            result->X = *(uint*)data;
            result->Y = *(uint*)(data + 0x4);
            result->Z = *(uint*)(data + 0x8);
            result->W = *(uint*)(data + 0xC);
        }

        #endregion

        #region INT 1 -> 4 N

        public unsafe static void unpack_INT1N(igVec4f* result, byte* data)
        {
            result->X = *(int*)data * 4.656613e-10f;
            result->Y = 0f;
            result->Z = 0f;
            result->W = 1f;
        }

        public unsafe static void unpack_INT2N(igVec4f* result, byte* data)
        {
            result->X = *(int*)data * 4.656613e-10f;
            result->Y = *(int*)(data + 0x4) * 4.656613e-10f;
            result->Z = 0f;
            result->W = 1f;
        }

        public unsafe static void unpack_INT4N(igVec4f* result, byte* data)
        {
            result->X = *(int*)data * 4.656613e-10f;
            result->Y = *(int*)(data + 0x4) * 4.656613e-10f;
            result->Z = *(int*)(data + 0x8) * 4.656613e-10f;
            result->W = *(int*)(data + 0xC) * 4.656613e-10f;
        }

        #endregion

        #region UINT 1 -> 4 N

        public unsafe static void unpack_UINT1N(igVec4f* result, byte* data)
        {
            result->X = *(uint*)data * 2.328306e-10f;
            result->Y = 0f;
            result->Z = 0f;
            result->W = 1f;
        }

        public unsafe static void unpack_UINT2N(igVec4f* result, byte* data)
        {
            result->X = *(uint*)data * 2.328306e-10f;
            result->Y = *(uint*)(data + 0x4) * 2.328306e-10f;
            result->Z = 0f;
            result->W = 1f;
        }

        public unsafe static void unpack_UINT4N(igVec4f* result, byte* data)
        {
            result->X = *(uint*)data * 2.328306e-10f;
            result->Y = *(uint*)(data + 0x4) * 2.328306e-10f;
            result->Z = *(uint*)(data + 0x8) * 2.328306e-10f;
            result->W = *(uint*)(data + 0xC) * 2.328306e-10f;
        }

        #endregion

        #region BYTE & UBYTE

        public unsafe static void unpack_BYTE3(igVec4f* result, byte* data)
        {
            result->X = (*data + (*data++ & 0x80) * -2);
            result->Y = (*data + (*data++ & 0x80) * -2);
            result->Z = (*data + (*data & 0x80) * -2);
            result->W = 1f;
        }

        public unsafe static void unpack_BYTE4(igVec4f* result, byte* data)
        {
            result->X = (*data + (*data++ & 0x80) * -2);
            result->Y = (*data + (*data++ & 0x80) * -2);
            result->Z = (*data + (*data++ & 0x80) * -2);
            result->W = (*data + (*data   & 0x80) * -2);
        }

        public unsafe static void unpack_UBYTE4(igVec4f* result, byte* data)
        {
            result->X = *data++;
            result->Y = *data++;
            result->Z = *data++;
            result->W = *data;
        }

        public unsafe static void unpack_UBYTE4_X4(igVec4f* result, byte* data)
        {
            result->X = *data++ * 0.25f;
            result->Y = *data++ * 0.25f;
            result->Z = *data++ * 0.25f;
            result->W = *data * 0.25f;
        }

        public unsafe static void unpack_UBYTE4_ENDIAN(igVec4f* result, byte* data)
        {
            result->W = *data++;
            result->Z = *data++;
            result->Y = *data++;
            result->Z = *data;
        }

        public unsafe static void unpack_UBYTE4N(igVec4f* result, byte* data)
        {
            result->X = *data++ / 255.0f;
            result->Y = *data++ / 255.0f;
            result->Z = *data++ / 255.0f;
            result->W = *data   / 255.0f;
        }

        public unsafe static void unpack_BYTE3N(igVec4f* result, byte* data)
        {
            result->X = (*data + (*data++ & 0x80) * -2);
            result->Y = (*data + (*data++ & 0x80) * -2);
            result->Z = (*data + (*data & 0x80) * -2);
            result->W = 1f;
        }

        public unsafe static void unpack_BYTE4N(igVec4f* result, byte* data)
        {
            result->X = ((*data & 0x7f) + (*data++ & 0x80) * -2) / 127.0f;
            result->Y = ((*data & 0x7f) + (*data++ & 0x80) * -2) / 127.0f;
            result->Z = ((*data & 0x7f) + (*data++ & 0x80) * -2) / 127.0f;
            result->W = ((*data & 0x7f) + (*data   & 0x80) * -2) / 127.0f;
        }

        #endregion

        #region SHORT 1 -> 4

        public unsafe static void unpack_SHORT2(igVec4f* result, byte* data)
        {
            result->X = *(short*)data;
            result->Y = *(short*)(data + 0x2);
            result->Z = 0f;
            result->W = 1f;
        }

        public unsafe static void unpack_SHORT3(igVec4f* result, byte* data)
        {
            result->X = *(short*)data;
            result->Y = *(short*)(data + 0x2);
            result->Z = *(short*)(data + 0x4);
            result->W = 1f;
        }

        public unsafe static void unpack_SHORT4(igVec4f* result, byte* data)
        {
            result->X = *(short*)data;
            result->Y = *(short*)(data + 0x2);
            result->Z = *(short*)(data + 0x4);
            result->W = *(short*)(data + 0x6);
        }

        #endregion

        #region USHORT 1 -> 4

        public unsafe static void unpack_USHORT2(igVec4f* result, byte* data)
        {
            result->X = *(ushort*)data;
            result->Y = *(ushort*)(data + 0x2);
            result->Z = 0f;
            result->W = 1f;
        }

        public unsafe static void unpack_USHORT3(igVec4f* result, byte* data)
        {
            result->X = *(ushort*)data;
            result->Y = *(ushort*)(data + 0x2);
            result->Z = *(ushort*)(data + 0x4);
            result->W = 1f;
        }

        public unsafe static void unpack_USHORT4(igVec4f* result, byte* data)
        {
            result->X = *(ushort*)data;
            result->Y = *(ushort*)(data + 0x2);
            result->Z = *(ushort*)(data + 0x4);
            result->W = *(ushort*)(data + 0x6);
        }

        #endregion

        #region SHORT 1 -> 4 N

        public unsafe static void unpack_SHORT2N(igVec4f* result, byte* data)
        {
            result->X = *(short*)data / 32767.0f;
            result->Y = *(short*)(data + 0x2) / 32767.0f;
            result->Z = 0f;
            result->W = 1f;
        }

        public unsafe static void unpack_SHORT3N(igVec4f* result, byte* data)
        {
            result->X = *(short*)data / 32767.0f;
            result->Y = *(short*)(data + 0x2) / 32767.0f;
            result->Z = *(short*)(data + 0x4) / 32767.0f;
            result->W = 1f;
        }

        public unsafe static void unpack_SHORT4N(igVec4f* result, byte* data)
        {
            result->X = *(short*)data / 32767.0f;
            result->Y = *(short*)(data + 0x2) / 32767.0f;
            result->Z = *(short*)(data + 0x4) / 32767.0f;
            result->W = *(short*)(data + 0x6) / 32767.0f;
        }

        public unsafe static void unpack_SHORT4N_EDGE(igVec4f* result, byte* data)
        {
            result->X = *(short*)data * 3.051804e-05f + 1.525902e-05f;
            result->Y = *(short*)(data + 0x2) * 3.051804e-05f + 1.525902e-05f;
            result->Z = *(short*)(data + 0x4) * 3.051804e-05f + 1.525902e-05f;
            result->W = *(short*)(data + 0x6) * 3.051804e-05f + 1.525902e-05f;
        }

        #endregion

        #region USHORT 1 -> 4 N

        public unsafe static void unpack_USHORT2N(igVec4f* result, byte* data)
        {
            result->X = *(ushort*)data / 65535.0f;
            result->Y = *(ushort*)(data + 0x2) / 65535.0f;
            result->Z = 0f;
            result->W = 1f;
        }

        public unsafe static void unpack_USHORT3N(igVec4f* result, byte* data)
        {
            result->X = *(ushort*)data / 65535.0f;
            result->Y = *(ushort*)(data + 0x2) / 65535.0f;
            result->Z = *(ushort*)(data + 0x4) / 65535.0f;
            result->W = 1f;
        }

        public unsafe static void unpack_USHORT4N(igVec4f* result, byte* data)
        {
            result->X = *(ushort*)data / 65535.0f;
            result->Y = *(ushort*)(data + 0x2) / 65535.0f;
            result->Z = *(ushort*)(data + 0x4) / 65535.0f;
            result->W = *(ushort*)(data + 0x6) / 65535.0f;
        }

        #endregion

        #region DEC & UDEC

        public unsafe static void unpack_DEC3N(igVec4f* result, byte* data)
        {
            uint @int = *(uint*)data;

            result->X = ((@int & 0x3ff) + (@int & 0x200) * -2) / 511.0f;
            result->Y = ((@int >> 0xa & 0x3ff) + (@int >> 0xa & 0x200) * -2) / 511.0f;
            result->Z = ((@int >> 0x14 & 0x3ff) + (@int >> 0x14 & 0x200) * -2) / 511.0f;
            result->W = 1f;
        }

        public unsafe static void unpack_DEC3N_OES(igVec4f* result, byte* data)
        {
            uint @int = *(uint*)data;

            result->X = ((@int >> 0x16 & 0x3ff) + (@int >> 0x16 & 0x200) * -2) / 511.0f;
            result->Y = ((@int >> 0xc & 0x3ff) + (@int >> 0xc & 0x200) * -2) / 511.0f;
            result->Z = ((@int >> 0x2 & 0x3ff) + (@int >> 0x2 & 0x200) * -2) / 511.0f;
            result->W = 1f;
        }

        public unsafe static void unpack_DEC3N_S11_11_10(igVec4f* result, byte* data)
        {
            uint @int = *(uint*)data;

            result->X = ((@int & 0x7ff) + (@int & 0x400) * -2) / 1023.0f;
            result->Y = ((@int >> 0xb & 0x7ff) + (@int >> 0xb & 0x400) * -2) / 1023.0f;
            result->Z = ((@int >> 0x16 & 0x3ff) + (@int >> 0x16 & 0x200) * -2) / 511.0f;
            result->W = 1f;
        }

        public unsafe static void unpack_UDEC3(igVec4f* result, byte* data)
        {
            uint @int = *(uint*)data;

            result->X = (@int & 0x3ff);
            result->Y = (@int >> 0xa & 0x3ff);
            result->Z = (@int >> 0x14 & 0x3ff);
            result->W = 1f;
        }

        public unsafe static void unpack_UDEC3_OES(igVec4f* result, byte* data)
        {
            uint @int = *(uint*)data;

            result->X = (@int >> 0x16& 0x3ff);
            result->Y = (@int >> 0xc & 0x3ff);
            result->Z = (@int >> 0x2 & 0x3ff);
            result->W = 1f;
        }

        #endregion

        #region HALF 2 & 4

        public unsafe static void unpack_HALF2(igVec4f* result, byte* data)
        {
            result->X = (float)*(Half*)data;
            result->Y = (float)*(Half*)(data + 0x2);
            result->Z = 0f;
            result->W = 1f;
        }

        public unsafe static void unpack_HALF4(igVec4f* result, byte* data)
        {
            result->X = (float)*(Half*)data;
            result->Y = (float)*(Half*)(data + 0x2);
            result->Z = (float)*(Half*)(data + 0x4);
            result->W = (float)*(Half*)(data + 0x6);
        }

        #endregion
    }
}
