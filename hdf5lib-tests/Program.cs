using System.Text;
using System.Xml.Linq;

using HDF.PInvoke;

using static HDF.PInvoke.H5Z;

namespace hdf5lib_tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");



            var filePath = @"C:\Users\alexa\OneDrive\Desktop\test.h5";
            
            var fileID = H5F.open(filePath, H5F.ACC_RDONLY);


            var dataset = "dsetname";
            var datasetID = H5D.open(fileID, $"/{dataset}");


            H5O.info_t info = new H5O.info_t();
            var r = H5O.get_info(datasetID, ref info);

            H5.ih_info_t

            H5O.get_info_by_idx(datasetID,"/", H5.index_t.NAME, H5.iter_order_t.INC, 2, ref info);



            //for (int i = 0; i < (int)info.num_attrs; i++)
            //{
            //    H5A.get_info_by_idx()
            //}


            //var t = H5A.get_type(datasetID);

            //int size = 256;
            //byte[] encoded = new byte[size];
            //var sn = H5A.get_name(datasetID, (IntPtr)size, encoded);
            //string decoded = Encoding.ASCII.GetString(encoded);

            //var storage = H5A.get_storage_size(datasetID);








        }

        


    }


   
}

