using System;

namespace RocksDbSharp
{
    public struct SstFileMetaData<TKey>
    {
        public readonly string Name;
        public readonly int Level;
        public readonly ulong Size;
        public readonly TKey SmallestKey;
        public readonly TKey LargestKey;

        public SstFileMetaData(
            string name, int level, ulong size, TKey smallestKey, TKey largestKey)
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
