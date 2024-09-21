using System;

public interface ChunkGenerator
{
    void LoadChunkAt(Random random, int chunkX, int chunkY, GenerateDirection generateDirection, RoadType roadType);
    void UnloadChunkAt(int chunkX, int chunkY);
}
public enum GenerateDirection
{
    NORTH,
    SOUTH,
    EAST,
    WEST
}