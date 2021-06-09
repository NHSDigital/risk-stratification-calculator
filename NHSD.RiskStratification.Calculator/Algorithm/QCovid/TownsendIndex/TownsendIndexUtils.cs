using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NHSD.RiskStratification.Calculator.Algorithm.QCovid.TownsendIndex
{
    public static class TownsendIndexUtils
    {
        // This is the 32 bytes of a SHA256 hash excluding the 1st byte used as the name of the index file
        private const int EntryKeyLengthBytes = 31;
        private const int EntryScoreLengthBytes = sizeof(double);
        private const int EntryLengthBytes = EntryKeyLengthBytes + EntryScoreLengthBytes;

        public static void WriteIndex(IEnumerable<(byte[] key, double score)> entries, Stream outputStream)
        {
            using var writer = new BinaryWriter(outputStream);

            var sortedEntries = entries.OrderBy(g => g.key, new ByteArrayKeyComparer());
            foreach (var (key, score) in entries)
            {
                if (key.Length != EntryKeyLengthBytes)
                {
                    throw new InvalidOperationException($"Unexpected key length of {key.Length + 1} bytes found. Expected all keys to be {EntryKeyLengthBytes + 1} bytes");
                }
                writer.Write(key);
                writer.Write(score);
            }
        }

        //Require a function to open the stream so that the disposable scope can be handled within this yielding IEnumerable method and not get prematurely disposed
        public static IEnumerable<(byte[] key, double score)> ReadIndex(Stream inputStream, long streamLength)
        {
            using var binaryReader = new BinaryReader(inputStream);

            var entries = CalculateEntryCount(streamLength);
            for (int i = 0; i < entries; i++)
            {
                var key = binaryReader.ReadBytes(EntryKeyLengthBytes);
                var score = binaryReader.ReadDouble();

                yield return (key, score);
            }
        }

        private static long CalculateEntryCount(long fileLength)
        {
            if (fileLength % EntryLengthBytes != 0)
            {
                throw new InvalidOperationException($"Unable to process bucket file due to unexpected file length [{fileLength}] (file length should be a multiple of a fixed length entries of {EntryLengthBytes} bytes)");
            }
            return fileLength / EntryLengthBytes;
        }
    }
}
