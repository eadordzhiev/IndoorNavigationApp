namespace IndoorNavigationApp.Models
{
    public sealed class Address
    {
        public string Country { get; }

        public string City { get; }

        public string Street { get; }

        public string House { get; }

        public Address(string country, string city, string street, string house)
        {
            Country = country;
            City = city;
            Street = street;
            House = house;
        }
    }
}