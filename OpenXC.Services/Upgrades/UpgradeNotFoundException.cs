using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXC.Services.Upgrades
{
    /// <summary>
    /// Exception thrown when an upgrade is not found.
    /// </summary>
    public class UpgradeNotFoundException : Exception
    {
        /// <summary>
        /// The upgrade ID that could not be found.
        /// </summary>
        public int UpgradeId { get; private set; }

        /// <summary>
        /// Create an UpgradeNotFoundException.
        /// </summary>
        /// <param name="upgradeId">The name of the device that could not be found.</param>
        public UpgradeNotFoundException(int upgradeId)
        {
            UpgradeId = upgradeId;
        }

        public override string Message
        {
            get
            {
                return String.Format("Could not find upgrade {0}", UpgradeId);
            }
        }
    }
}
