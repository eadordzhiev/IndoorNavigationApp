using System;
using System.Collections.Generic;

namespace IndoorNavigationApp.Models
{
    public sealed class Building
    {
        public int Id { get; }

        public string Name { get; }

        public Uri HeroImageUri { get; }

        public IReadOnlyList<Map> Maps { get; }

        public Address Address { get; }

        public Building(int id, string name, Uri heroImageUri, IReadOnlyList<Map> maps, Address address)
        {
            Id = id;
            Name = name;
            HeroImageUri = heroImageUri;
            Maps = maps;
            Address = address;
        }
    }
}