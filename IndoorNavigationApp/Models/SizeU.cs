using System;

namespace IndoorNavigationApp.Models
{
    public struct SizeU : IEquatable<SizeU>
    {
        public uint Width { get; }

        public uint Height { get; }

        public SizeU(uint width, uint height)
        {
            Width = width;
            Height = height;
        }

        public bool Equals(SizeU other)
        {
            return Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SizeU && Equals((SizeU) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Width * 397) ^ (int) Height;
            }
        }

        public static bool operator ==(SizeU left, SizeU right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SizeU left, SizeU right)
        {
            return !left.Equals(right);
        }
    }
}