using HDF.PInvoke;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

using static hdf5lib.Utils;
using static System.Net.WebRequestMethods;


namespace hdf5lib
{
    public class H5DataSet : H5Object
    {
        private H5Collection<H5Attribute> Attributes;

        private long dataSpaceID;
        // TODO : understand how the plist is required to update parameters after the dataset is created or loaded
        private long propertyListID; 

        private int compressionFilter; // i dont think this is needed after the ctor if it cannot be changed
        private uint[] compressionOpts; // i dont think this is needed after the ctor if it cannot be changed

        public ulong[] Dimensions { get; private set; }
        public ulong[] MaxDimensions { get; private set; }
        public ulong[] Chunks { get; private set; }

        public Type DataType { get; private set; }

        /// <summary>
        /// internal representation of the underlying hdf5 format.
        /// </summary>
        private long H5TDataType;


        /// <summary>
        /// Opens an existing dataset
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="name"></param>
        public H5DataSet(long fileID, string name)
        {
            // open file
            ID = H5D.open(fileID, $"/{name}");
            CheckAndThrow(ID);

            // get spaceid
            dataSpaceID = H5D.get_space(ID);
            CheckAndThrow(dataSpaceID);

            // get shape of the data in the dataset
            var ndims = H5S.get_simple_extent_ndims(dataSpaceID);
            Dimensions = new ulong[ndims];
            MaxDimensions = new ulong[ndims];
            var result = H5S.get_simple_extent_dims(dataSpaceID, Dimensions, MaxDimensions);
            CheckAndThrow(result);

            // get the property list
            var plist = H5D.get_create_plist(ID);
            CheckAndThrow(plist);

            if (H5P.get_layout(plist) == H5D.layout_t.CHUNKED)
            {
                // get chunk shape
                Chunks = new ulong[MaxDimensions.Length];
                H5P.get_chunk(plist, Chunks.Length, Chunks);
                CheckAndThrow(result);
            }

            // set data types
            var typeHandle = H5D.get_type(ID);
            SetDataType(ConvertH5TToType(typeHandle));
            result = H5T.close(typeHandle);
            CheckAndThrow(result);


            // get attributes
            Attributes = H5Attribute.ExtractAll(ID);
        }

        public H5DataSet(string name, Type datatype, ulong[] dimensions, ulong[] chunks = null, int compressionFilter = 0, uint[] compressionOptions = null)
        {
            Name = name;
            SetDataType(datatype); // no fileid 
            CreateDataSpace(dimensions); // no fileid 
            CreatePropertyList(); // no fileid 
            if (chunks != null)
            {
                SetChunk(chunks);
                if (compressionFilter != 0 && compressionOptions != null) // TODO : convert to CompressionFilter object c#+ req
                {
                    SetCompression(compressionFilter, compressionOptions);
                }
            } // no fileid 
            //TODO : there is no path for when chunks is null but not the filter
        }

        


        private void SetDataType(Type dataType)
        {
            H5TDataType = Utils.ConvertTypeToH5T(dataType);
            DataType = dataType;
        }
        private void CreateDataSpace(ulong[] dimensions)
        {
            Dimensions = dimensions;
            MaxDimensions = dimensions;
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
            if (chunks.Length != Dimensions.Length)
            {
                throw new Exception($"{nameof(chunks)} must be the same overall shape as {nameof(Dimensions)}");
            }

            // Set chunks
            var result = H5P.set_chunk(propertyListID, chunks.Length, chunks);
            CheckAndThrow(result);

            Chunks = chunks;
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
            ID = H5D.create(fileID, name, H5TDataType, dataSpaceID, H5P.DEFAULT, propertyListID, H5P.DEFAULT);
            CheckAndThrow(ID);
        }


        public unsafe void Write(ulong[] start, ulong[] count, ulong[] dimsToWrite, IntPtr dataToWrite)
        {
            ValidateInputRank(start, count);
            int status;
            long memorySpaceID;

            fixed (ulong* startPtr = start)
            fixed (ulong* countPtr = count)
            {
                status = H5S.select_hyperslab(dataSpaceID, H5S.seloper_t.SET, startPtr, null, countPtr, null);
                CheckAndThrow(status);

                memorySpaceID = H5S.create_simple(dimsToWrite.Length, dimsToWrite, null);
                CheckAndThrow(memorySpaceID);

                status = H5D.write(ID, H5TDataType, memorySpaceID, dataSpaceID, H5P.DEFAULT, dataToWrite);
                CheckAndThrow(status);

                status = H5S.close(memorySpaceID);
                CheckAndThrow(status);
            }
        }

