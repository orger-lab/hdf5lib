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
        static void Main(string[] args)
        {
            var nf = @"..\..\..\..\test.h5";


            var file = new H5File(nf, FileAccessMode.ReadWrite);



            file.Datasets.Add(new H5DataSet()


            var data = file["dsetname"].Read();




            file.Attributes.Add(new H5Attribute("7og", "tesoiwreoer"));



            file["aaronsdataset"][new[] {0ul,0ul,1ul}] = new[] { 10, 20, 30 };

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
