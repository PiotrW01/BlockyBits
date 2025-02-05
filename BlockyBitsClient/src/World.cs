using BlockyBits.src;
using BlockyBitsClient.src.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockyBitsClient.src
{
    public class World
    {
        WorldGenerator _worldGenerator;

        public World(int seed = 55)
        {
            _worldGenerator = new(seed);
        }


        public void LoadWorld()
        {
            ChunkManager.Start();
        }

        public void DoTick()
        {
            ChunkManager.UpdateChunks();
        }
    }
}
