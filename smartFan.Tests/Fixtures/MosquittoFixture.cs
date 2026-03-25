using Testcontainers.Mosquitto;
using Xunit;
using System.Threading.Tasks;

namespace smartFan.Tests.Fixtures
{
    public class MosquittoFixture : IAsyncLifetime
    {
        public MosquittoContainer Container { get; } = new MosquittoBuilder()
            .WithImage("eclipse-mosquitto:latest")
            .Build();

        public async Task InitializeAsync() => await Container.StartAsync();

        public async Task DisposeAsync() => await Container.DisposeAsync();

        public string Host => Container.Hostname;
        public ushort Port => Container.GetMappedPublicPort(1883);
    }
}
