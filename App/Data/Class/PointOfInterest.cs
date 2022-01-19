using FireMapper.Attributes;

namespace App.Data
{
    [FireCollection("PointsOfInterest")]
    public record PointOfInterest(
        [property:FireKey] string Name,
        City City,
        string Type,
        int Evaluation,
        string Description
        )
    {}
}