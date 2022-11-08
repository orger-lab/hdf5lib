using System;
using System.Runtime.InteropServices;
using System.Text;

using HDF.PInvoke;

using hdf5lib;

using static hdf5lib.Utils;


public class H5Attribute : H5Object
{
    long dataSpaceID;
    public DataType DataType { get; private set; }

    Array Data;
    ulong[] Dimensions;
    ulong[] MaxDimensions;

    public H5Attribute(string name, Array value)
    {
        Name = name;
        Data = value;

        DataType = new DataType(value.GetType().GetElementType());
    }

    public H5Attribute(string name, bool value) : this(name, Array.CreateInstance(typeof(bool), 1)) { Data.SetValue(value, 0); }
    public H5Attribute(string name, sbyte value) : this(name, Array.CreateInstance(typeof(sbyte), 1)) { Data.SetValue(value, 0); }
    public H5Attribute(string name, byte value) : this(name, Array.CreateInstance(typeof(byte), 1)) { Data.SetValue(value, 0); }
    public H5Attribute(string name, short value) : this(name, Array.CreateInstance(typeof(short), 1)) { Data.SetValue(value, 0); }
    public H5Attribute(string name, ushort value) : this(name, Array.CreateInstance(typeof(ushort), 1)) { Data.SetValue(value, 0); }
    public H5Attribute(string name, int value) : this(name, Array.CreateInstance(typeof(int), 1)) { Data.SetValue(value, 0); }
    public H5Attribute(string name, uint value) : this(name, Array.CreateInstance(typeof(uint), 1)) { Data.SetValue(value, 0); }
    public H5Attribute(string name, long value) : this(name, Array.CreateInstance(typeof(long), 1)) { Data.SetValue(value, 0); }
    public H5Attribute(string name, ulong value) : this(name, Array.CreateInstance(typeof(ulong), 1)) { Data.SetValue(value, 0); }
    public H5Attribute(string name, float value) : this(name, Array.CreateInstance(typeof(float), 1)) { Data.SetValue(value, 0); }
    public H5Attribute(string name, double value) : this(name, Array.CreateInstance(typeof(double), 1)) { Data.SetValue(value, 0); }
    public H5Attribute(string name, char value) : this(name, Array.CreateInstance(typeof(char), 1)) { Data.SetValue(value, 0); }
    public H5Attribute(string name, string value) : this(name, Array.CreateInstance(typeof(string), 1)) { Data.SetValue(value, 0); }

    internal H5Attribute(string name)
    {
        Name = name;
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
        Dimensions = new ulong[data.Rank];
        for (int i = 0; i < data.Rank; i++)
        {
            Dimensions[i] = (ulong)data.GetLength(i);
        }

        dataSpaceID = H5S.create_simple(Dimensions.Length, Dimensions, null);
        CheckAndThrow(dataSpaceID);
    }

    void CreateAttribute(long parentID)
    {
        ID = H5A.create(parentID, Name, DataType.HDF5, dataSpaceID, H5P.DEFAULT);
        CheckAndThrow(ID);
    }



    void Write()
    {

        GCHandle handle;
        if (DataType.Native == typeof(string))
        {
            IntPtr[] intPtrs = new IntPtr[Data.Length];
            int[] dims = Array.ConvertAll(Dimensions, p => (int)p);

            for (int i = 0; i < intPtrs.Length; i++)
            {
                int[] index = Utils.ConvertIndexToSubscript(dims, i);
                intPtrs[i] = Marshal.StringToHGlobalAnsi((string)Data.GetValue(index));
            }
            handle = GCHandle.Alloc(intPtrs, GCHandleType.Pinned);
        }
        else
        {
            handle = GCHandle.Alloc(Data, GCHandleType.Pinned);
        }

        IntPtr dataPtr = handle.AddrOfPinnedObject();
        int result = H5A.write(ID, DataType.HDF5, dataPtr);
        CheckAndThrow(result);
        handle.Free();
    }

