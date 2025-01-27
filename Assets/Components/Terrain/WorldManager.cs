﻿using Antymology.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Antymology.Terrain
{
    public class WorldManager : Singleton<WorldManager>
    {

        #region Fields

        /// <summary>
        /// The prefab containing the ant.
        /// </summary>
        public GameObject AntVariant;
        public GameObject QueenVariant1;
        public Queen queenLogic;
        //public Material[] materials;

        /// <summary>
        /// The material used for eech block.
        /// </summary>
        public Material blockMaterial;

        /// <summary>
        /// The raw data of the underlying world structure.
        /// </summary>
        private AbstractBlock[,,] Blocks;

        /// <summary>
        /// Reference to the geometry data of the chunks.
        /// </summary>
        private Chunk[,,] Chunks;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private System.Random RNG;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private SimplexNoise SimplexNoise;

        #endregion


        #region MyFields
        //List of ants
        public List<AntLogic> Ants;// = new List<AntLogic>();
        public int antsToGenerate;

        //Neural Network data from: https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/Manager.cs
        public int[] layers = new int[] { 5,5,7,10 }; //5 input normal ant, 5input queen, 7 anonymous/hidden nodes, 9outputs (f, b, l, r, 
        [Range(0.0001f, 1f)] public float mutationChance = 0.01f;
        [Range(0f, 1f)] public float mutationStrength = 0.5f;
        [Range(0.1f, 10f)] public float Gamespeed = 1f;

        public List<NeuralNetwork> networks;
        //private List<AntLogic> antsInNetwork; 

        #endregion

        #region Initialization

        /// <summary>
        /// Awake is called before any start method is called.
        /// </summary>
        void Awake()
        {
            // Generate new random number generator
            RNG = new System.Random(ConfigurationManager.Instance.Seed);

            // Generate new simplex noise generator
            SimplexNoise = new SimplexNoise(ConfigurationManager.Instance.Seed);

            // Initialize a new 3D array of blocks with size of the number of chunks times the size of each chunk
            Blocks = new AbstractBlock[
                ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter,
                ConfigurationManager.Instance.World_Height * ConfigurationManager.Instance.Chunk_Diameter,
                ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter];

            // Initialize a new 3D array of chunks with size of the number of chunks
            Chunks = new Chunk[
                ConfigurationManager.Instance.World_Diameter,
                ConfigurationManager.Instance.World_Height,
                ConfigurationManager.Instance.World_Diameter];
        }

        /// <summary>
        /// Called after every awake has been called.
        /// </summary>
        private void Start()
        {
            GenerateData();
            GenerateChunks();

            Camera.main.transform.position = new Vector3(0 / 2, Blocks.GetLength(1), 0);
            Camera.main.transform.LookAt(new Vector3(Blocks.GetLength(0), 0, Blocks.GetLength(2)));

            InitNetworks();
            GenerateAnts();
            GenerateQueen();
            
        }

        //code from: https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/Manager.cs
        public void InitNetworks()
        {
            networks = new List<NeuralNetwork>();
            int numberOfAnts = ConfigurationManager.Instance.Number_Ants_In_Gen; 

            for(int i = 0; i < numberOfAnts; i++)
            {
                NeuralNetwork network = new NeuralNetwork(layers);
                //network.Load("Assets/pre-trained-data.txt");
                networks.Add(network);
            }
        }

        /// <summary>
        /// TO BE IMPLEMENTED BY YOU
        /// </summary>
        private void GenerateAnts()
        {
            Time.timeScale = Gamespeed;//can increase to speed up training if GPU handles it

            //if the simulation is running but over we should reset by destroying ants, and sorting the neural network so that the next generation can start
            if (Ants.Count > 0)
            {
                CleanupAntsSortGen(); //destroys ants and sorts networks and mutates them.
            }

            int numberOfAnts = ConfigurationManager.Instance.Number_Ants_In_Gen;
            Ants = new List<AntLogic>();
            //-1 so that we can do the queen after the loop 
            for (int i = 0; i < numberOfAnts; i++)
            {
                //Create ant game object for each ant that is supposed to be created as selected/input in the worldManager script interface
                GameObject ant = GameObject.Instantiate(AntVariant);
                ant.name = "workerAnt " + i;
                //get the ant logic (script) to be attached to the new ant game object so that they follow our predefined logic/rules. 
                AntLogic antLogic = ant.GetComponent<AntLogic>();
                //assign ant network 
                antLogic.network = networks[i];
                //antLogic.tag = "Ant";


                //X position can be anything from 1 to the world diameter * chunk diameter
                int xPos = UnityEngine.Random.Range(1, (ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter ));
                //Y starts at the maximum (in the air) and falls down until a non-air block is found within bounds
                int yPos = ConfigurationManager.Instance.World_Height * ConfigurationManager.Instance.Chunk_Diameter;
                //Z position can be anything from 1 to the world diameter * chunk diameter
                int zPos = UnityEngine.Random.Range(1, (ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter));

                //Create vector with these random coordinates so that we can look through the ants Y position such that it lands on top of a box after starting in the air at max height
                var boxToTest = new Vector3(xPos, yPos, zPos);

               // While the block the ant is falling onto is an airblock we keep decrementing Y until the ant lands on a non-airblock type block 
               while(GetBlock((int)boxToTest.x, (int)boxToTest.y,(int)boxToTest.z).GetType() == typeof(AirBlock))
                {
                    //Ant is obviously still in the air so we need to decrement Y (so the ant falls) 
                    boxToTest.y --; 
                }
                boxToTest.y += 0.5f;//Cube offset so that ants spawn on top of cubes 
                antLogic.antPosition = boxToTest;//set position
                //update the actual gameobjects position so that it displays correctly (rather than in the air)
                ant.transform.position = boxToTest;
                Ants.Add(antLogic); //add this particular ant to the antLogic list named Ants
            }
            

        }

        private void CleanupAntsSortGen()
        {
            //kill queen
            // Destroy(QueenVariant1.gameObject);
            //kill ants
            for (int i = 0; i < Ants.Count; i++)
            {
                Destroy(Ants[i].gameObject);
            }
            SortNetworks();
        }

        //code from here: https://github.com/kipgparker/MutationNetwork/blob/master/Mutation%20Neural%20Network/Assets/Manager.cs
        public void SortNetworks()
        {
            int popSize = ConfigurationManager.Instance.Number_Ants_In_Gen;
            for (int i = 0; i < popSize; i++)
            {
                Ants[i].UpdateFitness();
            }

            networks.Sort();
            networks[popSize - 1].Save("Assets/SavedData.txt");

            for(int i = 0; i < popSize / 2; i++)
            {
                networks[i] = networks[i + popSize / 2].Copy(new NeuralNetwork(layers));
                networks[i].Mutate((int)(1 / mutationChance), mutationStrength);
            }
        }

        private void GenerateQueen()
        {
           
            GameObject thaQueen = GameObject.Instantiate(QueenVariant1);
            thaQueen.name = "BossAnt";           
            queenLogic = thaQueen.GetComponent<Queen>();
            //queenLogic.tag = "Ant";

            //X position can be anything from 1 to the world diameter * chunk diameter
            int xPos = UnityEngine.Random.Range(1, (ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter));
            //Y starts at the maximum (in the air) and falls down until a non-air block is found within bounds
            int yPos = ConfigurationManager.Instance.World_Height * ConfigurationManager.Instance.Chunk_Diameter;
            //Z position can be anything from 1 to the world diameter * chunk diameter
            int zPos = UnityEngine.Random.Range(1, (ConfigurationManager.Instance.World_Diameter * ConfigurationManager.Instance.Chunk_Diameter));

            //Create vector with these random coordinates so that we can look through the ants Y position such that it lands on top of a box after starting in the air at max height
            var boxToTest = new Vector3(xPos, yPos, zPos);

            // While the block the ant is falling onto is an airblock we keep decrementing Y until the ant lands on a non-airblock type block 
            while (GetBlock((int)boxToTest.x, (int)boxToTest.y, (int)boxToTest.z).GetType() == typeof(AirBlock))
            {
                //Ant is obviously still in the air so we need to decrement Y (so the ant falls) 
                boxToTest.y--;
            }
            boxToTest.y += 0.5f;//Cube offset so that ants spawn on top of cubes 
            queenLogic.antPosition = boxToTest;
            thaQueen.transform.position = boxToTest; 
            
            
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves an abstract block type at the desired world coordinates.
        /// </summary>
        public AbstractBlock GetBlock(int WorldXCoordinate, int WorldYCoordinate, int WorldZCoordinate)
        {
            if
            (
                WorldXCoordinate < 0 ||
                WorldYCoordinate < 0 ||
                WorldZCoordinate < 0 ||
                WorldXCoordinate >= Blocks.GetLength(0) ||
                WorldYCoordinate >= Blocks.GetLength(1) ||
                WorldZCoordinate >= Blocks.GetLength(2)
            )
                return new AirBlock();

            return Blocks[WorldXCoordinate, WorldYCoordinate, WorldZCoordinate];
        }

        /// <summary>
        /// Retrieves an abstract block type at the desired local coordinates within a chunk.
        /// </summary>
        public AbstractBlock GetBlock(
            int ChunkXCoordinate, int ChunkYCoordinate, int ChunkZCoordinate,
            int LocalXCoordinate, int LocalYCoordinate, int LocalZCoordinate)
        {
            if
            (
                LocalXCoordinate < 0 ||
                LocalYCoordinate < 0 ||
                LocalZCoordinate < 0 ||
                LocalXCoordinate >= Blocks.GetLength(0) ||
                LocalYCoordinate >= Blocks.GetLength(1) ||
                LocalZCoordinate >= Blocks.GetLength(2) ||
                ChunkXCoordinate < 0 ||
                ChunkYCoordinate < 0 ||
                ChunkZCoordinate < 0 ||
                ChunkXCoordinate >= Blocks.GetLength(0) ||
                ChunkYCoordinate >= Blocks.GetLength(1) ||
                ChunkZCoordinate >= Blocks.GetLength(2) 
            )
                return new AirBlock();

            return Blocks
            [
                ChunkXCoordinate * LocalXCoordinate,
                ChunkYCoordinate * LocalYCoordinate,
                ChunkZCoordinate * LocalZCoordinate
            ];
        }

        /// <summary>
        /// sets an abstract block type at the desired world coordinates.
        /// </summary>
        public void SetBlock(int WorldXCoordinate, int WorldYCoordinate, int WorldZCoordinate, AbstractBlock toSet)
        {
            if
            (
                WorldXCoordinate < 0 ||
                WorldYCoordinate < 0 ||
                WorldZCoordinate < 0 ||
                WorldXCoordinate > Blocks.GetLength(0) ||
                WorldYCoordinate > Blocks.GetLength(1) ||
                WorldZCoordinate > Blocks.GetLength(2)
            )
            {
                Debug.Log("Attempted to set a block which didn't exist");
                return;
            }

            Blocks[WorldXCoordinate, WorldYCoordinate, WorldZCoordinate] = toSet;

            SetChunkContainingBlockToUpdate
            (
                WorldXCoordinate,
                WorldYCoordinate,
                WorldZCoordinate
            );
        }

        /// <summary>
        /// sets an abstract block type at the desired local coordinates within a chunk.
        /// </summary>
        public void SetBlock(
            int ChunkXCoordinate, int ChunkYCoordinate, int ChunkZCoordinate,
            int LocalXCoordinate, int LocalYCoordinate, int LocalZCoordinate,
            AbstractBlock toSet)
        {
            if
            (
                LocalXCoordinate < 0 ||
                LocalYCoordinate < 0 ||
                LocalZCoordinate < 0 ||
                LocalXCoordinate > Blocks.GetLength(0) ||
                LocalYCoordinate > Blocks.GetLength(1) ||
                LocalZCoordinate > Blocks.GetLength(2) ||
                ChunkXCoordinate < 0 ||
                ChunkYCoordinate < 0 ||
                ChunkZCoordinate < 0 ||
                ChunkXCoordinate > Blocks.GetLength(0) ||
                ChunkYCoordinate > Blocks.GetLength(1) ||
                ChunkZCoordinate > Blocks.GetLength(2)
            )
            {
                Debug.Log("Attempted to set a block which didn't exist");
                return;
            }
            Blocks
            [
                ChunkXCoordinate * LocalXCoordinate,
                ChunkYCoordinate * LocalYCoordinate,
                ChunkZCoordinate * LocalZCoordinate
            ] = toSet;

            SetChunkContainingBlockToUpdate
            (
                ChunkXCoordinate * LocalXCoordinate,
                ChunkYCoordinate * LocalYCoordinate,
                ChunkZCoordinate * LocalZCoordinate
            );
        }

        #endregion

        #region Helpers

        #region Blocks

        /// <summary>
        /// Is responsible for generating the base, acid, and spheres.
        /// </summary>
        private void GenerateData()
        {
            GeneratePreliminaryWorld();
            GenerateAcidicRegions();
            GenerateSphericalContainers();
        }

        /// <summary>
        /// Generates the preliminary world data based on perlin noise.
        /// </summary>
        private void GeneratePreliminaryWorld()
        {
            for (int x = 0; x < Blocks.GetLength(0); x++)
                for (int z = 0; z < Blocks.GetLength(2); z++)
                {
                    /**
                     * These numbers have been fine-tuned and tweaked through trial and error.
                     * Altering these numbers may produce weird looking worlds.
                     **/
                    int stoneCeiling = SimplexNoise.GetPerlinNoise(x, 0, z, 10, 3, 1.2) +
                                       SimplexNoise.GetPerlinNoise(x, 300, z, 20, 4, 0) +
                                       10;
                    int grassHeight = SimplexNoise.GetPerlinNoise(x, 100, z, 30, 10, 0);
                    int foodHeight = SimplexNoise.GetPerlinNoise(x, 200, z, 20, 5, 1.5);

                    for (int y = 0; y < Blocks.GetLength(1); y++)
                    {
                        if (y <= stoneCeiling)
                        {
                            Blocks[x, y, z] = new StoneBlock();
                        }
                        else if (y <= stoneCeiling + grassHeight)
                        {
                            Blocks[x, y, z] = new GrassBlock();
                        }
                        else if (y <= stoneCeiling + grassHeight + foodHeight)
                        {
                            Blocks[x, y, z] = new MulchBlock();
                        }
                        else
                        {
                            Blocks[x, y, z] = new AirBlock();
                        }
                        if
                        (
                            x == 0 ||
                            x >= Blocks.GetLength(0) - 1 ||
                            z == 0 ||
                            z >= Blocks.GetLength(2) - 1 ||
                            y == 0
                        )
                            Blocks[x, y, z] = new ContainerBlock();
                    }
                }
        }

        /// <summary>
        /// Alters a pre-generated map so that acid blocks exist.
        /// </summary>
        private void GenerateAcidicRegions()
        {
            for (int i = 0; i < ConfigurationManager.Instance.Number_Of_Acidic_Regions; i++)
            {
                int xCoord = RNG.Next(0, Blocks.GetLength(0));
                int zCoord = RNG.Next(0, Blocks.GetLength(2));
                int yCoord = -1;
                for (int j = Blocks.GetLength(1) - 1; j >= 0; j--)
                {
                    if (Blocks[xCoord, j, zCoord] as AirBlock == null)
                    {
                        yCoord = j;
                        break;
                    }
                }

                //Generate a sphere around this point overriding non-air blocks
                for (int HX = xCoord - ConfigurationManager.Instance.Acidic_Region_Radius; HX < xCoord + ConfigurationManager.Instance.Acidic_Region_Radius; HX++)
                {
                    for (int HZ = zCoord - ConfigurationManager.Instance.Acidic_Region_Radius; HZ < zCoord + ConfigurationManager.Instance.Acidic_Region_Radius; HZ++)
                    {
                        for (int HY = yCoord - ConfigurationManager.Instance.Acidic_Region_Radius; HY < yCoord + ConfigurationManager.Instance.Acidic_Region_Radius; HY++)
                        {
                            float xSquare = (xCoord - HX) * (xCoord - HX);
                            float ySquare = (yCoord - HY) * (yCoord - HY);
                            float zSquare = (zCoord - HZ) * (zCoord - HZ);
                            float Dist = Mathf.Sqrt(xSquare + ySquare + zSquare);
                            if (Dist <= ConfigurationManager.Instance.Acidic_Region_Radius)
                            {
                                int CX, CY, CZ;
                                CX = Mathf.Clamp(HX, 1, Blocks.GetLength(0) - 2);
                                CZ = Mathf.Clamp(HZ, 1, Blocks.GetLength(2) - 2);
                                CY = Mathf.Clamp(HY, 1, Blocks.GetLength(1) - 2);
                                if (Blocks[CX, CY, CZ] as AirBlock != null)
                                    Blocks[CX, CY, CZ] = new AcidicBlock();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Alters a pre-generated map so that obstructions exist within the map.
        /// </summary>
        private void GenerateSphericalContainers()
        {

            //Generate hazards
            for (int i = 0; i < ConfigurationManager.Instance.Number_Of_Conatiner_Spheres; i++)
            {
                int xCoord = RNG.Next(0, Blocks.GetLength(0));
                int zCoord = RNG.Next(0, Blocks.GetLength(2));
                int yCoord = RNG.Next(0, Blocks.GetLength(1));


                //Generate a sphere around this point overriding non-air blocks
                for (int HX = xCoord - ConfigurationManager.Instance.Conatiner_Sphere_Radius; HX < xCoord + ConfigurationManager.Instance.Conatiner_Sphere_Radius; HX++)
                {
                    for (int HZ = zCoord - ConfigurationManager.Instance.Conatiner_Sphere_Radius; HZ < zCoord + ConfigurationManager.Instance.Conatiner_Sphere_Radius; HZ++)
                    {
                        for (int HY = yCoord - ConfigurationManager.Instance.Conatiner_Sphere_Radius; HY < yCoord + ConfigurationManager.Instance.Conatiner_Sphere_Radius; HY++)
                        {
                            float xSquare = (xCoord - HX) * (xCoord - HX);
                            float ySquare = (yCoord - HY) * (yCoord - HY);
                            float zSquare = (zCoord - HZ) * (zCoord - HZ);
                            float Dist = Mathf.Sqrt(xSquare + ySquare + zSquare);
                            if (Dist <= ConfigurationManager.Instance.Conatiner_Sphere_Radius)
                            {
                                int CX, CY, CZ;
                                CX = Mathf.Clamp(HX, 1, Blocks.GetLength(0) - 2);
                                CZ = Mathf.Clamp(HZ, 1, Blocks.GetLength(2) - 2);
                                CY = Mathf.Clamp(HY, 1, Blocks.GetLength(1) - 2);
                                Blocks[CX, CY, CZ] = new ContainerBlock();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Given a world coordinate, tells the chunk holding that coordinate to update.
        /// Also tells all 4 neighbours to update (as an altered block might exist on the
        /// edge of a chunk).
        /// </summary>
        /// <param name="worldXCoordinate"></param>
        /// <param name="worldYCoordinate"></param>
        /// <param name="worldZCoordinate"></param>
        private void SetChunkContainingBlockToUpdate(int worldXCoordinate, int worldYCoordinate, int worldZCoordinate)
        {
            //Updates the chunk containing this block
            int updateX = Mathf.FloorToInt(worldXCoordinate / ConfigurationManager.Instance.Chunk_Diameter);
            int updateY = Mathf.FloorToInt(worldYCoordinate / ConfigurationManager.Instance.Chunk_Diameter);
            int updateZ = Mathf.FloorToInt(worldZCoordinate / ConfigurationManager.Instance.Chunk_Diameter);
            Chunks[updateX, updateY, updateZ].updateNeeded = true;
            
            // Also flag all 6 neighbours for update as well
            if(updateX - 1 >= 0)
                Chunks[updateX - 1, updateY, updateZ].updateNeeded = true;
            if (updateX + 1 < Chunks.GetLength(0))
                Chunks[updateX + 1, updateY, updateZ].updateNeeded = true;

            if (updateY - 1 >= 0)
                Chunks[updateX, updateY - 1, updateZ].updateNeeded = true;
            if (updateY + 1 < Chunks.GetLength(1))
                Chunks[updateX, updateY + 1, updateZ].updateNeeded = true;

            if (updateZ - 1 >= 0)
                Chunks[updateX, updateY, updateZ - 1].updateNeeded = true;
            if (updateX + 1 < Chunks.GetLength(2))
                Chunks[updateX, updateY, updateZ + 1].updateNeeded = true;
        }

        #endregion

        #region Chunks

        /// <summary>
        /// Takes the world data and generates the associated chunk objects.
        /// </summary>
        private void GenerateChunks()
        {
            GameObject chunkObg = new GameObject("Chunks");

            for (int x = 0; x < Chunks.GetLength(0); x++)
                for (int z = 0; z < Chunks.GetLength(2); z++)
                    for (int y = 0; y < Chunks.GetLength(1); y++)
                    {
                        GameObject temp = new GameObject();
                        temp.transform.parent = chunkObg.transform;
                        temp.transform.position = new Vector3
                        (
                            x * ConfigurationManager.Instance.Chunk_Diameter - 0.5f,
                            y * ConfigurationManager.Instance.Chunk_Diameter + 0.5f,
                            z * ConfigurationManager.Instance.Chunk_Diameter - 0.5f
                        );
                        Chunk chunkScript = temp.AddComponent<Chunk>();
                        chunkScript.x = x * ConfigurationManager.Instance.Chunk_Diameter;
                        chunkScript.y = y * ConfigurationManager.Instance.Chunk_Diameter;
                        chunkScript.z = z * ConfigurationManager.Instance.Chunk_Diameter;
                        chunkScript.Init(blockMaterial);
                        chunkScript.GenerateMesh();
                        Chunks[x, y, z] = chunkScript;
                    }
        }

        #endregion

        #endregion
    }
}
