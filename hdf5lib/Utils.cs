using System;
using System.Collections.Generic;
using System.Text;

using HDF.PInvoke;

namespace hdf5lib
{
    internal static class Utils
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
    }
}
