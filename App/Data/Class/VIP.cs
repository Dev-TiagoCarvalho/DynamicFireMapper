using FireMapper.Attributes;

namespace App.Data
{
    [FireCollection("VIPs")]
    public record VIP(
        [property:FireKey] string Name,
        City City,
        string Job,
        string Birthday,
        string Description)
    {}
}