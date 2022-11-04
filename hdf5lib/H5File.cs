using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;

using HDF.PInvoke;

namespace hdf5lib
{
    public class H5File
    {
        private long ID;
        public H5Collection<H5Attribute> Attributes { get; private set; }
        public H5Collection<H5DataSet> Datasets { get; private set; }


        /// <summary>
        /// Creates a new empty file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="mode"></param>
        public H5File(string filePath, FileCreationMode mode = FileCreationMode.Fail)
        {
            ID = H5F.create(filePath, (uint)mode, H5P.DEFAULT, H5P.DEFAULT);
            Utils.CheckAndThrow(ID);

            Datasets = new H5Collection<H5DataSet>(ID);
            Attributes = new H5Collection<H5Attribute>(ID);
        }

        /// <summary>
        /// Opens an existing file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="mode"></param>
        public H5File(string filePath, FileAccessMode mode = FileAccessMode.ReadOnly)
        {
            ID = H5F.open(filePath, (uint)mode);
            Utils.CheckAndThrow(ID);

            Datasets = H5DataSet.ExtractAll(ID);
            Attributes  = H5Attribute.ExtractAll(ID);
        }


  

        /// <summary>
        /// Closes all datasets in this file and then closes the file.
        /// </summary>
        public void Close()
        {
            //// TODO : Also close attributes
            //foreach (var dataset in datasets.Values)
            //{
            //    dataset.Close();
            //}
            //H5F.close(ID);
        }



        /// <summary>
        /// Returns a link to an existing dataset in the file.
        /// </summary>
        /// <param name="name">name of the dataset</param>
        /// <returns></returns>
        public H5DataSet this[string name]
        {
            get => Datasets[name];
        }



    }

    public enum FileAccessMode : uint
    {
        ReadOnly = H5F.ACC_RDONLY,
        /// <summary>
        /// Open for read and write.
        /// </summary>
        ReadWrite = H5F.ACC_RDWR,
    }

    public enum FileCreationMode : uint
    {
        /// <summary>
        /// Fail if file already exists.
        /// </summary>
        Fail = H5F.ACC_EXCL,
        /// <summary>
        /// Overwrite existing files.
        /// </summary>
        Overwrite = H5F.ACC_TRUNC,
    }
}
