using smartFan.Services.Interfaces;

namespace smartFan.Services
{
    public class RandomProvider : IRandomProvider
    {
        private readonly Random _random;

        public RandomProvider()
        {
            _random = new Random();
        }

        public double NextDouble()
        {
            return _random.NextDouble();
        }
    }
}