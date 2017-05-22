namespace IndoorNavigationApp.Models
{
    public sealed class Node
    {
        public int Id { get; }

        public string Title { get; }

        public string Description { get; }

        public BeaconData BeaconData { get; }

        public NodeType Type { get; }

        public PointU Position { get; }

        public int NavigationId { get; }

        public Node(int id, string title, string description, BeaconData beaconData, NodeType type, PointU position, int navigationId)
        {
            Id = id;
            Title = title;
            Description = description;
            BeaconData = beaconData;
            Type = type;
            Position = position;
            NavigationId = navigationId;
        }
    }
}
