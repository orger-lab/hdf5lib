using System;
using System.Collections.Generic;
using System.Text;

using HDF.PInvoke;

namespace hdf5lib
{
    public static class Utils
    {
        internal static void CheckAndThrow(long status)
        {
            if (status == -1)
            {
                throw new Exception();
            }
        }


        /// <summary>
        /// Converts native datatypes to the equivalent H5T code.
        /// </summary>
        /// <param name="type">.NET data type</param>
        /// <returns>H5T enumerated value for the input data type</returns>
        /// <exception cref="NotSupportedException"></exception>
        internal static long ConvertTypeToH5T(Type type)
        {
            // Boolean
            // bool
            if (type == typeof(bool))
                return H5T.NATIVE_HBOOL;

            // Integers
            // sbyte
            if (type == typeof(sbyte))
                return H5T.NATIVE_INT8;
            // byte
            if (type == typeof(byte))
                return H5T.NATIVE_UINT8;
            // short
            if (type == typeof(short))
                return H5T.NATIVE_INT16;
            // ushort
            if (type == typeof(ushort))
                return H5T.NATIVE_UINT16;
            // int
            if (type == typeof(int))
                return H5T.NATIVE_INT32;
            // uint
            if (type == typeof(uint))
                return H5T.NATIVE_UINT32;
            // long
            if (type == typeof(long))
                return H5T.NATIVE_INT64;
            // ulong
            if (type == typeof(ulong))
                return H5T.NATIVE_UINT64;

            // Floating point
            // float
            if (type == typeof(float))
                return H5T.NATIVE_FLOAT;
            // double
            if (type == typeof(double))
                return H5T.NATIVE_DOUBLE;

            throw new NotSupportedException($"Type {type} is not supported.");
        }


        public static string ConvertH5TToType(long type)
        {
            // Boolean
            // bool
            if (type == H5T.NATIVE_HBOOL)
                return "bool";

            // Integers
            // sbyte
            if (type == H5T.NATIVE_INT8)
                return "sbyte";
            // byte
            if (type == H5T.NATIVE_UINT8)
                return "byte";
            // short
            if (type == H5T.NATIVE_INT16)
                return "short";
            // ushort
            if (type == H5T.NATIVE_UINT16)
                return "ushort";
            // int
            if (type == H5T.NATIVE_INT32)
                return "int";
            // uint
            if (type == H5T.NATIVE_UINT32)
                return "uint";
            // long
            if (type == H5T.NATIVE_INT64)
                return "long";
            // ulong
            if (type == H5T.NATIVE_UINT64)
                return "ulong";

            // Floating point
            // float
            if (type == H5T.NATIVE_FLOAT)
                return "float";
            // double
            if (type == H5T.NATIVE_DOUBLE)
                return "double";

            //throw new NotSupportedException($"Type {type} is not supported.");
            return($"Type {type} is not supported.");
        }


        public static string ConvertTypeToH5Type2(long type)
        {

            if (type == H5T.NATIVE_CHAR)
                return "H5T_NATIVE_CHAR";
            if (type == H5T.NATIVE_SHORT)
                return "H5T_NATIVE_SHORT";
            if (type == H5T.NATIVE_INT)
                return "H5T_NATIVE_INT";
            if (type == H5T.NATIVE_LONG)
                return "H5T_NATIVE_LONG";
            if (type == H5T.NATIVE_LLONG)
                return "H5T_NATIVE_LLONG";
            if (type == H5T.NATIVE_UCHAR)
                return "H5T_NATIVE_UCHAR";
            if (type == H5T.NATIVE_USHORT)
                return "H5T_NATIVE_USHORT";
            if (type == H5T.NATIVE_UINT)
                return "H5T_NATIVE_UINT";
            if (type == H5T.NATIVE_ULONG)
                return "H5T_NATIVE_ULONG";
            if (type == H5T.NATIVE_ULLONG)
                return "H5T_NATIVE_ULLONG";
            if (type == H5T.NATIVE_FLOAT)
                return "H5T_NATIVE_FLOAT";
            if (type == H5T.NATIVE_DOUBLE)
                return "H5T_NATIVE_DOUBLE";
            if (type == H5T.NATIVE_LDOUBLE)
                return "H5T_NATIVE_LDOUBLE";
            if (type == H5T.NATIVE_B8)
                return "H5T_NATIVE_B8";
            if (type == H5T.NATIVE_B16)
                return "H5T_NATIVE_B16";
            if (type == H5T.NATIVE_B32)
                return "H5T_NATIVE_B32";
            if (type == H5T.NATIVE_B64)
                return "H5T_NATIVE_B64";

            return "nope";

        }

    }
}
