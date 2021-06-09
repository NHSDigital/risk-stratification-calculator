using Amazon;
using Amazon.S3.Transfer;
using NHSD.RiskStratification.Calculator.Infrastructure.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace NHSD.RiskStratification.Calculator.Algorithm.QCovid.TownsendIndex
{
    public class S3TownsendIndexReader : ITownsendIndexReader
    {
        private readonly QCovidS3TownsendIndexConfiguration _configuration;

        public S3TownsendIndexReader(QCovidS3TownsendIndexConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (Stream stream, long length) GetIndexStream(byte indexKey)
        {
            var objectPath = PathUtils.CombineAwsPath(_configuration.Path, indexKey.ByteString());

            var transfer = new TransferUtility(RegionEndpoint.GetBySystemName(_configuration.Region));

            var lengthTask = transfer.S3Client.GetObjectMetadataAsync(_configuration.Bucket, objectPath);
            var streamTask = transfer.OpenStreamAsync(_configuration.Bucket, objectPath);

            Task.WaitAll(lengthTask, streamTask);

            return (streamTask.Result, lengthTask.Result.ContentLength);
        }
    }
}
