using FireMapper.Attributes;

namespace App.Data
{
    [FireCollection("Cities")]
    public record Coordinates(
        [property:FireKey] string Token,
        double X,
        double Y)
    {}
}