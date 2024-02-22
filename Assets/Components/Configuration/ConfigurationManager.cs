using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationManager : Singleton<ConfigurationManager>
{

    /// <summary>
    /// The seed for world generation.
    /// </summary>
    public int Seed = 1337;

    /// <summary>
    /// The number of chunks in the x and z dimension of the world.
    /// </summary>
    public int World_Diameter = 16;

    /// <summary>
    /// The number of chunks in the y dimension of the world.
    /// </summary>
    public int World_Height = 4;

    /// <summary>
    /// The number of blocks in any dimension of a chunk.
    /// </summary>
    public int Chunk_Diameter = 8;

    /// <summary>
    /// How much of the tile map does each tile take up.
    /// </summary>
    public float Tile_Map_Unit_Ratio = 0.25f;

    /// <summary>
    /// The number of acidic regions on the map.
    /// </summary>
    public int Number_Of_Acidic_Regions = 10;

    /// <summary>
    /// The radius of each acidic region
    /// </summary>
    public int Acidic_Region_Radius = 5;

    /// <summary>
    /// The number of acidic regions on the map.
    /// </summary>
    public int Number_Of_Conatiner_Spheres = 5;

    /// <summary>
    /// The radius of each acidic region
    /// </summary>
    public int Conatiner_Sphere_Radius = 20;

    /// <summary>
    /// Number of ants for a given generation
    /// </summary>
    public int Number_Ants_In_Gen = 20;

    public float Starting_Health = 5000.0f; //queen is 0.5 *= starting_health (i.e., 50% more) 

    public float Maximum_Health = 10000.0f; //queen is 50% more 

    public float Decrease_Health_Amount = 1.0f;

    public float Health_Share_Queen_Weight = 1.0f;

    public float Move_Towards_Queen_Weight = 0.6f;

    public float Mulch_Consumed_Weight = 0.2f;

    public float Amount_Nest_Built_Weight = 1.0f;

    public float Queen_Health_Weight = 0.5f;

    public float Distance_From_Queen_Weight = -0.1f;

    public float Maximum_Queen_Health = 15000.0f;

    public float Starting_Queen_health = 7500.0f;

    public float Minimum_Health_To_Donate = 550.0f;

    public float Donate_Health_Amount = 400.0f; 
}
