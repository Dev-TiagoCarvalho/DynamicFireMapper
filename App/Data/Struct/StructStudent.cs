using FireMapper.Attributes;

namespace App.Data
{
    [FireCollection("Students")]
    public struct StructStudent
    {
        public string Name { get; }
        [FireKey] public StructClassroom Classroom { get; }
        public int Number { get; }

        public StructStudent(string name, StructClassroom coordinates, int number)
        {
            Name = name;
            Classroom = coordinates;
            Number = number;
        }
    }
}