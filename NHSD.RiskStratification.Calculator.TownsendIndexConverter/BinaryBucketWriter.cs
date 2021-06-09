using NHSD.RiskStratification.Calculator.Algorithm.QCovid.TownsendIndex;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NHSD.RiskStratification.Calculator.TownsendIndexConverter
{
    public class BinaryBucketWriter
    {
        public static void WriteBuckets(IEnumerable<(byte[] key, double score)> index, string bucketPath)
        {
            Directory.CreateDirectory(bucketPath);

            var buckets = index.GroupBy(entry => entry.key[0], entry => (entry.key[1..], entry.score));

            foreach (var bucket in buckets)
            {
                WriteBucket(bucketPath, bucket.Key, bucket);
            }
        }

        private static void WriteBucket(string bucketPath, byte bucketKey, IEnumerable<(byte[] key, double score)> entries)
        {
            using var fileStream = File.Create(Path.Combine(bucketPath, bucketKey.ByteString()));
            TownsendIndexUtils.WriteIndex(entries, fileStream);
        }
    }
}
