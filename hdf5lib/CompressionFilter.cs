using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace hdf5lib
{


    /// <summary>
    /// Exposes the methods necessary to set a HDF5 compression filter.
    /// </summary>
    public interface ICompressionFilter
    {
        /// <summary>
        /// Filter ID
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Converts all filter paramenters to the standard format used by the HDF5 library
        /// </summary>
        /// <returns></returns>
        uint[] Serialize();

        /// <summary>
        /// Converts all filter paramenters from the standard format used by the HDF5 library to the respective variables.
        /// </summary>
        /// <returns></returns>
        void Deserialize(uint[] parameterArray);

    }



    /// <summary>
    /// No Compression.
    /// </summary>
    public class None : ICompressionFilter
    {
        public int ID => 0;

        public void Deserialize(uint[] parameterArray)
        {
        }

        public uint[] Serialize() => null;


    }




    /// <summary>
    /// GZIP Filter Compression.
    /// </summary>
    public class GZIP : ICompressionFilter
    {
        public int ID => 9;

        private uint _level = 4;
        public uint Level
        {
            get => _level;
            set
            {
                if (value >= 0 && value < 9)
                {
                    _level = value;
                }
            }
        }
        public uint[] Serialize() => new uint[] { Level };

        public void Deserialize(uint[] parameterArray)
        {
            if (parameterArray.Length != 1)
                throw new ArgumentException("Number of elements in the array does not match the requierements");

            Level = parameterArray[0];
        }
    }

    public class B5D : ICompressionFilter
    {
        public int ID => 306;

        public uint[] Serialize() => new uint[] { Quantization * 1000, Mode, CameraConversion * 1000, CameraOffset, ReadNoise * 1000, TileSize };

        public void Deserialize(uint[] parameterArray)
        {
            if (parameterArray.Length != 6)
                throw new ArgumentException("Number of elements in the array does not match the requierements");

            Quantization = parameterArray[0] / 1000;
            Mode = parameterArray[1];
            CameraConversion = parameterArray[2] / 1000;
            CameraOffset = parameterArray[3];
            ReadNoise = parameterArray[4] / 1000;
            TileSize = parameterArray[5];
        }


        private uint _quantization = 0;
        /// <summary>
        /// Quantization step relative to sigma. Set to 0 for lossless compression.
        /// </summary>
        public uint Quantization
        {
            get => _quantization;
            set
            {
                if (value >= 0 && value < 9)
                {
                    _quantization = value;
                }
            }
        }

        private uint _mode = 1;
        /// <summary>
        /// Compression mode. (see paper for details)
        /// </summary>
        public uint Mode
        {
            get => _mode;
            set
            {
                if (value >= 1 && value <= 2)
                {
                    _mode = value;
                }
            }
        }

        private uint _cameraConversion = 1000;
        /// <summary>
        /// Camera conversion parameter in DN/e-.
        /// </summary>
        public uint CameraConversion
        {
            get => _cameraConversion;
            set
            {
                if (value >= 0 && value < 9)
                {
                    _cameraConversion = value;
                }
            }
        }

        private uint _cameraOffset = 0;
        /// <summary>
        /// Camera background offset parameter in DN.
        /// </summary>
        public uint CameraOffset
        {
            get => _cameraOffset;
            set
            {
                if (value >= 0 && value < 9)
                {
                    _cameraOffset = value;
                }
            }
        }

        /// <summary>
        /// Camera read noise parameter in e-.
        /// </summary>
        private uint _readNoise = 0;
        public uint ReadNoise
        {
            get => _readNoise;
            set
            {
                if (value >= 0 && value < 9)
                {
                    _readNoise = value;
                }
            }
        }

        private uint _tileSize = 24;
        /// <summary>
        /// Tile size, to optimize parallel execution.
        /// </summary>
        public uint TileSize
        {
            get => _tileSize;
            set
            {
                if (value >= 0 && value < 9)
                {
                    _tileSize = value;
                }
            }
        }

    }
}
