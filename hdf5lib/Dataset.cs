using HDF.PInvoke;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using static hdf5lib.Utils;


namespace hdf5lib
{
    public class Dataset
    {
        private Dictionary<string, Attribute> atributes = new Dictionary<string, Attribute>();


        //private long fileID;
        //private string name;
        //private byte[] data;
        //private ChannelOrder channelOrder;
        //private ulong[] maxdims;
        //private int count = 0;


        /////////////

        private long ID;
        private long dataType;

        private long dataSpaceID;
        private long propertyListID;

        private ulong[] dimensions;// i dont think this is needed after the ctor
        private ulong[] chunks; // i dont think this is needed after the ctor
        private int compressionFilter; // i dont think this is needed after the ctor
        private uint[] compressionOpts; // i dont think this is needed after the ctor



        /*
         
         H5PY METHODS
            __bool__()
            __getitem__(args)
            __setitem__(args)
            asstr(encoding=None, errors='strict')
            astype(dtype)
            fields(names)
            iter_chunks()
            len()
            make_scale(name='')
            read_direct(array, source_sel=None, dest_sel=None)
            resize(size, axis=None)
            virtual_sources()
            write_direct(source, source_sel=None, dest_sel=None)

        H5PY PROPERTIES
            attrs
            chunks
            compression
            compression_opts
            dims
            dtype
            external
            file
            fillvalue
            fletcher32
            id
            is_virtual
            maxshape
            name
            nbytes
            ndim
            parent
            ref
            regionref
            scaleoffset
            shape
            shuffle
            size 

         */


        public Dataset(long fileID, string name, Type datatype, ulong[] dimensions, ulong[] chunks = null, int compressionFilter = 0, uint[] compressionOptions = null)
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
            var result = H5Z.filter_avail((H5Z.filter_t)compressionFilter);
            if (result == 0)
            {
                throw new Exception("Filter not available");
            }
            result = H5P.set_filter(propertyListID, (H5Z.filter_t)compressionFilter, 1, (IntPtr)compressionOptions.Length, compressionOptions);
            CheckAndThrow(result);
        }

        private void CreateDataset(long fileID, string name)
        {
            ID = H5D.create(fileID, name, dataType, dataSpaceID, H5P.DEFAULT, propertyListID, H5P.DEFAULT);
            CheckAndThrow(ID);
        }

      
        public unsafe void Write<T>(T[] dataToWrite, ulong[] start, ulong[] count, ulong[] dimsToWrite) where T : unmanaged 
        {
            // TODO : add simple rank test to see if the data fits in the selected memory space
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

                status = H5D.write(ID, dataType, memorySpaceID, dataSpaceID, H5P.DEFAULT, (IntPtr)buf);
                CheckAndThrow(status);

                status = H5S.close(memorySpaceID);
                CheckAndThrow(status);
            }
        }

        // TODO : Improve perfomance by creating methods to direcr read/write w/ memspaces
        public unsafe void Read<T>(out T[] data, ulong[] start, ulong[] count, ulong[] dataShape) where T : unmanaged
        {
            // TODO : add simple rank test to see if the data fits in the selected memory space

            int status;
            long memorySpaceID;

            fixed (T* buf = data)
            fixed (ulong* startPtr = start)
            fixed (ulong* countPtr = count)
            {
                status = H5S.select_hyperslab(dataSpaceID, H5S.seloper_t.SET, startPtr, null, countPtr, null);
                CheckAndThrow(status);

                memorySpaceID = H5S.create_simple(dataShape.Length, dataShape, null);
                CheckAndThrow(memorySpaceID);

                status = H5D.read(ID, dataType, memorySpaceID, dataSpaceID, H5P.DEFAULT, (IntPtr)buf);
                CheckAndThrow(status);

                status = H5S.close(memorySpaceID);
                CheckAndThrow(status);
            }
        }



        public void Close()
        {
            H5D.close(ID);
        }

        // test code to load all file properties
        private void Open()
        {
            var filePath = @"..\..\..\..\test.h5";
            var fileID = H5F.open(filePath, H5F.ACC_RDONLY);
            var dataset = "dsetname";

            // open file
            var datasetID = H5D.open(fileID, $"/{dataset}");

            // get data type
            var datatype = H5D.get_type(datasetID); // this looks like its returning the object type. as in Dataset or Group

            // get spaceid
            var spaceid = H5D.get_space(datasetID);

            // get shape of the data in the dataset
            var ndims = H5S.get_simple_extent_ndims(spaceid);
            ulong[] dims = new ulong[ndims];
            ulong[] maxdims = new ulong[ndims];
            H5S.get_simple_extent_dims(spaceid, dims, maxdims);

            // get the property list
            var plist = H5D.get_create_plist(datasetID);

            // get chunk shape
            ulong[] chunks = new ulong[maxdims.Length];
            H5P.get_chunk(plist, chunks.Length, chunks);

        }

        /// <summary>
        /// Opens an existing dataset
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="name"></param>
        public Dataset(long fileID, string name)
        {
            // open file
            ID = H5D.open(fileID, $"/{name}");
            CheckAndThrow(ID);

            // get spaceid
            spaceID = H5D.get_space(ID);
            CheckAndThrow(spaceID);

            // get shape of the data in the dataset
            var ndims = H5S.get_simple_extent_ndims(spaceID);
            Dimensions = new ulong[ndims];
            MaxDimensions = new ulong[ndims];
            var result = H5S.get_simple_extent_dims(spaceID,Dimensions,MaxDimensions);
            CheckAndThrow(result);

            // get the property list
            var plist = H5D.get_create_plist(ID);
            CheckAndThrow(plist);

            if (H5P.get_layout(plist) == H5D.layout_t.CHUNKED)
            {
                // get chunk shape
                Chunks = new ulong[MaxDimensions.Length];
                H5P.get_chunk(plist, Chunks.Length, Chunks);
            }
        }

        public ulong[] Dimensions { get; private set; }
        public ulong[] MaxDimensions { get; private set; }
        public ulong[] Chunks { get; private set; }

        private long spaceID;

    }   
}
