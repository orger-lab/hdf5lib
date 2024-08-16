using System;

namespace hdf5lib
{
    public static class Utils
    {
        internal static void CheckAndThrow(long status,string message = "")
        {
            if (status == -1)
            {
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Converts a given index to the respective position in a multidimensional array.
        /// (Equivalent to MATLAB's ind2sub)
        /// </summary>
        /// <param name="dataShape">Number of elements in each dimension of the array</param>
        /// <param name="index">Index to retrieve</param>
        /// <returns></returns>
        internal static int[] ConvertIndexToSubscript(int[] dataShape, int index)
        {
            if (index < 0)
                throw new IndexOutOfRangeException($"{nameof(index)} must be greater than zero.");

            int numberOfElements = dataShape[0];
            for (int i = 1; i < dataShape.Length; i++)
                numberOfElements *= dataShape[i];
            if(index > numberOfElements)
                throw new IndexOutOfRangeException($"{nameof(index)} is outside the array.");


            int[] result = new int[dataShape.Length];
            // N-Dimensional case - process all dimensions except the first 2.
            if (dataShape.Length > 2)
            {
                var k = cumprod(dataShape);

                for (int i = dataShape.Length-1; i >= 2; i--)
                {
                    var vi = (index) % k[i-1];
                    var vj = (index - vi) / k[i-1];
                    result[i] = vj;
                    index = vi;
                }
            }
            // 2D case
            if (dataShape.Length >= 2)
            {
                result[0] = (index % dataShape[0]);
                result[1] = (index - result[0]) / dataShape[0];
            }
            else // 1D case
            {
                result[0] = index;
            }
            return result;
        }

        static int[] cumprod(int[] size)
        {
            int[] result = new int[size.Length];
            result[0]=size[0];
            for (int i = 1; i < result.Length; i++)
                result[i] = size[i] * result[i-1];
            return result;
        }
    }
}
