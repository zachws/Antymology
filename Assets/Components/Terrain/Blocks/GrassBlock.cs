using UnityEngine;

namespace Antymology.Terrain
{
    /// <summary>
    /// A block of grass.
    /// </summary>
    public class GrassBlock : AbstractBlock
    {

        /// <summary>
        /// The tile at the 0, 2, position in the tilemap.
        /// </summary>
        public override Vector2 tileMapCoordinate => new Vector2(0, 2);

        /// <summary>
        /// grass is a visible block.
        /// </summary>
        public override bool isVisible => true;
    }
}
