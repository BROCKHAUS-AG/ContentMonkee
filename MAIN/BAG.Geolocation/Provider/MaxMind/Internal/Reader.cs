namespace BAG.Geolocation.Provider.MaxMind.Internal
{
    using System;
    using System.IO;
    using System.IO.MemoryMappedFiles;
    using System.Linq;
    using System.Net;
    using System.Threading;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// An enumeration specifying the API to use to read the database
    /// </summary>
    public enum FileAccessMode
    {
        /// <summary>
        /// Open the file in memory mapped mode. Does not load into real memory.
        /// </summary>
        MemoryMapped,
        /// <summary>
        /// Load the file into memory.
        /// </summary>
        Memory
    }

    /// <summary>
    /// Given a MaxMind DB file, this class will retrieve information about an IP address
    /// </summary>
    public class Reader : IDisposable
    {
        /// <summary>
        /// The metadata for the open database.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        public Metadata Metadata { get; private set; }

        private const int DataSectionSeparatorSize = 16;

        private readonly byte[] _metadataStartMarker = { 0xAB, 0xCD, 0xEF, 77, 97, 120, 77, 105, 110, 100, 46, 99, 111, 109 };

        private readonly string _fileName;

        private int _fileSize;

        private readonly MemoryMappedFile _memoryMappedFile;

        private int _ipV4Start = 0;
        private int IPv4Start
        {
            get
            {
                if (this._ipV4Start != 0 || this.Metadata.IPVersion == 4)
                {
                    return this._ipV4Start;
                }
                int node = 0;
                for (int i = 0; i < 96 && node < this.Metadata.NodeCount; i++)
                {
                    node = this.ReadNode(node, 0);
                }
                this._ipV4Start = node;
                return node;
            }
        }

        private static readonly Object FileLocker = new Object();

        private readonly ThreadLocal<Stream> _stream;

        private Decoder Decoder { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="Reader"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        public Reader(string file) : this(file, FileAccessMode.MemoryMapped) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Reader"/> class.
        /// </summary>
        /// <param name="file">The MaxMind DB file.</param>
        /// <param name="mode">The mode by which to access the DB file.</param>
        public Reader(string file, FileAccessMode mode)
        {
            //if (file[0] > 1000)
            //{
            //    file = file.Substring(2);
            //}

            file = file.Sanitize();
            file = file.ToPhysicalPath();

            this._fileName = file;
            if (mode == FileAccessMode.MemoryMapped)
            {
                var fileInfo = new FileInfo(file);
                var mmfName = fileInfo.FullName.Replace("C:\\", "").Replace("\\", "-");
                lock (FileLocker)
                {
                    try
                    {
                        this._memoryMappedFile = MemoryMappedFile.OpenExisting(mmfName, MemoryMappedFileRights.Read);
                    }
                    catch (Exception ex)
                    {
                        if (ex is IOException || ex is NotImplementedException)
                        {
                            this._memoryMappedFile = MemoryMappedFile.CreateFromFile(this._fileName, FileMode.Open,
                                mmfName, fileInfo.Length, MemoryMappedFileAccess.Read);
                        }
                        else
                            throw;
                    }
                }
            }

            if (mode == FileAccessMode.Memory)
            {
                byte[] fileBytes = File.ReadAllBytes(this._fileName);
                this._stream = new ThreadLocal<Stream>(() => new MemoryStream(fileBytes, false));
            }
            else
            {
                this._stream = new ThreadLocal<Stream>(() =>
                {
                    var fileLength = (int)new FileInfo(file).Length;
                    return this._memoryMappedFile.CreateViewStream(0, fileLength, MemoryMappedFileAccess.Read);
                });
            }

            this.InitMetaData();
        }

        /// <summary>
        /// Initialize with Stream.
        /// </summary>
        /// <param name="stream">The stream to use. It will be used from its current position. </param>
        public Reader(Stream stream)
        {
            byte[] fileBytes = null;

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            if (fileBytes.Length == 0)
            {
                throw new InvalidDatabaseException("There are zero bytes left in the stream. Perhaps you need to reset the stream's position.");
            }

            this._stream = new ThreadLocal<Stream>(() => new MemoryStream(fileBytes, false));
            this.InitMetaData();
        }

        private void InitMetaData()
        {
            var start = this.FindMetadataStart();
            var metaDecode = new Decoder(this._stream, start);
            var result = metaDecode.Decode(start);
            this.Metadata = this.Deserialize<Metadata>(result.Node);
            this.Decoder = new Decoder(this._stream, this.Metadata.SearchTreeSize + DataSectionSeparatorSize);
        }

        /// <summary>
        /// Finds the data related to the specified address.
        /// </summary>
        /// <param name="ipAddress">The IP address.</param>
        /// <returns>An object containing the IP related data</returns>
        public JToken Find(string ipAddress)
        {
            return this.Find(IPAddress.Parse(ipAddress));
        }

        /// <summary>
        /// Finds the data related to the specified address.
        /// </summary>
        /// <param name="ipAddress">The IP address.</param>
        /// <returns>An object containing the IP related data</returns>
        public JToken Find(IPAddress ipAddress)
        {
            var pointer = this.FindAddressInTree(ipAddress);
            return pointer == 0 ? null : this.ResolveDataPointer(pointer);
        }

        private JToken ResolveDataPointer(int pointer)
        {
            var resolved = (pointer - this.Metadata.NodeCount) + this.Metadata.SearchTreeSize;

            if (resolved >= this._stream.Value.Length)
            {
                throw new InvalidDatabaseException(
                        "The MaxMind Db file's search tree is corrupt: "
                                + "contains pointer larger than the database.");
            }

            return this.Decoder.Decode(resolved).Node;
        }

        private int FindAddressInTree(IPAddress address)
        {
            byte[] rawAddress = address.GetAddressBytes();

            int bitLength = rawAddress.Length * 8;
            int record = this.StartNode(bitLength);

            for (int i = 0; i < bitLength; i++)
            {
                if (record >= this.Metadata.NodeCount)
                {
                    break;
                }
                byte b = rawAddress[i / 8];
                int bit = 1 & (b >> 7 - (i % 8));
                record = this.ReadNode(record, bit);
            }
            if (record == this.Metadata.NodeCount)
            {
                // record is empty
                return 0;
            }
            else if (record > this.Metadata.NodeCount)
            {
                // record is a data pointer
                return record;
            }
            throw new InvalidDatabaseException("Something bad happened");
        }

        private int StartNode(int bitLength)
        {
            // Check if we are looking up an IPv4 address in an IPv6 tree. If this
            // is the case, we can skip over the first 96 nodes.
            if (this.Metadata.IPVersion == 6 && bitLength == 32)
            {
                return this.IPv4Start;
            }
            // The first node of the tree is always node 0, at the beginning of the
            // value
            return 0;
        }

        private T Deserialize<T>(JToken value)
        {
            var serializer = new JsonSerializer();
            return serializer.Deserialize<T>(new JTokenReader(value));
        }

        private int FindMetadataStart()
        {
            this._fileSize = (int)this._stream.Value.Length;
            var buffer = new byte[this._metadataStartMarker.Length];

            for (int i = (this._fileSize - this._metadataStartMarker.Length); i > 0; i--)
            {
                this._stream.Value.Seek(i, SeekOrigin.Begin);
                this._stream.Value.Read(buffer, 0, buffer.Length);

                if (!buffer.SequenceEqual(this._metadataStartMarker))
                    continue;

                return i + this._metadataStartMarker.Length;
            }

            throw new InvalidDatabaseException(
                    "Could not find a MaxMind Db metadata marker in this file ("
                            + this._fileName + "). Is this a valid MaxMind Db file?");
        }

        private int ReadNode(int nodeNumber, int index)
        {
            var baseOffset = nodeNumber * this.Metadata.NodeByteSize;

            var size = this.Metadata.RecordSize;

            switch (size)
            {
                case 24:
                {
                    byte[] buffer = this.ReadMany(baseOffset + index * 3, 3);
                    return Decoder.DecodeInteger(buffer);
                }
                case 28:
                {
                    byte middle = this.ReadOne(baseOffset + 3);
                    middle = (index == 0) ? (byte)(middle >> 4) : (byte)(0x0F & middle);

                    byte[] buffer = this.ReadMany(baseOffset + index * 4, 3);
                    return Decoder.DecodeInteger(middle, buffer);
                }
                case 32:
                {
                    byte[] buffer = this.ReadMany(baseOffset + index * 4, 4);
                    return Decoder.DecodeInteger(buffer);
                }
            }

            throw new InvalidDatabaseException("Unknown record size: "
                    + size);
        }

        private byte ReadOne(int position)
        {
            this._stream.Value.Seek(position, SeekOrigin.Begin);
            return (byte)this._stream.Value.ReadByte();
        }

        private byte[] ReadMany(int position, int size)
        {
            var buffer = new byte[size];
            this._stream.Value.Seek(position, SeekOrigin.Begin);
            this._stream.Value.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// Release resources back to the system.
        /// </summary>
        public void Dispose()
        {
            this._stream.Dispose();
            if (this._memoryMappedFile != null)
                this._memoryMappedFile.Dispose();
        }
    }
}
