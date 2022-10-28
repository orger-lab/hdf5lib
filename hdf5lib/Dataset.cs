using HDF.PInvoke;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

using static HDF.PInvoke.H5Z;
using static hdf5lib.Utils;


namespace hdf5lib
{
    public class Dataset
    {
        private long _fileID;
        private string name;
        private DataType datatype;
        private ulong[] dims;
        private byte[]? data;
        private ChannelOrder channelOrder;
        private ulong[]? maxdims;
        private ulong[]? chunks;
        private int? compressionFilter;
        private uint[]? compressionOpts;

        private long dataSpaceID = -1;
        private long plistID = -1;
        private long _dsetID = -1;
        private long dtype = -1;
        private int count = 0;




        public Dataset(
                long fileID,
                string name,
                DataType datatype,
                ulong[] dims,
                ChannelOrder channelOrder = ChannelOrder.TCZYX,
                byte[] data = null,
                ulong[] maxdims = null,
                ulong[] chunks = null,
                int compressionFilter = -1,
                uint[] compressionOpts = null)
        {
            this._fileID = fileID;
            this.name = name;
            this.datatype = datatype;
            this.dims = dims;
            this.channelOrder = channelOrder;
            this.data = data;
            this.maxdims = maxdims;
            this.chunks = chunks;
            this.compressionFilter = compressionFilter;
            this.compressionOpts = compressionOpts;
            Create();
        }



        private void Create()
        {
            switch(datatype)
            {
                case DataType.NATIVE_FLOAT:
                    dtype = H5T.NATIVE_FLOAT;
                    break;
                case DataType.NATIVE_UINT16:
                default:
                    dtype = H5T.NATIVE_UINT16;
                    break;
            }

            CreateDataSpace(dims.Length);
            CreateParameterList(dims.Length);

            _dsetID = H5D.create(_fileID, name, dtype, dataSpaceID, H5P.DEFAULT, plistID, H5P.DEFAULT);
            CheckAndThrow(_dsetID);


            // Special case where we want to just write out the data set
            if (data != null)
            {
                Write(data);
            }
        }

        private void CreateDataSpace(int ndims)
        {
            var result = H5S.create_simple(ndims, dims, null);
            CheckAndThrow(result);
            dataSpaceID = result;
        }

        private void CreateParameterList(int ndims)
        {
            plistID = H5P.create(H5P.DATASET_CREATE);
            CheckAndThrow(plistID);

            if (chunks != null)
            {
                if (chunks.Length != ndims)
                {
                    throw new Exception("Chunks must be the same overall shape as dims");
                }

                // Set chunks
                var result = H5P.set_chunk(plistID, chunks.Length, chunks);
                CheckAndThrow(result);

                if (compressionFilter != null & compressionOpts != null)
                {
                    var filterAvailable = H5Z.filter_avail((filter_t)compressionFilter);
                    if (filterAvailable == 0)
                    {
                        throw new Exception("Filter not available");
                    }
                    H5P.set_filter(plistID, (filter_t)compressionFilter, 1, (IntPtr)compressionOpts.Length, compressionOpts);
                }
            }
            else
            {
                if (compressionFilter != null)
                {
                    throw new Exception("Can't use compression filters without chunking");
                }
            }
        }



        private unsafe void Write(byte[] data)
        {
            fixed (byte* buf = data)
            {
                var result = H5D.write(_dsetID, dtype, H5S.ALL, H5S.ALL, H5P.DEFAULT, (IntPtr)buf);
                CheckAndThrow(result);
            }
        }

      
        public unsafe void Append<T>(T[] dataToWrite, ulong[] start, ulong[] count, ulong[] dimsToWrite) where T : unmanaged 
        {
            int status;
            long mspace_id;

            fixed (T* buf = dataToWrite)
            fixed (ulong* startPtr = start)
            fixed (ulong* countPtr = count)
            {
                status = H5S.select_hyperslab(dataSpaceID, H5S.seloper_t.SET, startPtr, null, countPtr, null);
                CheckAndThrow(status);

                mspace_id = H5S.create_simple(dimsToWrite.Length, dimsToWrite, null);
                CheckAndThrow(mspace_id);

                status = H5D.write(_dsetID, dtype, mspace_id, dataSpaceID, H5P.DEFAULT, (IntPtr)buf);
                CheckAndThrow(status);

                status = H5S.close(mspace_id);
                CheckAndThrow(status);
            }
        }
    }


    public enum ChannelOrder
    {
        /// <summary>
        /// Standard Imaging Order [value = 0]
        /// </summary>
        TCZYX = 0,
        /// <summary>
        /// Reversed Imaging Order [value = 1]
        /// </summary>
        XYZCT = 1,
        /// <summary>
        /// Linear Datasets [value = 2]
        /// Used for saving 1 dimensional
        /// datasets (ie average value of each frame or volume over time)
        /// </summary>
        T = 2,

    } // ChannelOrder

    public enum DataType : long
    {
        NATIVE_UINT16 = 0,
        NATIVE_FLOAT = 1,
    }

}
