using HDF.PInvoke;

using System;
using System.Collections.Generic;
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
    public sealed class H5Collection<T> where T : H5Object
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
    }



    //internal abstract class H5O
    //{

    //}

    //internal sealed class H5OC<T> where T : H5O
    //{

    //}

    //internal interface IH5Iterable<T> where T : H5O
    //{
    //    /// <summary>
    //    /// Exposes a method to extract all elements of a particular type from a parent node.
    //    /// </summary>
    //    /// <param name="parentID"></param>
    //    /// <returns></returns>
    //    internal H5OC<T> ExtractAll2(long parentID);
    //}





    //internal class A : H5O, IH5Iterable<A>
    //{
    //    H5OC<A> IH5Iterable<A>.ExtractAll2(long parentID)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    internal H5OC<A> ExtractAll(long parentID)
    //    {
    //        return this.ExtractAll2(parentID);
    //    }
    //}



    //internal class B
    //{
    //    A a = new A();

    //    void foo()
    //    {

    //        H5OC<A> it = a.ExtractAll(0);
    //    }

    //}
}









