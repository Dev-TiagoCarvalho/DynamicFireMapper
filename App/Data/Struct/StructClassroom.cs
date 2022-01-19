using FireMapper.Attributes;

namespace App.Data
{
    [FireCollection("Students")]
    public struct StructClassroom
    {
        [FireKey] public int Id { get; }
        public string Something { get; }

        public StructClassroom(int id, string something)
        {
            Id = id;
            Something = something;
        }
    }
}