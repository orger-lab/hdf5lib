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

namespace hdf5lib_tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var filePath = @"..\..\..\..\test.h5";

            H5File file = new hdf5lib.H5File(filePath, FileAccessMode.ReadWrite);



            file.Attributes.Add(new H5Attribute("test123", new int[] { 420, 69 }));

            //var a = new ulong[] { 2, 10, 10 };

            //Console.WriteLine();

            //file.Datasets.Add(
            //    new H5DataSet("v12", typeof(double), new ulong[] { 2, 10, 10 })
            //    //{ Attributes = {new H5Attribute("cenas",new int[] {5,6,7})}}
            //    );


            //file["v12"][new ulong[] { 0, 0, 0 }, new ulong[] { 1, 1, 3 }] = new double[] { 1, 2, 3 };


        }
    }
}