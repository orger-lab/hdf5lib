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
            var fileID  = H5F.create(filePath, H5F.ACC_TRUNC, H5P.DEFAULT, H5P.DEFAULT);


            ulong[] dims = new ulong[] {10,10};
            var dataspace = H5S.create_simple(dims.Length, dims, null);



            var pList = H5P.create(H5P.DATASET_CREATE);



            ulong[] chunks = new ulong[] {5,5};
            var result = H5P.set_chunk(pList, chunks.Length, chunks);




            int compressionFilter = 1;
            uint[] compressionOpts = new uint[] { 9 };
                
                
            var filter = H5P.set_filter(pList, (filter_t)compressionFilter, 1, (IntPtr)compressionOpts.Length, compressionOpts);









        }

        


    }


    public  class test
    {

        public test(int a)
        {
            Console.WriteLine("A");
        }

        public test(int a, int b) : this(a)
        {
            Console.WriteLine("B");
        
        }
    }
}