        public unsafe void Write<T>(ulong[] start, ulong[] count, ulong[] dimsToWrite, T[] dataToWrite) where T : unmanaged
        {
            ValidateInputRank(start, count);
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

                status = H5D.write(ID, H5TDataType, memorySpaceID, dataSpaceID, H5P.DEFAULT, (IntPtr)buf);
                CheckAndThrow(status);

                status = H5S.close(memorySpaceID);
                CheckAndThrow(status);
            }
        }

        public unsafe void Write(ulong[] start, ulong[] count, Array dataToWrite)
        {
            ValidateInputRank(start, count);

            var dimsToWrite = new ulong[dataToWrite.Rank];
            for (int i = 0; i < dimsToWrite.Length; i++)
            {
                dimsToWrite[i] = (ulong)dataToWrite.GetLength(i);
            }

            int status;
            long memorySpaceID;


            var handle = GCHandle.Alloc(dataToWrite, GCHandleType.Pinned);
            IntPtr dataPtr = handle.AddrOfPinnedObject();

            fixed (ulong* startPtr = start)
            fixed (ulong* countPtr = count)
            {
                status = H5S.select_hyperslab(dataSpaceID, H5S.seloper_t.SET, startPtr, null, countPtr, null);
                CheckAndThrow(status);

                memorySpaceID = H5S.create_simple(dimsToWrite.Length, dimsToWrite, null);
                CheckAndThrow(memorySpaceID);

                status = H5D.write(ID, H5TDataType, memorySpaceID, dataSpaceID, H5P.DEFAULT, dataPtr);
                CheckAndThrow(status);

                status = H5S.close(memorySpaceID);
                CheckAndThrow(status);
            }

            handle.Free();
        }




        /// <summary>
        /// Reads all data in the DataSet.
        /// </summary>
        /// <param name="data"> Pointer to the location where the data will be stored</param>
        /// <returns></returns>
        public void Read(IntPtr data)
        {
            var status = H5D.read(ID, H5TDataType, H5S.ALL, H5S.ALL, H5P.DEFAULT, data);
            CheckAndThrow(status);
        }

        /// <summary>
        /// Reads a section of a DataSet.
        /// </summary>
        /// <param name="start"> Indexes where to start reading from.</param>
        /// <param name="count"> Number of elements to read from each index.</param>
        /// <param name="dataShape">Final shape of the data</param>
        /// <param name="data"> Pointer to the location where the data will be stored</param>
        /// <returns></returns>
        public void Read(ulong[] start, ulong[] count, bool removeEmpty, IntPtr data)
        {
            ValidateInputRank(start, count);
            var dataShape = CalculateFinalDataShape(start, count, removeEmpty);


            unsafe
            {
                fixed (ulong* startPtr = start)
                fixed (ulong* countPtr = count)
                {
                    var status = H5S.select_hyperslab(dataSpaceID, H5S.seloper_t.SET, startPtr, null, countPtr, null);
                    CheckAndThrow(status);

                    var memorySpaceID = H5S.create_simple(dataShape.Length, dataShape, null);
                    CheckAndThrow(memorySpaceID);

                    status = H5D.read(ID, H5TDataType, memorySpaceID, dataSpaceID, H5P.DEFAULT, data);
                    CheckAndThrow(status);

                    status = H5S.close(memorySpaceID);
                    CheckAndThrow(status);
                }
            }
        }

        /// <summary>
        /// Reads all data in the DataSet.
        /// </summary>
        /// <returns></returns>
        public Array Read()
        {
            Array array = Array.CreateInstance(DataType, Array.ConvertAll(Dimensions, p => (int)p));

            var handle = GCHandle.Alloc(array, GCHandleType.Pinned);
            IntPtr buf = handle.AddrOfPinnedObject();

            var status = H5D.read(ID, H5TDataType, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf);
            CheckAndThrow(status);

            handle.Free();
            return array;
        }