    void Read(long parentID)
    {
        var attributeID = H5A.open(parentID, Name);

        var typeHandle = H5A.get_type(attributeID);
        DataType = new DataType(typeHandle);

        var dataSpaceID = H5A.get_space(attributeID);

        var ndims = H5S.get_simple_extent_ndims(dataSpaceID);

        Dimensions = new ulong[ndims];
        var MaxDimensions = new ulong[ndims];

        var result = H5S.get_simple_extent_dims(dataSpaceID, Dimensions, MaxDimensions);

        Data = Array.CreateInstance(DataType.Native, Array.ConvertAll(Dimensions, p => (int)p));


        if (DataType.Native == typeof(string))
        {
            var numberOfElements = H5S.get_simple_extent_npoints(dataSpaceID);
            var dataPointers = new IntPtr[numberOfElements];
            var handle = GCHandle.Alloc(dataPointers, GCHandleType.Pinned);

            H5A.read(attributeID, typeHandle, handle.AddrOfPinnedObject());

            int[] dataShape = new int[Data.Rank];
            for (int i = 0; i < dataShape.Length; i++)
            {
                dataShape[i] = Data.GetLength(i);
            }

            for (int i = 0; i < dataPointers.Length; ++i)
            {
                int len = 0;
                while (Marshal.ReadByte(dataPointers[i], len) != 0) { ++len; }

                byte[] buffer = new byte[len];
                Marshal.Copy(dataPointers[i], buffer, 0, buffer.Length);

                var dataAsString = Encoding.UTF8.GetString(buffer);
                Data.SetValue(dataAsString, Utils.ConvertIndexToSubscript(dataShape, i));
            }
            handle.Free();
        }
        else
        {
            var handle = GCHandle.Alloc(Data, GCHandleType.Pinned);
            IntPtr buf = handle.AddrOfPinnedObject();
            H5A.read(attributeID, typeHandle, buf);
            handle.Free();
        }

    }

    internal override void Close()
    {
        //var status = H5S.close(dataSpaceID);
        //CheckAndThrow(status);

        var status = H5A.close(ID);
        CheckAndThrow(status);
    }




    //internal void ReadVlen()
    //{
    //    var nf = @"..\..\..\..\teststr.h5";
    //    var fh = H5F.open(nf, H5F.ACC_RDONLY);

    //    var attr_name = "str3";


    //    var attributeID = H5A.open(fh, attr_name);


    //    var typeHandle = H5A.get_type(attributeID);





    //    var size = H5T.get_size(typeHandle);
    //    var pad = H5T.get_strpad(typeHandle);
    //    var cset = H5T.get_cset(typeHandle);

    //    // get the storage size and space_id
    //    var storage_size = H5A.get_storage_size(attributeID);
    //    var space_id = H5A.get_space(attributeID);
    //    var fscount = H5S.get_simple_extent_npoints(space_id);




    //    var ndims = H5S.get_simple_extent_ndims(space_id);
    //    var Dimensions = new ulong[ndims];
    //    var MaxDimensions = new ulong[ndims];
    //    var result = H5S.get_simple_extent_dims(space_id, Dimensions, MaxDimensions);




    //    //var rrr = Array.CreateInstance(typeof(string),new int[] {1,1});
    //    var rrr = Array.CreateInstance(typeof(string), Array.ConvertAll(Dimensions, p => (int)p));
    //    //GCHandle rrrhnd = GCHandle.Alloc(rrr, GCHandleType.Pinned);


    //    IntPtr[] rdata = new IntPtr[fscount];
    //    GCHandle hnd = GCHandle.Alloc(rdata, GCHandleType.Pinned);



    //    H5A.read(attributeID, typeHandle, hnd.AddrOfPinnedObject());


    //    for (int i = 0; i < rdata.Length; ++i)
    //    {
    //        int len = 0;
    //        while (Marshal.ReadByte(rdata[i], len) != 0) { ++len; }

    //        byte[] buffer = new byte[len];

    //        Marshal.Copy(rdata[i], buffer, 0, buffer.Length);

    //        var s = Encoding.UTF8.GetString(buffer);




    //        var siz = rrr.GetLength(0);
    //        var ndx = i;

    //        var vi = (ndx-1 % siz)  + 1;
    //        var v2 = ((ndx - vi)/siz + 1);
    //        var v1 = vi;


    //        var pos = new int[2];
    //        switch (i)
    //        {
    //            case 0:
    //                pos[0] = 0;
    //                pos[1] = 0;
    //                break;
    //            case 1:
    //                pos[0] = 0;
    //                pos[1] = 1;
    //                break;
    //            case 2:
    //                pos[0] = 1;
    //                pos[1] = 0;
    //                break;
    //            case 3:
    //                pos[0] = 1;
    //                pos[1] = 1;
    //                break;


    //        }


    //        //rrr.SetValue(int.Parse(s), pos);

    //    }
    //}





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
