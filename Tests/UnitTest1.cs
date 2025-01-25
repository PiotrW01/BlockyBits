
using BlockyBitsClient.src;
using Microsoft.Xna.Framework;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Vector2 result = Utils.WorldToChunkCoord(new Vector3(0, 0, 0));
            Assert.True(result.X == 0);

            result = Utils.WorldToChunkCoord(new Vector3(0.2f, 0, 0));
            Assert.True(result.X == 0);

            result = Utils.WorldToChunkCoord(new Vector3(15.5f, 0, 0));
            Assert.True(result.X == 0);

            result = Utils.WorldToChunkCoord(new Vector3(16f, 0, 0));
            Assert.True(result.X == 1);

            result = Utils.WorldToChunkCoord(new Vector3(16.1f, 0, 0));
            Assert.True(result.X == 1);

            result = Utils.WorldToChunkCoord(new Vector3(-0.5f, 0, 0));
            Assert.True(result.X == -1);

            result = Utils.WorldToChunkCoord(new Vector3(-15.5f, 0, 0));
            Assert.True(result.X == -1);

            result = Utils.WorldToChunkCoord(new Vector3(-16f, 0, 0));
            Assert.True(result.X == -1);

            result = Utils.WorldToChunkCoord(new Vector3(-16.1f, 0, 0));
            Assert.True(result.X == -2);
        }
    }
}