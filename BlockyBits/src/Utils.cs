using BlockyBitsClient.src.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

internal class Utils
{
    static public float DegToRad(float angle)
    {
        return (((float)Math.PI) / 180) * angle;
    }
    static public float RadToDeg(float angle)
    {
        return angle * 180 / ((float)Math.PI);
    }

    public static Vector2[] RepeatValues6(Vector2 pos)
    {
        Vector2[] result = new Vector2[6];
        for (int i = 0; i < 6; i++)
        {
            result[i] = new Vector2(pos.X,pos.Y);
        }
        return result;
    }

    public static Vector2 ChunkToWorldCoord(Chunk chunk)
    {
        return new Vector2(chunk.posX * Chunk.width, chunk.posY * Chunk.depth);
    }

    public static Vector2 WorldToChunkCoord(Vector3 pos)
    {
        return new Vector2((int)(pos.X / Chunk.width), (int)(pos.Y / Chunk.depth));
    }

    public static Vector3 WorldToGridCoord(Vector3 pos)
    {
        Vector2 chunkPos = new Vector2((int)(pos.X / Chunk.width), (int)(pos.Y / Chunk.depth));
        Chunk chunk = ChunkManager.chunks[chunkPos];
        chunk.HasBlockAt(pos);
        return new Vector3();
    }

    public static bool CollidesWithBlockAt(Vector3 pos)
    {
        int x, z;
        Vector2 chunkPos = Vector2.Zero;

        if (pos.X < 0)
        {
            x = 16 + (int)Math.Floor(pos.X % Chunk.width);
            chunkPos.X = (int)Math.Floor(pos.X / Chunk.width);
        } else
        {
            x = (int)pos.X % Chunk.width;
            chunkPos.X = (int)pos.X / Chunk.width;
        }

        if(pos.Z < 0)
        {
            z = 16 + (int)Math.Floor(pos.Z % Chunk.depth);
            chunkPos.Y = (int)Math.Floor(pos.Z / Chunk.depth);
        } else
        {
            z = (int)pos.Z % Chunk.depth;
            chunkPos.Y = (int)pos.Z / Chunk.depth;
        }

        ChunkManager.chunks.TryGetValue(chunkPos, out Chunk chunk);
        if(chunk != null)
        {
            bool has = chunk.HasBlockAt(new Vector3(x, pos.Y, z));
            //Debug.Write($"{pos} ");
            //Debug.WriteLine($"collision at chunk [{chunkPos.X},{chunkPos.Y}], X:{x}, Z:{z}, collision:{has}");
            if (has)
            {
                //Debug.WriteLine("collided");
            }
            return chunk.HasBlockAt(new Vector3(x, (int)pos.Y, z));
        }
        return false;
    }
    
    public static Vector2 GetChunkAt(Vector3 pos)
    {
        Vector2 chunkPos = Vector2.Zero;

        if (pos.X < 0)
        {
            chunkPos.X = (int)Math.Floor(pos.X / Chunk.width);
        }
        else
        {
            chunkPos.X = (int)pos.X / Chunk.width;
        }

        if (pos.Z < 0)
        {
            chunkPos.Y = (int)Math.Floor(pos.Z / Chunk.depth);
        }
        else
        {
            chunkPos.Y = (int)pos.Z / Chunk.depth;
        }
        return chunkPos;
    }

    public static Vector3 GetPlayerGlobalPos()
    {
        Vector3 pos = Game1.player.pos;
        int x, z;
        //Vector2 chunkPos;
        if (pos.X < 0)
        {
            x = 16 + (int)Math.Floor(pos.X % Chunk.width);
            //chunkPos.X = (int)Math.Floor(pos.X / Chunk.width);
        }

        if (pos.Z < 0)
        {
            z = 16 + (int)Math.Floor(pos.Z % Chunk.depth);
            //chunkPos.Y = (int)Math.Floor(pos.Z / Chunk.depth);
        }

        return new Vector3((int)pos.X, (int)pos.Y, (int)pos.Z);
    }

    public static Vector2 GetPlayerChunkPos()
    {
        Vector3 pos = Game1.player.pos;
        int x, z;
        //Vector2 chunkPos;
        if (pos.X < 0)
        {
            x = 16 + (int)Math.Floor(pos.X % Chunk.width);
            //chunkPos.X = (int)Math.Floor(pos.X / Chunk.width);
        }
        else
        {
            x = (int)pos.X % Chunk.width;
            //chunkPos.X = (int)pos.X / Chunk.width;
        }

        if (pos.Z < 0)
        {
            z = 16 + (int)Math.Floor(pos.Z % Chunk.depth);
            //chunkPos.Y = (int)Math.Floor(pos.Z / Chunk.depth);
        }
        else
        {
            z = (int)pos.Z % Chunk.depth;
            //chunkPos.Y = (int)pos.Z / Chunk.depth;
        }
        return new Vector2(x, z);
    }

    public static Vector3 GridToWorldCoord(Vector3 pos)
    {
        return new Vector3();
    }

    public static Vector3 EaseInOut(Vector3 a, Vector3 b, float t)
    {
        t = Math.Max(0.0f, Math.Min(1.0f, t));
        var t2 = t * t;
        return a + (b - a) * (3 - 2 * t) * t2;
    }

    public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

}

