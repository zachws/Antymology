using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Antymology.Terrain
{
    /// <summary>
    /// The air type of block. Contains the internal data representing phermones in the air.
    /// </summary>
    public class AirBlock : AbstractBlock
    {

        #region Fields

        /// <summary>
        /// A dictionary representing the phermone deposits in the air. Each type of phermone gets it's own byte key, and each phermone type has a concentration.
        /// </summary>
        private Dictionary<byte, double> phermoneDeposits;

        #endregion

    }
}
