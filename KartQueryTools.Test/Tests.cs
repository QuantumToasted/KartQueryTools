using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace KartQueryTools.Test
{
    public class Tests
    {
        // Developer's note: These tests will fail if these servers cease to function.
        // These servers are fully functional and pass all tests as of August 16th, 2021.
        // Other servers can be tested from https://ms.kartkrew.org/.

        [Theory]
        [InlineData("73.176.15.98:5031",
                    "73.176.15.98:5032",
                    "73.176.15.98:5033",
                    "73.176.15.98:5034",
                    "157.245.170.147:5029")]
        public void ServerQuery_Success(params string[] addresses)
        {
            foreach (var address in addresses)
            {
                var server = KartQuery.QueryServer(IPEndPoint.Parse(address));

                Assert.NotNull(server);
                Assert.True(!string.IsNullOrWhiteSpace(server.Name));
                Assert.True(!string.IsNullOrWhiteSpace(server.Application));
                Assert.True(server.NumberOfFilesNeeded > 0); // SHOULD be > 0 for ALL SRB2Kart servers.
                Assert.True(server.MaxPlayerCount > 0); // SHOULD be > 0 for ALL SRB2Kart servers. Imagine a 0 player server.

                Assert.NotNull(server.CurrentMap);
                Assert.True(!string.IsNullOrWhiteSpace(server.CurrentMap.Title));
                Assert.True(!string.IsNullOrWhiteSpace(server.CurrentMap.InternalName));
                Assert.True(server.CurrentMap.MD5.Length == 16);
                Assert.True(server.CurrentMap.TimeElapsed > TimeSpan.Zero);
                
                Assert.True(server.CurrentPlayerCount == server.CurrentPlayers.Length);

                Assert.All(server.CurrentPlayers, player =>
                {
                    Assert.True(player.Node != 255);
                    Assert.True(!string.IsNullOrWhiteSpace(player.Name));
                    Assert.True(player.TimeInServer > TimeSpan.Zero);
                });

                Assert.All(server.NeededFiles, file =>
                {
                    Assert.True(!string.IsNullOrWhiteSpace(file.Filename));
                    Assert.True(file.Md5.Length == 16);
                });
            }
        }

        [Fact]
        public async Task ServerList_Success()
        {
            var servers = await KartQuery.QueryMasterServerAsync();

            Assert.All(servers, server =>
            {
                Assert.NotNull(server.Endpoint);
                Assert.True(server.Contact?.Equals(string.Empty) != true);
            });
        }
    }
}
