using System;

namespace IndoorNavigationApp.Models
{
    public sealed class BeaconData : IEquatable<BeaconData>
    {
        public Guid Uuid { get; }

        public ushort Major { get; }

        public ushort Minor { get; }

        public BeaconData(Guid uuid, ushort major, ushort minor)
        {
            Uuid = uuid;
            Major = major;
            Minor = minor;
        }

        public bool Equals(BeaconData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Uuid.Equals(other.Uuid) && Major == other.Major && Minor == other.Minor;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is BeaconData && Equals((BeaconData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Uuid.GetHashCode();
                hashCode = (hashCode * 397) ^ Major.GetHashCode();
                hashCode = (hashCode * 397) ^ Minor.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(BeaconData left, BeaconData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BeaconData left, BeaconData right)
        {
            return !Equals(left, right);
        }
    }
}