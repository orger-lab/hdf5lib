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

    private H5Attribute(string name)
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
