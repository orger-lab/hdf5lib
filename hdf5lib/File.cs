using System;
using System.Collections.Generic;
using System.Data;

using HDF.PInvoke;

namespace hdf5lib
{
    public class File
    {
        private long _fileID;
        private Dictionary<string, Dataset> datasets = new Dictionary<string, Dataset>();


        public File(string filePath)
        {
            Open(filePath);
        }

        private void Open(string filePath)
        {
            //TODO : add read mode
            var file = H5F.create(filePath, H5F.ACC_TRUNC, H5P.DEFAULT, H5P.DEFAULT);
            Utils.CheckAndThrow(file);
            _fileID = file;
        }


        public Dataset CreateDataSet(
           string name,
           DataType dataType,
           ulong[] dims,
           byte[] data = null,
           ulong[] maxdims = null,
           ulong[] chunks = null,
           int compressionFilter = 0,
           uint[] compressionOpts = null,
           ChannelOrder channelOrder = ChannelOrder.TCZYX)
        {
            var dset = new H5DataSet(_fileID, name, dataType, dims, channelOrder, data, maxdims, chunks, compressionFilter, compressionOpts);
            datasets.Add(dset);
            return dset;
        }

    }
}
