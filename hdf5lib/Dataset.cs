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
        //private long fileID;
        //private string name;
        //private byte[] data;
        //private ChannelOrder channelOrder;
        //private ulong[] maxdims;
        //private int count = 0;


        /////////////

        private long datasetID;
        private long dataType;

        private long dataSpaceID;
        private long propertyListID;

        private ulong[] dimensions;// i dont think this is needed after the ctor
        private ulong[] chunks; // i dont think this is needed after the ctor
        private int compressionFilter; // i dont think this is needed after the ctor
        private uint[] compressionOpts; // i dont think this is needed after the ctor


        public Dataset(long fileID, string name, Type datatype, ulong[] dimensions, ulong[] chunks = null, int compressionFilter =0, uint[] compressionOptions=null)
        {
            SetDataType(datatype);
            CreateDataSpace(dimensions);
            CreatePropertyList();
            if (chunks != null)
            {
                SetChunk(chunks);
                if (compressionFilter != 0 && compressionOptions != null) // TODO : convert to CompressionFilter object c#+ req
                {
                    SetCompression(compressionFilter, compressionOptions);
                }
            }
            //TODO : there is no path for when chunks is null but not the filter
            CreateDataset(fileID, name);
        }


        private void SetDataType(Type dataType)
        {
            this.dataType = Utils.ConvertTypeToH5T(dataType);
        }
        private void CreateDataSpace(ulong[] dimensions)
        {
            this.dimensions = dimensions;
            dataSpaceID = H5S.create_simple(dimensions.Length, dimensions, null);
            CheckAndThrow(dataSpaceID);
        }

        private void CreatePropertyList()
        {
            propertyListID = H5P.create(H5P.DATASET_CREATE);
            CheckAndThrow(propertyListID);
        }


        private void SetChunk(ulong[] chunks)
        {
            if (chunks.Length != dimensions.Length)
            {
                throw new Exception($"{nameof(chunks)} must be the same overall shape as {nameof(dimensions)}");
            }

            // Set chunks
            var result = H5P.set_chunk(propertyListID, chunks.Length, chunks);
            CheckAndThrow(result);
        }


        private void SetCompression(int compressionFilter, uint[] compressionOptions)
        {
            var result = H5Z.filter_avail((filter_t)compressionFilter);
            if (result == 0)
            {
                throw new Exception("Filter not available");
            }
            result = H5P.set_filter(propertyListID, (filter_t)compressionFilter, 1, (IntPtr)compressionOptions.Length, compressionOptions);
            CheckAndThrow(result);
        }

        private void CreateDataset(long fileID, string name)
        {
            datasetID = H5D.create(fileID, name, dataType, dataSpaceID, H5P.DEFAULT, propertyListID, H5P.DEFAULT);
            CheckAndThrow(datasetID);
        }

      
        public unsafe void Append<T>(T[] dataToWrite, ulong[] start, ulong[] count, ulong[] dimsToWrite) where T : unmanaged 
        {
            int status;
            long memorySpaceID;

            fixed (T* buf = dataToWrite)
            fixed (ulong* startPtr = start)
            fixed (ulong* countPtr = count)
            {
                status = H5S.select_hyperslab(dataSpaceID, H5S.seloper_t.SET, startPtr, null, countPtr, null);
                CheckAndThrow(status);

                memorySpaceID = H5S.create_simple(dimsToWrite.Length, dimsToWrite, null);
                CheckAndThrow(memorySpaceID);

                status = H5D.write(datasetID, dataType, memorySpaceID, dataSpaceID, H5P.DEFAULT, (IntPtr)buf);
                CheckAndThrow(status);

                status = H5S.close(memorySpaceID);
                CheckAndThrow(status);
            }
        }

        public void Close()
        {
            H5D.close(datasetID);
        }
    }



}
