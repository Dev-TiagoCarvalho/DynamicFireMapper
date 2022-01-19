using FireMapper.Attributes;

namespace App.Data
{
    [FireCollection("VIPs")]
    public struct StructVIP
    {
        [FireKey] public string Name { get; }
        public StructCity City { get; }
        public string Job { get; }
        public string Birthday { get; }
        public string Description { get; }

        public StructVIP(string name, StructCity city, string job, string birthday, string description)
        {
            Name = name;
            City = city;
            Job = job;
            Birthday = birthday;
            Description = description;
        }
    }
}