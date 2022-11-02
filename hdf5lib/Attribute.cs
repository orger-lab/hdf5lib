using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using HDF.PInvoke;
using static hdf5lib.Utils;

public class H5Attribute
{
    long ID;
    long dataSpaceID;
    long parentID;

    string Name;

    public Type DataType { get; private set; }

    /// <summary>
    /// internal representation of the underlying hdf5 format.
    /// </summary>
    private long H5TDataType;

    Array value;

	public H5Attribute(long parentID, string name, Array value)
	{

	}



    public H5Attribute(long parentID, string name, string value)
    {

    }

    public H5Attribute()
    {
        //var t = new StackFrame(1).ins
    }


    public void Close()
    {
        var status = H5S.close(dataSpaceID);
        CheckAndThrow(status);

        status = H5A.close(ID);
        CheckAndThrow(status);
    }



    void CreateDataSpace(Array data)
    {
        var dimensions = new ulong[data.Rank];
        for (int i = 0; i < data.Rank; i++)
        {
            dimensions[i] = (ulong)data.GetLength(i);
        }


        dataSpaceID = H5S.create_simple(dimensions.Length, dimensions, null);
        CheckAndThrow(dataSpaceID);
    }
    void CreateAttribute()
    {
        ID = H5A.create(parentID, Name, H5TDataType, dataSpaceID, H5P.DEFAULT);
        CheckAndThrow(ID);
    }



    //public void Create(long parentID, string name, Array data)
    //{
    //    CreateDataSpace(data);
    //    CreateAttribute();

    //    int result;

    //    unsafe
    //    {
    //        fixed (T* ptr = data)
    //        {
    //            result = H5A.write(ID, h5tType, (IntPtr)ptr);
    //            CheckAndThrow(spaceID);
    //        }
    //    }

    //    result = H5S.close(spaceID);
    //    CheckAndThrow(result);

    //    result = H5A.close(ID);
    //    CheckAndThrow(result);
    //}






    public static void Create<T>(long parentID, string name, T[] data) where T : unmanaged
    {
        var h5tType = ConvertTypeToH5T(typeof(T));

        var dimensions = new ulong[] { (ulong)data.Length };

        var spaceID = H5S.create_simple(dimensions.Length, dimensions, null);
        CheckAndThrow(spaceID);

        var ID = H5A.create(parentID, name, h5tType, spaceID, H5P.DEFAULT);
        CheckAndThrow(ID);

        int result;

        unsafe
        {
            fixed (T* ptr = data)
            {
                result = H5A.write(ID, h5tType, (IntPtr)ptr);
                CheckAndThrow(spaceID);
            }
        }
        
        result = H5S.close(spaceID);
        CheckAndThrow(result);

        result = H5A.close(ID);
        CheckAndThrow(result);
    }


    public void Create(long parentID, string name, string data)
    {
        byte[] encodedString = Encoding.ASCII.GetBytes(data); 

        var dimensions = new ulong[] { (ulong)encodedString.Length };

        var spaceID = H5S.create_simple(dimensions.Length, dimensions, null);
        CheckAndThrow(spaceID);

        var ID = H5A.create(parentID, name, H5T.NATIVE_CHAR, spaceID, H5P.DEFAULT);
        CheckAndThrow(ID);

        int result;

        unsafe
        {
            fixed (byte* ptr = encodedString)
            {
                result = H5A.write(ID, H5T.NATIVE_CHAR, (IntPtr)ptr);
                CheckAndThrow(spaceID);
            }
        }

        result = H5S.close(spaceID);
        CheckAndThrow(result);

        result = H5A.close(ID);
        CheckAndThrow(result);
    }

    //TODO : add code to read from file.

    // TODO : function that returns all attributes of a parentID



    // TODO : finish this function. depends on how the ctors are implemented
    public static Dictionary<string, H5Attribute> ExtractAllAttributes(long parentID)
    {
        Dictionary<string, H5Attribute> attributes = new Dictionary<string, H5Attribute>();

        ulong idx = 0;
        H5A.iterate(parentID, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, op, IntPtr.Zero);


        int op(long location_id, IntPtr attr_name, ref H5A.info_t ainfo, IntPtr op_data)
        {
            var attributeName = Marshal.PtrToStringAnsi(attr_name);
            Console.WriteLine(attributeName);
            return 0;
        }

        return attributes;
    }

}
