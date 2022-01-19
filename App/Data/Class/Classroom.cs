using FireMapper.Attributes;

namespace App.Data
{
    [FireCollection("Students")]
    public record Classroom(
        [property:FireKey] int Id,
        string Something)
    {}
}