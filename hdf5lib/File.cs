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
        private Dictionary<string, Attribute> atributes = new Dictionary<string, Attribute>();


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

            ulong idx = 0;
            H5L.iterate(fileID, H5.index_t.NAME, H5.iter_order_t.INC, ref idx, op, IntPtr.Zero);

            int op(long loc_id, IntPtr name, ref H5L.info_t info, IntPtr op_data)
            {
                var datasetName = Marshal.PtrToStringAnsi(name);
                var dataset = new H5DataSet(this.ID, datasetName);
                this.datasets.Add(datasetName,dataset);
                return 0;
            }
        }


  

        /// <summary>
        /// Closes all datasets in this file and then closes the file.
        /// </summary>
        public void Close()
        {
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



        /* THIS CODE GETS THE DATASET NAMES FROM THE FILE OBJECT
         
                     ulong idx =0;
            H5L.iterate(fileID, H5.index_t.NAME, H5.iter_order_t.INC, ref idx , op, IntPtr.Zero);

        private static int op(long loc_id, IntPtr name, ref H5L.info_t info, IntPtr op_data)
        {
            Console.WriteLine("------");
            var nm = Marshal.PtrToStringAnsi(name);
            Console.WriteLine($"i {nm}");
            var group = H5G.open(loc_id,$"/{nm}", H5P.DEFAULT);
            Console.WriteLine(group);
            Console.WriteLine($"o {nm}");
            H5G.close(group);
            return 0;
        }
         
         */
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
