using FireMapper.Attributes;

namespace App.Data
{
    [FireCollection("PointsOfInterest")]
    public struct StructPointOfInterest
    {
        [FireKey] public string Name { get; }
        public StructCity City { get; }
        public string Type { get; }
        public int Evaluation { get; }
        public string Description { get; }

        public StructPointOfInterest(string name, StructCity city, string type, int evaluation, string description)
        {
            Name = name;
            City = city;
            Type = type;
            Evaluation = evaluation;
            Description = description;
        }
    }
}