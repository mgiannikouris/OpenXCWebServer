using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXC.Services.Upgrades
{
    /// <summary>
    /// Information about a firmware upgrade.
    /// </summary>
    public class FirmwareUpgradeInfo
    {
        /// <summary>
        /// Upgrade identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A friendly name for the upgrade.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A hash of the upgrade file data.
        /// </summary>
        public string FileHash { get; set; }
    }
}
