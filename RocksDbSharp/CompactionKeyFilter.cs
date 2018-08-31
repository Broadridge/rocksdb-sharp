using System;

namespace RocksDbSharp
{
    public unsafe class CompactionKeyFilter : IDisposable
    {
        public delegate bool SourceFilter(Span<byte> key);

        // Field for properly GC collection
        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        private readonly Native.CompactionFilterDestructor destructorHolder;
        private readonly Native.CompactionFilterFilter filterHolder;
        private readonly Native.CompactionFilterName nameHolder;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

        public CompactionKeyFilter(string name, SourceFilter source)
        {
            this.destructorHolder = state => { };
            this.filterHolder = (IntPtr state, int level, IntPtr key, UIntPtr length, IntPtr value, UIntPtr valueLength,
                IntPtr newValue, UIntPtr newValueLength, out bool changed) =>
            {
                changed = false;
                var keySpan = new Span<byte>((void*) key, (int) length);
                return source(keySpan);
            };
            this.nameHolder = state => name;

            this.Handle = Native.Instance.rocksdb_compactionfilter_create(
                new IntPtr(), destructorHolder, filterHolder, nameHolder);
        }

        ~CompactionKeyFilter()
        {
            ReleaseUnmanagedResources();
        }

        public IntPtr Handle { get; private set; }
        
        public void SetIgnoreSnapshots(bool value)
        {
            Native.Instance.rocksdb_compactionfilter_set_ignore_snapshots(this.Handle, value);
        }

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
            Native.Instance.rocksdb_compactionfilter_destroy(this.Handle);
#endif
            this.Handle = IntPtr.Zero;
        }
    }
}
