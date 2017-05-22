using System;
using JetBrains.Annotations;

namespace IndoorNavigationApp.Models
{
    public class RouteSegment : IEquatable<RouteSegment>
    {
        [NotNull]
        public Node StartingNode { get; }

        [NotNull]
        public Node EndingNode { get; }

        public RouteSegment([NotNull] Node startingNode, [NotNull] Node endingNode)
        {
            StartingNode = startingNode;
            EndingNode = endingNode;
        }

        public bool Equals(RouteSegment other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return StartingNode.Equals(other.StartingNode) && EndingNode.Equals(other.EndingNode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RouteSegment) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StartingNode.GetHashCode() * 397) ^ EndingNode.GetHashCode();
            }
        }

        public static bool operator ==(RouteSegment left, RouteSegment right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RouteSegment left, RouteSegment right)
        {
            return !Equals(left, right);
        }
    }
}