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


            var nf = @"..\..\..\..\teststr.h5";
            var fh = H5F.open(nf, H5F.ACC_RDONLY);

            var attr_name = "str3";


            var attributeID = H5A.open(fh, attr_name);


            var typeHandle = H5A.get_type(attributeID);


            /*

            // create memtype
            // Note: I removed the status checks for readability, they are all zero
            memtype = H5T.copy(H5T_C_S1);
            status = H5T.set_size(memtype, H5T_VARIABLE);
            status = H5T.set_strpad(memtype, H5T_STR_NULLTERM);
            status = H5T.set_cset(memtype, H5T_CSET_UTF8);

            // get the storage size and space_id
            storage_size = H5D.get_storage_size(dset);
            space_id = H5D.get_space(dset);

            // allocate string buffer
            char* s = (char*)malloc(storage_size * sizeof(char));
            memset(s, 0, storage_size);

            // read string from dataset
            status = H5D.read(dset, memtype, space_id, H5S_ALL, H5P_DEFAULT, s);



            */


            var size = H5T.get_size(typeHandle);
            var pad = H5T.get_strpad(typeHandle);
            var cset = H5T.get_cset(typeHandle);

            // get the storage size and space_id
            var storage_size = H5A.get_storage_size(attributeID);
            var space_id = H5A.get_space(attributeID);
            var fscount = H5S.get_simple_extent_npoints(space_id);




            var ndims = H5S.get_simple_extent_ndims(space_id);
            var Dimensions = new ulong[ndims];
            var MaxDimensions = new ulong[ndims];
            var result = H5S.get_simple_extent_dims(space_id, Dimensions, MaxDimensions);




            //var mem_type = H5T.create(H5T.class_t.STRING, H5T.VARIABLE);

            //var fspace = H5D.get_space(attributeID);

            //var count = H5S.get_simple_extent_npoints(fspace);

            //var count = 1;

            IntPtr[] rdata = new IntPtr[fscount];
            GCHandle hnd = GCHandle.Alloc(rdata, GCHandleType.Pinned);



            H5A.read(attributeID, typeHandle, hnd.AddrOfPinnedObject());










            for (int i = 0; i < rdata.Length; ++i)
            {
                int len = 0;

                while (Marshal.ReadByte(rdata[i], len) != 0) { ++len; }
                byte[] buffer = new byte[len];
                //Marshal.Copy(rdata[i], buffer, 0, buffer.Length);
                //string s = Encoding.UTF8.GetString(buffer);





                //int len = (int)storage_size;
                //byte[] buffer = new byte[len];




                //byte[] buffer = new byte[len];
                Marshal.Copy(rdata[i], buffer, 0, buffer.Length);
                string s = Encoding.UTF8.GetString(buffer);
            }
        }
    }
}