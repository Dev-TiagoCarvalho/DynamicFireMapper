using FireMapper.Attributes;

namespace App.Data
{
    [FireCollection("Students")]
    public record Student(
        [property:FireKey] string Name,
        Classroom Classroom,
        int Number)
    {}
}