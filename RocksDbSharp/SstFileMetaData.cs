using System;

namespace RocksDbSharp
{
    public struct SstFileMetaData
    {
        public readonly string Name;
        public readonly int Level;
        public readonly ulong Size;
        public readonly Memory<byte> SmallestKey;
        public readonly Memory<byte> LargestKey;

        public SstFileMetaData(
            string name, int level, ulong size, Memory<byte> smallestKey, Memory<byte> largestKey)
        {
            Name = name;
            Level = level;
            Size = size;
            SmallestKey = smallestKey;
            LargestKey = largestKey;
        }

        public override string ToString() => this.Name;
    }
}
