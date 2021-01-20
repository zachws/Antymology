using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Antymology.Terrain
{
    /// <summary>
    /// A Chunk is a unity-instanced object which holds the underlying world data in segments
    /// </summary>
    public class Chunk : MonoBehaviour
    {

        #region Fields

        /// <summary>
        /// The X Coordinate of theis chunk.
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// The y Coordinate of this chunk.
        /// </summary>
        public int y { get; set; }

        /// <summary>
        /// The z coordinate of this chunk.
        /// </summary>
        public int z { get; set; }

        /// <summary>
        /// The rendering material used to display this chunk.
        /// </summary>
        public Material mat { get; set; }

        /// <summary>
        /// Whether or not this chunk needs to have its mesh updated
        /// </summary>
        public bool updateNeeded { get; set; }

        /// <summary>
        /// The Unity mesh component of this chunk object.
        /// </summary>
        Mesh mesh;

        /// <summary>
        /// The Unity mesh renderer component of this chunk object.
        /// </summary>
        MeshRenderer renderer;

        #endregion

        #region Methods

        /// <summary>
        /// On start, initialize the gameobject to have a mesh and mesh renderer, and set the references internally.
        /// </summary>
        private void Start()
        {
            mesh = gameObject.AddComponent<MeshFilter>().mesh;
            renderer = gameObject.AddComponent<MeshRenderer>();
        }

        /// <summary>
        /// Gets the Block with local coordinates from this chunk.
        /// </summary>
        /// <param name="localXCoordinate">The local x coordinate of the desired block.</param>
        /// <param name="localYCoordinate">The local y coordinate of the desired block.</param>
        /// <param name="localZCoordinate">The local z coordinate of the desired block.</param>
        /// <exception cref="System.Exception">Invalid local coordinate (either less than 0, or greater than the chunk diameter.</exception>
        /// <returns>The Block with local coordinates from this chunk.</returns>
        public AbstractBlock GetBlock(int localXCoordinate, int localYCoordinate, int localZCoordinate)
        {
            // Error check that the local coordinate exists between 0, and the diameter of the chunk
            if
            (
                localXCoordinate > ConfigurationManager.Instance.Chunk_Diameter || localXCoordinate < 0 ||
                localYCoordinate > ConfigurationManager.Instance.Chunk_Diameter || localYCoordinate < 0 ||
                localZCoordinate > ConfigurationManager.Instance.Chunk_Diameter || localZCoordinate < 0
            )
                throw new Exception(
                    string.Format("Invalid local coordinate. Chunks have a minimum coordinate of 0, and a maximum coordinate of {0}",
                    ConfigurationManager.Instance.Chunk_Diameter)
                );

            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a mesh object which is then passed to the Mesh component of this monobehaviour.
        /// </summary>
        public void GenerateMesh()
        {
            // Creation of temporary mesh variables which we will use to create the unity mesh object.
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            int NumFaces = 0;

            // For each block in this chunk
            for (int x = 0; x < ConfigurationManager.Instance.Chunk_Diameter; x++)
                for (int z = 0; z < ConfigurationManager.Instance.Chunk_Diameter; z++)
                    for (int y = 0; y < ConfigurationManager.Instance.Chunk_Diameter; y++)
                    {
                        // If the block is visible, display all faces which are adjacent to invisible blocks
                        AbstractBlock ours = GetBlock(x, y, z);
                        if (ours.isVisible)
                        {
                            if (!GetBlock(x + 1, y, z).isVisible)
                                AddPosXFace(x, y, z, ours, vertices, triangles, uvs, ref NumFaces);
                            if (!GetBlock(x - 1, y, z).isVisible)
                                AddNegXFace(x, y, z, ours, vertices, triangles, uvs, ref NumFaces);
                            if (!GetBlock(x, y + 1, z).isVisible)
                                AddPosYFace(x, y, z, ours, vertices, triangles, uvs, ref NumFaces);
                            if (!GetBlock(x, y - 1, z).isVisible)
                                AddNegYFace(x, y, z, ours, vertices, triangles, uvs, ref NumFaces);
                            if (!GetBlock(x, y, z + 1).isVisible)
                                AddPosZFace(x, y, z, ours, vertices, triangles, uvs, ref NumFaces);
                            if (!GetBlock(x, y, z - 1).isVisible)
                                AddNegZFace(x, y, z, ours, vertices, triangles, uvs, ref NumFaces);
                        }
                    }

            // Clear whatever data was in the mesh previously
            mesh.Clear();

            // Add in newly calculated values
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();

            // Optimize, and normal calculation
            MeshUtility.Optimize(mesh);
            mesh.RecalculateNormals();
        }

        #endregion

        #region Helpers

        #endregion

    }
}