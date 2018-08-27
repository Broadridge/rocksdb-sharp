using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace RocksDbSharp
{
    public unsafe class LiveFiles : IDisposable, IReadOnlyCollection<SstFileMetaData>
    {
        private readonly SstFileMetaData[] data;
        
        public LiveFiles(IntPtr dbHandle)
        {
            this.Handle = Native.Instance.rocksdb_livefiles(dbHandle);

            var count = Native.Instance.rocksdb_livefiles_count(this.Handle);
            this.data = new SstFileMetaData[count];
            for (var i = 0; i < count; i++)
            {
                var name = Native.Instance.rocksdb_livefiles_name(this.Handle, i);
                var level = Native.Instance.rocksdb_livefiles_level(this.Handle, i);
                var size = Native.Instance.rocksdb_livefiles_size(this.Handle, i);
                var smallestKey = Native.Instance.rocksdb_livefiles_smallestkey(this.Handle, i, out var skSize);
                var largestKey = Native.Instance.rocksdb_livefiles_largestkey(this.Handle, i, out var lkSize);

                this.data[i] = new SstFileMetaData(
                    Marshal.PtrToStringAnsi(name),
                    level,
                    size,
                    new Memory<byte>(new Span<byte>((void*) smallestKey, (int) skSize).ToArray()),
                    new Memory<byte>(new Span<byte>((void*) largestKey, (int) lkSize).ToArray()));
            }
        }

        ~LiveFiles()
        {
            ReleaseUnmanagedResources();
        }

        public IntPtr Handle { get; private set; }

        public SstFileMetaData this[int i] => this.data[i];

        public int Count => data.Length;

        public IEnumerator<SstFileMetaData> GetEnumerator() => this.data.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseUnmanagedResources()
        {
            if (this.Handle == IntPtr.Zero)
            {
                return;
            }

#if !NODESTROY
            Native.Instance.rocksdb_livefiles_destroy(this.Handle);
#endif
            this.Handle = IntPtr.Zero;
        }
    }
}
