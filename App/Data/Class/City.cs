using FireMapper.Attributes;

namespace App.Data
{
    [FireCollection("Cities")]
    public record City(
        [property:FireKey] string Name,
        Coordinates Coordinates,
        string Country,
        int Population,
        float Area,
        string TimeZone,
        string Description,
        [property:FireIgnore] int Ignore)
    {}
}