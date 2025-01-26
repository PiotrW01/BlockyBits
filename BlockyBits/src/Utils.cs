using BlockyBitsClient.src.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

public class Utils
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

    // returns the x,y of the chunk
    public static Vector2 WorldToChunkPosition(Vector3 pos)
    {
        return new Vector2((int)Math.Floor(pos.X / Chunk.width), (int)Math.Floor(pos.Z / Chunk.depth));
    }

    // returns the x,y,z relative to the chunk start point
    public static Vector3 WorldToLocalChunkCoord(Vector3 pos)
    {
        int x, z;
        x = (int)((pos.X % Chunk.width + Chunk.width) % Chunk.width);
        z = (int)((pos.Z % Chunk.depth + Chunk.depth) % Chunk.depth);
        return new Vector3(x, (int)pos.Y, z);
    }

    public static BoundingBox? GetBoundingBoxAt(Vector3 globalPos)
    {
        int x, z;
        Vector2 chunkPos = WorldToChunkPosition(globalPos);

        // Compute local block coordinates (ensuring positive values)
        x = (int)((globalPos.X % Chunk.width + Chunk.width) % Chunk.width);
        z = (int)((globalPos.Z % Chunk.depth + Chunk.depth) % Chunk.depth);

        ChunkManager.chunks.TryGetValue(chunkPos, out Chunk chunk);
        if (chunk != null)
        {
            if(chunk.HasBlockAt(new Vector3(x, (int)globalPos.Y, z)))
            {
                return new BoundingBox(new Vector3(chunkPos.X * Chunk.width + x,(int)globalPos.Y, chunkPos.Y * Chunk.depth + z), 
                                       new Vector3(chunkPos.X * Chunk.width + x + 1, (int)globalPos.Y + 1, chunkPos.Y * Chunk.depth + z + 1));
            }
        }
        return null;
    }

    public static bool CollidesWithBlockAt(Vector3 pos)
    {
        int x, z;
        Vector2 chunkPos = WorldToChunkPosition(pos);

        // Compute local block coordinates (ensuring positive values)
        x = (int)((pos.X % Chunk.width + Chunk.width) % Chunk.width);
        z = (int)((pos.Z % Chunk.depth + Chunk.depth) % Chunk.depth);

        ChunkManager.chunks.TryGetValue(chunkPos, out Chunk chunk);
        if(chunk != null)
        {
            return chunk.HasBlockAt(new Vector3(x, (int)pos.Y, z));
        }
        return false;
    }

    public static Vector2 GetPlayerChunkPos()
    {
        Vector3 pos = Game1.player.Transform.GlobalPosition;
        int x, z;
        x = (int)((pos.X % Chunk.width + Chunk.width) % Chunk.width);
        z = (int)((pos.Z % Chunk.depth + Chunk.depth) % Chunk.depth);
        return new Vector2(x, z);
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

    public static float Lerp(float a, float b, float f)
    {
        return (a * (1.0f - f) + (b * f));
    }
}

