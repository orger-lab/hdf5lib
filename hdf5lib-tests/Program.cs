using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

using HDF.PInvoke;

using static HDF.PInvoke.H5O;
using static HDF.PInvoke.H5T;
using static HDF.PInvoke.H5Z;

using hdf5lib;
using System.Drawing;
using System.Security.Cryptography;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace hdf5lib_tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");


            var filePath = @"..\..\..\..\test.h5";



            var x = Array.CreateInstance(typeof(int), 10);



            //H5File fl = new hdf5lib.H5File(filePath, FileAccessMode.ReadWrite);



            Type t = x.GetType().GetElementType();



            H5File fl = new hdf5lib.H5File(filePath, FileAccessMode.ReadOnly);
            fl.atributes.Add("", new H5Attribute());




            var fileID = H5F.open(filePath, H5F.ACC_RDONLY);







            //var q = H5G.open(fileID, "/");





            //H5File fl = new hdf5lib.H5File(filePath, FileAccessMode.ReadWrite);



            //H5File fl = new hdf5lib.H5File(filePath, FileAccessMode.ReadWrite);




            //H5DataSet dset = fl["dsetname"];

            //var data = dset.Read();

            ////Array data2 = dset.Read(
            ////    start: new ulong[]{0, 0},
            ////    count: new ulong[]{3, 3},
            ////    dataShape: new ulong[] { 3, 3 }
            ////    ) ;


            //int[] x = new int[25];

            //Random r = new Random();

            //for (int i = 0; i < 25; i++)
            //    x[i] = r.Next();


            //var xy = fl["dsetname"][new ulong[] { 0, 0 }, new ulong[] { 3, 3 }];



            ////fl.CreateDataset("new_data", typeof(int), new ulong[] { 5, 5 });
            //fl["new_data"][new ulong[] { 0, 0 }, new ulong[] { 5, 5 }] = x;


            //fl.Close();

            H5File fle = new hdf5lib.H5File(filePath, FileAccessMode.ReadWrite);


            
            
            var xy = fle["dsetname"];


        }
    }



    class file
    {
        public atrr this[string name]
        {
            get => datasets[name];
        }

    }



    class atrr
    {
        long parentid;
        long id;
        string name;


    }


}