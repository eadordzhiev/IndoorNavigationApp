using System;

namespace IndoorNavigationApp.Models
{
    public struct PointU : IEquatable<PointU>
    {
        public uint X { get; }

        public uint Y { get; }

        public PointU(uint x, uint y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(PointU other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PointU && Equals((PointU) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) X * 397) ^ (int) Y;
            }
        }

        public static bool operator ==(PointU left, PointU right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PointU left, PointU right)
        {
            return !left.Equals(right);
        }
    }
}