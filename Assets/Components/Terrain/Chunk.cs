using System;
using System.Collections;
using System.Collections.Generic;
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
        /// <returns>The Block with local coordinates from this chunk.</returns>
        public Block GetBlock(int localXCoordinate, int localYCoordinate, int localZCoordinate)
        {
            if
            (
                localXCoordinate > radius || localXCoordinate < 0 ||
                localYCoordinate > radius || localYCoordinate < 0 ||
                localZCoordinate > radius || localZCoordinate < 0
            )
                throw new Exception(
                    string.Format("Invalid local coordinate. Chunks have a minimum coordinate of 0, and a maximum coordinate of {0}",
                    WorldManager.Instance.CHUNK_RADIUS)
                );

            throw new NotImplementedException();
        }

        #endregion
    
    }
}