        /// <summary>
        /// Reads a section of a DataSet.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="dataShape"></param>
        /// <returns></returns>
        public Array Read(ulong[] start, ulong[] count, bool removeEmpty)
        {
            ValidateInputRank(start, count);
            var dataShape = CalculateFinalDataShape(start, count, removeEmpty);

            Array array = Array.CreateInstance(DataType, Array.ConvertAll(dataShape, p => (int)p));

            var handle = GCHandle.Alloc(array, GCHandleType.Pinned);
            IntPtr buf = handle.AddrOfPinnedObject();

            unsafe
            {
                fixed (ulong* startPtr = start)
                fixed (ulong* countPtr = count)
                {
                    var status = H5S.select_hyperslab(dataSpaceID, H5S.seloper_t.SET, startPtr, null, countPtr, null);
                    CheckAndThrow(status);

                    var memorySpaceID = H5S.create_simple(dataShape.Length, dataShape, null);
                    CheckAndThrow(memorySpaceID);

                    status = H5D.read(ID, H5TDataType, memorySpaceID, dataSpaceID, H5P.DEFAULT, buf);
                    CheckAndThrow(status);

                    status = H5S.close(memorySpaceID);
                    CheckAndThrow(status);
                }
            }

            handle.Free();
            return array;
        }


        /// <summary>
        /// Helper function to validate if inputs to Read/Write functions.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ValidateInputRank(ulong[] start, ulong[] count)
        {
            if (start.Length != count.Length)
                throw new ArgumentException($"Input dimensions do not match. {nameof(start)} has rank {start.Length} and {nameof(count)} has rank {count.Length}");

            if (start.Length != Dimensions.Length)
                throw new ArgumentException($"Input dimensions do not match. {nameof(start)} has rank {start.Length} and DataSet dimensions has rank {Dimensions.Length}");

            if (count.Length != Dimensions.Length)
                throw new ArgumentException($"Input dimensions do not match. {nameof(count)} has rank {count.Length} and DataSet dimensions has rank {Dimensions.Length}");

            for (int i = 0; i < start.Length; i++)
            {
                if (start[i] + count[i] > MaxDimensions[i])
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Helper function to calculate the final data shape when reading data from a dataset.
        /// This function assumes the data shapes have already been validated.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="removeEmpty">Wheter or not remove empty dimensions.</param>
        /// <returns></returns>
        private ulong[] CalculateFinalDataShape(ulong[] start, ulong[] count, bool removeEmpty)
        {
            // TODO : check the behavoir when trying to extract a single element of a multidimentional array. I'm very confident finalShape will explode.
            ulong[] finalShape;

            if (removeEmpty)
            {
                var emptyDimensions = 0;
                for (int i = 0; i < count.Length; i++)
                {
                    if (count.Length == 1)
                        emptyDimensions++;
                }
                finalShape = new ulong[count.Length - emptyDimensions];

                int j = 0;
                for (int i = 0; i < count.Length; i++)
                {
                    if (count[i] != 1)
                    {
                        finalShape[j] = start[i] + count[i];
                        j++;
                    }
                }
            }
            else
            {
                finalShape = new ulong[start.Length];

                for (int i = 0; i < finalShape.Length; i++)
                {
                    finalShape[i] = start[i] + count[i];
                }
            }
            return finalShape;
        }



        public Array this[ulong[] start, ulong[] count]
        {
            get => Read(start,count,false);
            set => Write(start,count,value);  
        }


        // THIS WONT WORK ON .NET STANDARD
        //public Array this[params Range[] ranges]
        //{
        //    get => Read(start, count, false);
        //    set => Write(start, count, value);
        //}



        internal override void Create(long parentID)
        {
            if (ID == 0)
            {
                CreateDataset(parentID, Name);
                Attributes = new H5Collection<H5Attribute>(ID);
            } 
        }

        internal override void Close()
        {
            // TODO : Close all attributes
            H5P.close(propertyListID);
            H5S.close(dataSpaceID);
            H5D.close(ID);
        }
    }
}