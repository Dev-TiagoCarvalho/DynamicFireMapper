using FireMapper.Attributes;

namespace App.Data
{
    [FireCollection("Cities")]
    public struct StructCity 
    {
        [FireKey] public string Name { get; }
        public StructCoordinates Coordinates { get; }
        public string Country { get; }
        public int Population { get; }
        public float Area { get; }
        public string TimeZone { get; }
        public string Description { get; }
        [FireIgnore] public int Ignore { get; }

        public StructCity(string name, StructCoordinates coordinates, string country, int population, float area, string timeZone, string description, int ignore)
        {
            Name = name;
            Coordinates = coordinates;
            Country = country;
            Population = population;
            Area = area;
            TimeZone = timeZone;
            Description = description;
            Ignore = ignore;
        }
    }
}