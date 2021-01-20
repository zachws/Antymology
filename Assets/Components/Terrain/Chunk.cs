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
        /// The radius of a chunk object.
        /// </summary>
        public int radius;

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

        public Block GetBlock(int localXCoordinate, int localYCoordinate, int localZCoordinate)
        {
            if
            (
                localXCoordinate > radius || localXCoordinate < 0 ||
                localYCoordinate > radius || localYCoordinate < 0 ||
                localZCoordinate > radius || localZCoordinate < 0
            )
                throw new Exception(
                    string.Format("Invalid local coordinate. Chunks have a minimum coordinate of 0, and a maximum coordinate of {0}", radius)
                );

            throw new NotImplementedException;
        }

        #endregion
    
    }
}