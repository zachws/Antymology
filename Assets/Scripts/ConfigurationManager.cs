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
}
