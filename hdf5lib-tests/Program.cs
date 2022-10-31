using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

using HDF.PInvoke;

using static HDF.PInvoke.H5O;
using static HDF.PInvoke.H5T;
using static HDF.PInvoke.H5Z;

using hdf5lib;
using System.Drawing;

namespace hdf5lib_tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");


            var filePath = @"..\..\..\..\test.h5";

            //var fileID = H5F.open(filePath, H5F.ACC_RDONLY);

            //var dset = H5D.open(fileID, "dsetname");


            //var dt = H5D.get_type(dset);

            //var t = H5T.get_native_type(dt, direction_t.ASCEND);




            H5File fl = new hdf5lib.H5File(filePath, FileAccessMode.ReadOnly);

            H5DataSet dset = fl["dsetname"];

            var data = dset.Read();

            Array data2 = dset.Read(
                start: new ulong[]{0, 0},
                count: new ulong[]{3, 3},
                dataShape: new ulong[] { 3, 3 }
                ) ;
        }


        }


   
}