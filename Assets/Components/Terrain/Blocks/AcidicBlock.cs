using UnityEngine;

namespace Antymology.Terrain
{
    /// <summary>
    /// A dangerous block for ants to walk on. Damages ants for every tick they are on this block.
    /// </summary>
    public class AcidicBlock : AbstractBlock
    {

        /// <summary>
        /// The tile at the 0, 3, position in the tilemap.
        /// </summary>
        public override Vector2 tileMapCoordinate => new Vector2(0, 3);

        /// <summary>
        /// acidic blocks are visible.
        /// </summary>
        public override bool isVisible => true;
    }
}
