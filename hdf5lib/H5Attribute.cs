using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using System.Xml.Schema;

using HDF.PInvoke;

using hdf5lib;

using static HDF.PInvoke.H5O.hdr_info_t;
using static hdf5lib.Utils;


// TODO : add code to read from file.
// TODO : function that returns all attributes of a parentID
// TODO : methods to read and write strings
public class H5Attribute : H5Object
{
    long dataSpaceID;
    public Type DataType { get; private set; }

    /// <summary>
    /// internal representation of the underlying hdf5 format.
    /// </summary>
    private long H5TDataType;

    Array Data;

    public H5Attribute(string name, Array value)
    {
        Name = name;
        Data = value;
        DataType = value.GetType().GetElementType();
        H5TDataType =  Utils.ConvertTypeToH5T(DataType);

        //CreateDataSpace(Data);
    }


    internal H5Attribute(string name)
    {
        Name=name;
    }


    internal override void Create(long parentID)
    {
        if (Data == null)
        {
            Read(parentID);
        }
        else
        {
            CreateDataSpace(Data);
            CreateAttribute(parentID);
            Write();
        Close();

        }

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

    void CreateAttribute(long parentID)
    {
        ID = H5A.create(parentID, Name, H5TDataType, dataSpaceID, H5P.DEFAULT);
        CheckAndThrow(ID);
    }



    void Write()
    {
        var handle = GCHandle.Alloc(Data, GCHandleType.Pinned);
        IntPtr dataPtr = handle.AddrOfPinnedObject();

        int result = H5A.write(ID, H5TDataType, (IntPtr)dataPtr);
        CheckAndThrow(result);

        handle.Free();
    }

    void Read(long parentID)
    {

        byte[] attributeName = Encoding.ASCII.GetBytes(Name);

        var attributeID = H5A.open(parentID, attributeName);


        var typeHandle = H5A.get_type(attributeID);
        var dataType = Utils.ConvertH5TToType(typeHandle);
        var h5Type = Utils.ConvertTypeToH5T(dataType);


        var dataSpaceID = H5A.get_space(attributeID);

        var ndims = H5S.get_simple_extent_ndims(dataSpaceID);

        var Dimensions = new ulong[ndims];
        var MaxDimensions = new ulong[ndims];

        var result = H5S.get_simple_extent_dims(dataSpaceID, Dimensions, MaxDimensions);

        Data = Array.CreateInstance(dataType, Array.ConvertAll(Dimensions, p => (int)p));

        var handle = GCHandle.Alloc(Data, GCHandleType.Pinned);
        IntPtr buf = handle.AddrOfPinnedObject();

        H5A.read(attributeID, typeHandle, buf);

        handle.Free();
    }

    internal override void Close()
    {
        //var status = H5S.close(dataSpaceID);
        //CheckAndThrow(status);

        var status = H5A.close(ID);
        CheckAndThrow(status);
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



    /*


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
    */

    internal static H5Collection<H5Attribute> ExtractAll(long parentID)
    {
        H5Collection<H5Attribute> attributes = new H5Collection<H5Attribute>(parentID);

        ulong idx = 0;
        H5A.iterate(parentID, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, op, IntPtr.Zero);


        int op(long location_id, IntPtr attr_name, ref H5A.info_t ainfo, IntPtr op_data)
        {
            var attributeName = Marshal.PtrToStringAnsi(attr_name);
            var attribute = new H5Attribute(attributeName);
            attributes.Add(attribute);
            return 0;
        }

        return attributes;
    }
}
