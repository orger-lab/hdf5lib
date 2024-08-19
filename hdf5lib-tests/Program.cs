using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

using HDF.PInvoke;

//using static HDF.PInvoke.H5O;
//using static HDF.PInvoke.H5T;
//using static HDF.PInvoke.H5Z;

using hdf5lib;
using System.Drawing;
using System.Security.Cryptography;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

//using ILNumerics.IO.HDF5;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using System.Threading.Channels;
using System.Runtime.CompilerServices;
using System.Data;
using static HDF.PInvoke.H5O.hdr_info_t;

namespace hdf5lib_tests
{
    internal class Program
    {

        private static byte[] MakeImage(ulong width, ulong height, ulong depth)
        {
            byte[] data = new byte[width * height * depth * 2];
            Random rnd = Random.Shared;
            byte[] bytes = new byte[2];

            //rnd.NextBytes(data);

            for (int i = 0; i < (int)(width * height * depth); i++)
            {
                int value = rnd.Next(-5, 12) + 399;
                bytes = BitConverter.GetBytes((short)value);
                data[i * 2] = bytes[0];
                data[(i * 2) + 1] = bytes[1];
            }
            return data;
        }

        static void Main(string[] args)
        {


            //ushort gray16 = ushort.MaxValue / 100;

            //byte gray8 = (byte)(gray16 >> 8);




            H5File file = new H5File(@"F:\Heka Scans\7130\7130.h5", FileAccessMode.ReadOnly);



            var attr = file.Attributes["acquisition_date"];

            //int[] data = (int[])attr.Data;


            string data = attr.DataAs<string>();

            //var v = data[0];




            //int H = 576;
            //int W = 640;

            //ulong  index = 1;

            //ulong[] start = new ulong[] { index, 0, 0 };
            //ulong[] count = new ulong[] { 1, (ulong)(W  * H), 6 };

            //short[] data = new short[W*H*6];


            //unsafe
            //{
            //    fixed (short* ptr = data)
            //    {
            //        file["subordinate_pointcloud_depthspace"].Read(start, count, false, (IntPtr)ptr);
            //    }
            //}




            //return;

            //ulong x = 5;
            //ulong y = 5;
            //ulong z = 5;

            //byte[] data = MakeImage(x, y, 1);
            //UInt16[] data2 = { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5 };

            //string filename = "newfile.h5";

            //H5File myFile = new H5File(filename, FileCreationMode.Overwrite);


            //// minimum dataset
            //ulong[] dims = new ulong[]{ x, y, z };
            //var dtype = typeof(UInt16);
            //string datasetName = "testDataset";

            //// dataset extras
            //ulong[] chunks = new ulong[] { x, 1, 1 };
            //int filter = 306;
            //uint[] cd_values = new uint[]{ 4000 };

            //myFile.Datasets.Add(new H5DataSet(datasetName, dtype, dims, chunks, filter, cd_values));

            //ulong[] start = new ulong[] { 0, 0, 0 };
            //ulong[] count = new ulong[] { x, y, 1 };

            //myFile["testDataset"][start, count] = data2;

            //myFile.Attributes.Add(new H5Attribute("fileAttr", 1));
            //myFile["testDataset"].Attributes.Add(new H5Attribute("pixels", new float[] { 0.8f, 0.8f, 1.0f }));

            //myFile.Close();

        }
    }

    class b : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
