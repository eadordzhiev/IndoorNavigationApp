using System;
using IndoorNavigationApp.Models;
using JetBrains.Annotations;

namespace IndoorNavigationApp.Beacon
{
    public sealed class BeaconGeolocation : IEquatable<BeaconGeolocation>
    {
        [NotNull]
        public Map Map { get; }

        [NotNull]
        public Node NearestNode { get; }

        [NotNull]
        public BeaconData NearestBeacon { get; }

        public BeaconGeolocation(Map map, Node nearestNode, BeaconData nearestBeacon)
        {
            Map = map;
            NearestNode = nearestNode;
            NearestBeacon = nearestBeacon;
        }

        public bool Equals(BeaconGeolocation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(NearestNode, other.NearestNode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is BeaconGeolocation && Equals((BeaconGeolocation) obj);
        }

        public override int GetHashCode()
        {
            return NearestNode.GetHashCode();
        }

        public static bool operator ==(BeaconGeolocation left, BeaconGeolocation right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BeaconGeolocation left, BeaconGeolocation right)
        {
            return !Equals(left, right);
        }
    }
}
