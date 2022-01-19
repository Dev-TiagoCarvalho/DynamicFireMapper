using FireMapper.Attributes;

namespace App.Data
{
    [FireCollection("Cities")]
    public struct StructCoordinates
    {
        [FireKey] public string Token { get; }
        public double X { get; }
        public double Y { get; }

        public StructCoordinates(string token, double x, double y)
        {
            Token = token;
            X = x;
            Y = y;
        }
    }
}