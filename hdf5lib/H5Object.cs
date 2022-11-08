using HDF.PInvoke;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Text;

namespace hdf5lib
{
    /// <summary>
    /// Base class for all HDF5 objects.
    /// </summary>
    public abstract class H5Object
    {
        public string Name { get; set; }

        protected internal long ID { get; protected set; } = 0;

        /// <summary>
        /// Creates the HDF5 object and links it to its parent object.
        /// </summary>
        /// <param name="parentID"></param>
        internal abstract void Create(long parentID);
        /// <summary>
        /// Closes the HDF5 Object.
        /// </summary>
        internal abstract void Close();
    }

    /// <summary>
    /// Represents a collection of <see cref="H5Object"/> with controlled access.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class H5Collection<T> : IEnumerable<T> where T : H5Object 
    {
        // TODO : this needs to implement IENUMEBRABLE
        Dictionary<string, T> dictonary;
        long parentID;

        internal H5Collection(long parentID)
        {
            dictonary = new Dictionary<string, T>();
            this.parentID = parentID;
        }

        public T this[string name]
        {
            get { return dictonary[name]; }
            set { }
        }

        /// <summary>
        /// Adds a new element to the collection.
        /// </summary>
        /// <param name="newItem"></param>
        public void Add(T newItem)
        {
            newItem.Create(parentID);
            dictonary.Add(newItem.Name, newItem);
        }



        internal void Close()
        {
            foreach(T obj in dictonary.Values)
                obj.Close();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictonary.Values.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return dictonary.Values.GetEnumerator();
        }
    }
}









