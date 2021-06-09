using System;

namespace NHSD.RiskStratification.Calculator.TownsendIndexConverter
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("Expected 2 parameters to be provided: PostgreSQL connection string and bucket target directory path");
            }

            var connectionString = args[0];
            var outputDirectory = args[1];

            var townsendIndexes = TownsendDbExtractor.ExtractTownsendIndex(connectionString);
            BinaryBucketWriter.WriteBuckets(townsendIndexes, outputDirectory);
        }
    }
}
