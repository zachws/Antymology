using UnityEngine;

namespace Antymology.Terrain
{
    /// <summary>
    /// A solid stone block.
    /// </summary>
    public class StoneBlock : AbstractBlock
    {

        /// <summary>
        /// The tile at the 3, 1, position in the tilemap.
        /// </summary>
        public override Vector2 tileMapCoordinate => new Vector2(3, 1);

        /// <summary>
        /// Stone is a visible block.
        /// </summary>
        public override bool isVisible => true;
    }
}
