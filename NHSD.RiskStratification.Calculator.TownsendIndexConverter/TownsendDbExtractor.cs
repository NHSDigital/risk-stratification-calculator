using Npgsql;
using System.Collections.Generic;

namespace NHSD.RiskStratification.Calculator.TownsendIndexConverter
{
    public static class TownsendDbExtractor
    {
        public static IEnumerable<(byte[], double score)> ExtractTownsendIndex(string connectionString)
        {
            using var dbConnection = new NpgsqlConnection(connectionString);
            dbConnection.Open();

            var dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = "Select * from hashedpostcodelookup";
            var dbReader = dbCommand.ExecuteReader();

            var keyIndex = dbReader.GetOrdinal("hashedpostcode");
            var valueIndex = dbReader.GetOrdinal("transformedscore");

            while (dbReader.Read())
            {
                // Check length of bytes
                long len = dbReader.GetBytes(keyIndex, 0, null, 0, 0);

                // Create a buffer to hold the bytes, and then
                // read the bytes from the dbReader.
                byte[] indexHash = new byte[len];
                dbReader.GetBytes(keyIndex, 0, indexHash, 0, (int)len);

                var score = dbReader.GetDouble(valueIndex);

                yield return (indexHash, score);
            }
        }
    }
}
