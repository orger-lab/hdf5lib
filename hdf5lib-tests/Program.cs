using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

using HDF.PInvoke;

using static HDF.PInvoke.H5O;
using static HDF.PInvoke.H5T;
using static HDF.PInvoke.H5Z;

using hdf5lib;

namespace hdf5lib_tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");


            //   static void GetDims()
            //{
            //    var space = H5D.get_space(datasetID);
            //    var ndims = H5S.get_simple_extent_ndims(space);
            //    ulong[] dims = new ulong[ndims];
            //    ulong[] maxdims = new ulong[ndims];
            //    H5S.get_simple_extent_dims(space, dims, maxdims);
            //}




            var filePath = @"..\..\..\..\test.h5";

            var fileID = H5F.open(filePath, H5F.ACC_RDONLY);

            var dset = H5D.open(fileID, "dsetname");


            var dt = H5D.get_type(dset);

            var t = H5T.get_native_type(dt, direction_t.ASCEND);


            Console.WriteLine(hdf5lib.Utils.ConvertTypeToH5Type2(dt));
            Console.WriteLine(hdf5lib.Utils.ConvertTypeToH5Type2(t));




            //ulong idx = 0;
            //H5L.iterate(fileID, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, op, IntPtr.Zero);

            //int op(long loc_id, IntPtr name, ref H5L.info_t info, IntPtr op_data)
            //{
            //    Console.WriteLine("------");
            //    var nm = Marshal.PtrToStringAnsi(name);
            //    Console.WriteLine($"i {nm}");
            //    var group = H5G.open(loc_id, $"/{nm}", H5P.DEFAULT);
            //    Console.WriteLine(group);
            //    Console.WriteLine($"o {nm}");
            //    H5G.close(group);
            //    return 0;
            //}

            //H5File fl = new hdf5lib.H5File(filePath, FileAccessMode.ReadOnly);


        }


    }


   
}

;