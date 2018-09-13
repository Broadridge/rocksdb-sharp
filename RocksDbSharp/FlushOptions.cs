using System;

namespace RocksDbSharp
{
    public class FlushOptions : IDisposable
    {
        public FlushOptions()
        {
            this.Handle = Native.Instance.rocksdb_flushoptions_create();
        }

        ~FlushOptions()
        {
            ReleaseUnmanagedResources();
        }

        public IntPtr Handle { get; private set; }

        public void SetWait(byte value) => Native.Instance.rocksdb_flushoptions_set_wait(this.Handle, value);

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
            Native.Instance.rocksdb_flushoptions_destroy(this.Handle);
#endif
            this.Handle = IntPtr.Zero;
        }
    }
}
