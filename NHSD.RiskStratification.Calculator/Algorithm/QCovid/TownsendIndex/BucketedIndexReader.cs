using QCovid.RiskCalculator.Townsend;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.RiskStratification.Calculator.Algorithm.QCovid.TownsendIndex
{
    public class BucketedIndexReader : ITownsendScoreDbReader
    {
        private readonly ByteArrayKeyComparer _keyComparer = new ByteArrayKeyComparer();
        private readonly IDictionary<byte, IDictionary<byte[], double>> _loadedIndexes = new Dictionary<byte, IDictionary<byte[], double>>();
        private readonly ITownsendIndexReader _indexReader;
        private readonly ILogger _logger;

        public BucketedIndexReader(ITownsendIndexReader indexReader, ILogger logger)
        {
            _indexReader = indexReader;
            _logger = logger.ForContext<BucketedIndexReader>();
        }

        public double? GetValueByKey(byte[] key)
        {
            try
            {
                return TryGetIndex(key[0], out var bucket) && bucket.TryGetValue(key[1..], out var score)
                    ? (double?)score
                    : null;
            }
            catch (Exception e)
            {
                _logger.Error(e, "TownsendIndex lookup failed");
                throw;
            }
        }

        private bool TryGetIndex(byte indexKey, out IDictionary<byte[], double> index)
        {
            if (!_loadedIndexes.TryGetValue(indexKey, out index))
            {
                index = LoadIndex(indexKey);
                _loadedIndexes.Add(indexKey, index);
            }
            return index != null;
        }

        private IDictionary<byte[], double> LoadIndex(byte bucketKey)
        {
            var (indexStream, streamLength) = _indexReader.GetIndexStream(bucketKey);
            using (indexStream)
            {
                return TownsendIndexUtils.ReadIndex(indexStream, streamLength).ToDictionary(entry => entry.key, entry => entry.score, _keyComparer);
            }

        }
    }
}
