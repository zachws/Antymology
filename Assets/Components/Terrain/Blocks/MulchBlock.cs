using UnityEngine;

namespace Antymology.Terrain
{
    /// <summary>
    /// A block of mulch. Ants can use this resource to replenish their hunger.
    /// </summary>
    public class MulchBlock : AbstractBlock
    {
        /// <summary>
        /// Statically held tile map coordinate.
        /// </summary>
        static Vector2 _tileMapCoordinate = new Vector2(0, 1);

        /// <summary>
        /// Statically held is visible.
        /// </summary>
        static bool _isVisible = true;

        /// <summary>
        /// The tile at the 0, 1, position in the tilemap.
        /// </summary>
        public override Vector2 tileMapCoordinate()
        {
            return _tileMapCoordinate;
        }

        /// <summary>
        /// mulch is a visible block.
        /// </summary>
        public override bool isVisible()
        {
            return _isVisible;
        }
    }
}
