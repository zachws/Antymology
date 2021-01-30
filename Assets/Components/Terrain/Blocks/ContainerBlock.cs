using UnityEngine;

namespace Antymology.Terrain
{
    /// <summary>
    /// An unmovable, unbreakable block which houses the simulation.
    /// </summary>
    public class ContainerBlock : AbstractBlock
    {

        /// <summary>
        /// The tile at the 3, 3, position in the tilemap.
        /// </summary>
        public override Vector2 tileMapCoordinate => new Vector2(3, 3);

        /// <summary>
        /// Container blocks are visible blocks.
        /// </summary>
        public override bool isVisible => true;
    }
}
