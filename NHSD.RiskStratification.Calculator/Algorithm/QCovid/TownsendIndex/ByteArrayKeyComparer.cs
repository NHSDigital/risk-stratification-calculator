using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NHSD.RiskStratification.Calculator.Algorithm.QCovid.TownsendIndex
{
    public class ByteArrayKeyComparer : EqualityComparer<byte[]>, IComparer<byte[]>
    {
        public int Compare([NotNull] byte[] x, [NotNull] byte[] y)
        {
            if (x.Length != y.Length)
            {
                throw new InvalidOperationException("ByteArrayKeys must be of equal length");
            }
            return x.AsSpan().SequenceCompareTo(y.AsSpan());
        }

        public override bool Equals([AllowNull] byte[] x, [AllowNull] byte[] y)
        {
            if (x == null || y == null)
            {
                return x == y;
            }
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (x.Length != y.Length)
            {
                return false;
            }
            return x.SequenceEqual(y);
        }

        public override int GetHashCode([DisallowNull] byte[] obj)
        {
            return BitConverter.ToInt32(obj, 0); // Quick hashcode, taking int from the first 4 bytes to reduce collisions but faster than doing an proper Equals;
        }
    }
}
