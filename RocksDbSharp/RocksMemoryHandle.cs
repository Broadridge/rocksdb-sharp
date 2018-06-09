using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace RocksDbSharp
{
    public struct RocksMemoryHandle : IDisposable
    {
        private IntPtr handle;
        private int length;

        public RocksMemoryHandle(IntPtr handle, int length)
        {
            this.handle = handle;
            this.length = length;
        }

        public unsafe ReadOnlySpan<byte> Span => new ReadOnlySpan<byte>((void *)this.handle, this.length);

        public void Dispose()
        {
            var oldHandle = Interlocked.Exchange(ref handle, IntPtr.Zero);

            if (oldHandle != IntPtr.Zero)
            {
                Native.Instance.rocksdb_free(oldHandle);
            }
        }
    }
}
