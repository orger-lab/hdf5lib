using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

//using HDF.PInvoke;

//using static HDF.PInvoke.H5O;
//using static HDF.PInvoke.H5T;
//using static HDF.PInvoke.H5Z;

//using hdf5lib;
using System.Drawing;
using System.Security.Cryptography;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using ILNumerics.IO.HDF5;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using System.Threading.Channels;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace hdf5lib_tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[][] data = new byte[10][600*600];






0

1            var filePath = @"..\..\..\..\test.h5";







            H5File file = new hdf5lib.H5File(filePath, FileAccessMode.ReadWrite);
            var dataset = file["dsetname"];


            dataset[new ulong[] { 0, 0 }, new ulong[] { 3, 3 }] =  writearray;



            dataset.Write(new ulong[] { 0, 0 }, new ulong[] { 3, 3 }, writearray);




                    public Array this[ulong[] start, ulong[] count]
        {
            get => Read(start, count, false);
            set => Write(start, count, value);
        }




        ////H5DataSet dset = fl["dsetname"];

        ////var data = dset.Read();

        //////Array data2 = dset.Read(
        //////    start: new ulong[]{0, 0},
        //////    count: new ulong[]{3, 3},
        //////    dataShape: new ulong[] { 3, 3 }
        //////    ) ;


        ////int[] x = new int[25];

        ////Random r = new Random();

        ////for (int i = 0; i < 25; i++)
        ////    x[i] = r.Next();


        ////var xy = fl["dsetname"][new ulong[] { 0, 0 }, new ulong[] { 3, 3 }];


















        //Console.WriteLine("Hello, World!");


        //var filePath = @"..\..\..\..\test.h5";



        //var x = Array.CreateInstance(typeof(int), 10);



        ////H5File fl = new hdf5lib.H5File(filePath, FileAccessMode.ReadWrite);



        //Type t = x.GetType().GetElementType();



        //H5File fl = new hdf5lib.H5File(filePath, FileAccessMode.ReadOnly);
        //fl.atributes.Add("", new H5Attribute());




        //var fileID = H5F.open(filePath, H5F.ACC_RDONLY);







        ////var q = H5G.open(fileID, "/");





        H5File fl = new hdf5lib.H5File(filePath, FileAccessMode.ReadWrite);



        ////H5File fl = new hdf5lib.H5File(filePath, FileAccessMode.ReadWrite);

        fl.Datasets.Add( new H5Dataset(04,4,4,));
        fl.Atrributes.Add( new H5Dataset(04,4,4,));

            

             fl.Datasets.Add( 
                    new H5Dataset(04,4,4,).
                            Attrributes{});




        ////H5DataSet dset = fl["dsetname"];

        ////var data = dset.Read();

        ////Array data2 = dset.Read(
        ////    start: new ulong[]{0, 0},
        ////    count: new ulong[]{3, 3},
        ////    dataShape: new ulong[] { 3, 3 }
        ////    ) ;


        ////int[] x = new int[25];

        ////Random r = new Random();

        ////for (int i = 0; i < 25; i++)
        ////    x[i] = r.Next();


        var xy = fl["dsetname"][
                new ulong[] { 0, 0 },
                new ulong[] { 3, 3 }];






        }
    }


    class collection<T> where T : obj
    {
        
    }

    //class grp : collection<obj>
    //{
    //    Dictionary<string, attr> attrs;
    //    Dictionary<string, dset> datasets;
    //}


    class T
    {
        void doo()
        {

            var f = new fle();

            var q = f.Datasets;

        }
    
    }

    class fle : obj
    {

        h5ocoll<attr> attributes;
        h5ocoll<dset> datasets;


        internal override void create(long parentID)
        {
            datasets = new h5ocoll<dset>(id);
            attributes = new h5ocoll<attr>(id);
        }


        internal override void close()
        {
            // close attrs
            // close file
        }
    }


    class dset : obj
    {
        h5ocoll<attr> attributes;

        internal dset(fle file)
        {
        
        }


        internal override void create(long parentID)
        {
            attributes = new h5ocoll<attr>(id);
        }


        internal override void close()
        {
            // close attrs
            // close dataspace
            // close dset
        }

    }



    class attr : obj
    {
        internal attr(obj file)
        {

        }


        internal override void create(long parentID)
        {
            // close attrs
        }


        internal override void close()
        {
            // close attrs
        }
    }


    internal class h5ocoll<T> where T : obj
    {
        Dictionary<string, T> dict;

        long parentID;


        internal h5ocoll(long parentID)
        {
            dict = new Dictionary<string, T>();
            this.parentID = parentID;
        }
    


        public T this[string name]
        {
            get { return dict[name]; }
            set { }
        }


        public void Add(T t)
        {
            t.create(parentID);
            dict.Add(t.name, t);

        }

    }



    abstract class obj
    {
        public string name;

        internal long id;

        internal abstract void create(long parentID);
        internal abstract void close();
    }

    

}