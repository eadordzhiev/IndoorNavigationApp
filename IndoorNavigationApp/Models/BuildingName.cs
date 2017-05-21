namespace IndoorNavigationApp.Models
{
    public sealed class BuildingName
    {
        public int Id { get; }

        public string Name { get; }

        public BuildingName(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
