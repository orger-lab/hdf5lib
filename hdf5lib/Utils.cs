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
            var size = (int)H5T.get_size(type);
            //var endianess = H5T.get_order(type);
            var clss = H5T.get_class(type);
            var signal = H5T.get_sign(type);

            
            switch (clss)
            {
                case H5T.class_t.INTEGER:
                    if (signal == H5T.sign_t.NONE) //unsigned
                    {
                        switch (size)
                        {
                            case 1: return "uint8";
                            case 2: return "uint16";
                            case 4: return "uint32";
                            case 8: return "uint64";
                            default: throw new InvalidCastException();
                        }
                    }
                    else
                    {
                        switch (size)
                        {
                            case 1: return "int8";
                            case 2: return "int16";
                            case 4: return "int32";
                            case 8: return "int64";
                            default: throw new InvalidCastException();
                        }
                    }

                case H5T.class_t.FLOAT:
                    if (size == 4)
                        return "float";
                    if (size == 8)
                        return "double";
                    throw new InvalidCastException();

                    

                default: throw new NotImplementedException();
            }





            //H5T_INTEGER
            //    H5T_ORDER_BE


            //size_t sz = H5Tget_size(type);
            //if (sz > 4)         // max 32 bit numbers are supported: 8/16 bit (u)int, float
            //    return 0;

            //H5T_order_t ord = H5Tget_order(type);
            //switch (H5Tget_class(type))
            //{
            //    case H5T_INTEGER:
            //        if (ord == H5T_ORDER_BE)
            //            return 0;
            //        if (sz > 2)     // max 16 bit intergers are supported
            //            return 0;
            //        break;
            //    case H5T_FLOAT:
            //        if (sz > 4)     // max 32 bit (single precision) floats are supported
            //            return 0;
            //        break;
            //    default:
            //        return 0;


            return "nope";

        }

    }
}
