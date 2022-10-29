using System;
using System.Collections.Generic;
using System.Data;

using HDF.PInvoke;

namespace hdf5lib
{
    public class File
    {
        private long ID;
        private Dictionary<string, Dataset> datasets = new Dictionary<string, Dataset>();


        public File(string filePath)
        {
            Open(filePath);
        }

        private void Open(string filePath)
        {
            //TODO : add read mode
            var fileID = H5F.create(filePath, H5F.ACC_TRUNC, H5P.DEFAULT, H5P.DEFAULT);
            Utils.CheckAndThrow(fileID);
            ID = fileID;
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


        public Dataset CreateDataset(string name, Type datatype, ulong[] dims, ulong[] chunks = null, int compressionFilter = 0, uint[] compressionOptions = null)
        {
            var dataset = new Dataset(ID, name, datatype, dims, chunks, compressionFilter, compressionOptions);
            datasets.Add(name,dataset);
            return dataset;
        }


        /// <summary>
        /// Returns a link to an existing dataset in the file.
        /// </summary>
        /// <param name="name">name of the dataset</param>
        /// <returns></returns>
        public Dataset this[string name]
        {
            get => datasets[name];
        }

    }
}
