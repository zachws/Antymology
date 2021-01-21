using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationManager : Singleton<ConfigurationManager>
{

    /// <summary>
    /// The number of chunks in any dimension of the world.
    /// </summary>
    public int World_Diameter = 16;

    /// <summary>
    /// The number of blocks in any dimension of a chunk.
    /// </summary>
    public int Chunk_Diameter = 8;

    /// <summary>
    /// How much of the tile map does each tile take up
    /// </summary>
    public float Tile_Map_Unit_Ratio = 0.25f;
}
