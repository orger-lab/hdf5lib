using System;
using System.Collections.Generic;
using System.Text;

namespace hdf5lib
{
    internal static class Utils
    {

        public static void CheckAndThrow(long status)
        {
            if (status == -1)
            {
                throw new Exception();
            }
        }
    }
}
