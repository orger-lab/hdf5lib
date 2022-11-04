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
        private Dictionary<string, H5DataSet> datasets = new Dictionary<string, H5DataSet>();
        public Dictionary<string, H5Attribute> atributes = new Dictionary<string, H5Attribute>();


        /// <summary>
        /// Creates a new empty file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="mode"></param>
        public H5File(string filePath, FileCreationMode mode = FileCreationMode.Fail)
        {
            
            var fileID = H5F.create(filePath, (uint)mode, H5P.DEFAULT, H5P.DEFAULT);
            Utils.CheckAndThrow(fileID);
            ID = fileID;
        }

        /// <summary>
        /// Opens an existing file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="mode"></param>
        public H5File(string filePath, FileAccessMode mode = FileAccessMode.ReadOnly)
        {
            var fileID = H5F.open(filePath, (uint)mode);
            Utils.CheckAndThrow(fileID);
            ID = fileID;

            
        }


  

        /// <summary>
        /// Closes all datasets in this file and then closes the file.
        /// </summary>
        public void Close()
        {
            // TODO : Also close attributes
            foreach (var dataset in datasets.Values)
            {
                dataset.Close();
            }
            H5F.close(ID);
        }


        public H5DataSet CreateDataset(string name, Type datatype, ulong[] dims, ulong[] chunks = null, int compressionFilter = 0, uint[] compressionOptions = null)
        {
            var dataset = new H5DataSet(ID, name, datatype, dims, chunks, compressionFilter, compressionOptions);
            datasets.Add(name,dataset);

            
            return dataset;
        }


        /// <summary>
        /// Returns a link to an existing dataset in the file.
        /// </summary>
        /// <param name="name">name of the dataset</param>
        /// <returns></returns>
        public H5DataSet this[string name]
        {
            get => datasets[name];
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
