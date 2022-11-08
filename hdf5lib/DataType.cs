using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using HDF.PInvoke;

namespace hdf5lib
{
    /// <summary>
    /// Helper class the handle the conversion between Native and HDF5 data types.
    /// </summary>
    [DebuggerDisplay("{Native.Name}")]
    public class DataType
    {
        /// <summary>
        /// Native dotnet data Type.
        /// </summary>
        public readonly Type Native;
        /// <summary>
        /// Encoded HDF5 data type.
        /// </summary>
        public readonly long HDF5;

        public DataType(Type native)
        {
            Native = native;
            HDF5  = ConvertTypeToH5T(native);
        }

        public DataType(long hdf55)
        {
            Native = ConvertH5TToType(hdf55);
            HDF5   = ConvertTypeToH5T(Native);
        }

        /// <summary>
        /// Converts native datatypes to the equivalent H5T code.
        /// </summary>
        /// <param name="type">.NET data type</param>
        /// <returns>H5T enumerated value for the input data type</returns>
        /// <exception cref="NotSupportedException"></exception>
        private static long ConvertTypeToH5T(Type type)
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
            
            // Text
            // char
            if (type == typeof(char))
                return H5T.NATIVE_CHAR;
            // string
            if (type == typeof(string))
            {
                var strtype = H5T.copy(H5T.C_S1);
                H5T.set_size(strtype, H5T.VARIABLE);
                return strtype;
            }
            throw new NotSupportedException($"Type {type} is not supported.");
        }

        /// <summary>
        /// Converts an H5T type object to the native Type object.
        /// </summary>
        /// <param name="type">handle the to the H5T object</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        private static Type ConvertH5TToType(long type)
        {
            var size = (int)H5T.get_size(type);
            //var endianess = H5T.get_order(type);
            var clss = H5T.get_class(type);
            var signal = H5T.get_sign(type);

            switch (clss)
            {
                case H5T.class_t.INTEGER:
                    if (signal == H5T.sign_t.NONE)
                    {
                        switch (size)
                        {
                            case 1: return typeof(byte);
                            case 2: return typeof(ushort);
                            case 4: return typeof(uint);
                            case 8: return typeof(ulong);
                            default: throw new InvalidCastException();
                        }
                    }
                    else
                    {
                        switch (size)
                        {
                            case 1: return typeof(sbyte);
                            case 2: return typeof(short);
                            case 4: return typeof(int);
                            case 8: return typeof(long);
                            default: throw new InvalidCastException();
                        }
                    }
                case H5T.class_t.FLOAT:
                    if (size == 4)
                        return typeof(float);
                    if (size == 8)
                        return typeof(double);
                    throw new InvalidCastException();

                case H5T.class_t.STRING:
                    return typeof(string);

                default: throw new NotImplementedException();
            }
        }
    }
}
