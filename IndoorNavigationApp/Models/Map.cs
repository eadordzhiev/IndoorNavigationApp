using System;
using System.Collections.Generic;
using IndoorNavigationApp.Service;
using JetBrains.Annotations;

namespace IndoorNavigationApp.Models
{
    public sealed class Map : IEquatable<Map>
    {
        public int Id { get; }

        [NotNull]
        public string Title { get; }

        public SizeU Size { get; }

        [NotNull]
        public IReadOnlyList<Node> Nodes { get; }
        
        public Uri LowResolutionMapUri { get; }
        
        [NotNull]
        public ITileProvider TileProvider { get; }
        
        public Map(int id, string title, SizeU size, IReadOnlyList<Node> nodes, Uri lowResolutionMapUri, ITileProvider tileProvider)
        {
            Id = id;
            Title = title;
            Size = size;
            Nodes = nodes;
            LowResolutionMapUri = lowResolutionMapUri;
            TileProvider = tileProvider;
        }

        public bool Equals(Map other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Map && Equals((Map) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(Map left, Map right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Map left, Map right)
        {
            return !Equals(left, right);
        }
    }
}