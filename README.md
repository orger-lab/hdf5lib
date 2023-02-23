# hdf5lib

## Files
### Creating Files

Existing files can be opened as read/write, or as
read only. New files can be created; if a *new* file
already exists, it can either be overwritten, or the
operation can fail, leaving the file unaltered.

Initialize a file

```c#
string filePath = @"C:\Some\Path\Here\fileToSave.h5"
// Create a new file, fail to proceed if file already exists
H5File myFile = H5File(filePath, FileCreationMode.Fail); 

```

Once a file is opened, new features can be added to the file.
These features include groups, datasets, and attributes.
+ Groups do not contain data, but help to organize other features, including other groups and/or datasets. 
+ Datasets contain data of a particular type; they can be any size and essentially any number of dimensions. They cannot (easily) mix types within onedataset, eg you can't have an array that contains integers andfloats. 
+ Attributes are metadata that can be applied to the file, to a group, or to a dataset. These are generally small bits of data that should not change.

**Note that at the moment groups are not implemented.**

## Datasets
### Adding Datasets to a File

#### Minimum requirements
Each file has a Datasets attribute that can be used
to add new datasets or access existing datasets.
To add a new dataset, at a minimum, you need to 
know the final dimensions of the dataset, the type
of the elements of the dataset, and the name you
want for the dataset.

In our example we will have a 5x5x5 matrix of unsigned
16bit integers, similar to the kind of images we often
get from microscopes.

```c#
ulong x = 5;
ulong y = 5;
ulong z = 5;

ulong[] dims = new ulong[]{ x, y, z };
var dtype = typeof(UInt16);
string datasetName = "testDataset";
``` 

Note that for now, you must pass the type by calling
__typeof()__ and then passing the type as an argument.

```c#
myFile.Datasets.Add(new H5DataSet(datasetName, dtype, dims));
```

You can add an arbitrary number of datasets to each file,
and each dataset can have different dimensions, types, etc.

#### Advanced features
HDF5 datasets support internal filtering of datasets with 
various algorithms, eg zipping them for lossless compression,
or fancier filters like B5D for lossy compression.

One requirement when using filters is that the dataset is
'chunked.' 

In our example we will use chunk sizes that
correspond to a single row of our matrix.

```c#
ulong[] chunks = new ulong[] { x, 1, 1 }; // chunks must have the same shape as the dimensions of the array, but not necessarily the same size in each dimension
int filter = 306;
uint[] compressionOptions = new uint[]{ 4000 };

myFile.Datasets.Add(
	new H5DataSet(datasetName, dtype, dims, 
		chunks, filter, compressionOptions)
	);
```

Note that any dataset can be chunked, but datasets with filters
must be chunked.
Filters are given an ID as an integer value, and filters
will usually also need an array of compression options
that are passed to the filter. 

### Writing to Datasets
When writing to the dataset, you will need to keep a few things
in mind.
+ Where in the matrix you want to **start** writing
+ How many elements do you want to **count** into the matrix

If we want to add a single frame of our matrix 
(the first 25 values) to the dataset
we would generate our start and count like this:
```c#
ulong[] start = new ulong[] { 0, 0, 0 }; // start is "0-indexed"
ulong[] count = new ulong[] { x, y, 1 }; // count is "1-indexed"
```

Writing is as simple as:
```c#
UInt16[] data = { 1, 1, 1, 1, 1, 
				   2, 2, 2, 2, 2, 
				   3, 3, 3, 3, 3, 
				   4, 4, 4, 4, 4, 
				   5, 5, 5, 5, 5 };
myFile["testDataset"][start, count] = data;
```

Writing to the next frame requires changing the start value:

```c#
ulong[] start = new ulong[] { 0, 0, 1 }; // start is "0-indexed"

myFile["testDataset"][start, count] = data;
```

And you can easily turn this into a loop:
```c#
ulong[] start;
ulong[] count = new ulong[] { x, y, 1 }; // count is "1-indexed"

for (int i = 0; i < (int)z; i++)
{
	start = new ulong[] { 0, 0, i };
	myFile["testDataset"][start, count] = data;
}
```

Note that the start and the count can be much 
more complex than this, but this is a typical
usecase when working with images off of a camera.

### Attributes
#### Writing attributes
Adding attributes is similar to adding datasets:

```c#
myFile.Attributes.Add(new H5Attribute("fileAttr", 1)); // attribute on a file
myFile["testDataset"].Attributes.Add(
	new H5Attribute("pixels", new float[] { 0.8f, 0.8f, 1.0f })
	); // attribute on a dataset
```
Attributes can be added directly to the file, or they can
be added to individual datasets.

### Closing Files
Closing the connection to the file is easy:

```c#
myFile.Close();
```

This closes all dataset open as well as the file itself.