using System;
using System.Text;

using HDF.PInvoke;
using static hdf5lib.Utils;

public class Attribute
{
    long ID;
    string Name;

	public Attribute()
	{
	}

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


    public static void Create(long parentID, string name, string data)
    {
        byte[] encodedString = Encoding.ASCII.GetBytes(data);

        var dimensions = new ulong[] { (ulong)data.Length };

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

}
