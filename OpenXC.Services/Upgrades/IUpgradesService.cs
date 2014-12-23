using OpenXC.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXC.Services.Upgrades
{
    /// <summary>
    /// Interface for managing firmware upgrades
    /// </summary>
    public interface IUpgradesService
    {
        /// <summary>
        /// Get list of firmware upgrades.
        /// </summary>
        /// <returns>List of firmware upgrades.</returns>
        Task<List<FirmwareUpgradeInfo>> GetFirmwareUpgrades();

        /// <summary>
        /// Get a firmware upgrade info.
        /// </summary>
        /// <param name="upgradeId">The upgrade ID.</param>
        /// <exception cref="OpenXC.Services.Upgrades.UpgradeNotFoundException">Thrown if an upgrade with <code>upgradeId</code> does not exist.</exception>
        /// <returns>The firmware upgrade info.</returns>
        Task<FirmwareUpgradeInfo> GetFirmwareUpgrade(int upgradeId);

        /// <summary>
        /// Create a new firmware upgrade.
        /// </summary>
        /// <param name="upgrade">The upgrade to create.</param>
        /// <returns>The saved upgrade. Null if the upgrade could not be created.</returns>
        Task<FirmwareUpgrade> CreateFirmwareUpgrade(FirmwareUpgrade upgrade);

        /// <summary>
        /// Gets a firmware upgrade for downloading
        /// </summary>
        /// <param name="upgradeId">The upgrade ID.</param>
        /// <exception cref="OpenXC.Services.Upgrades.UpgradeNotFoundException">Thrown if an upgrade with <code>upgradeId</code> does not exist.</exception>
        /// <returns>The firmware upgrade.</returns>
        Task<FirmwareUpgrade> GetFirmwareUpgradeFile(int upgradeId);

        /// <summary>
        /// Delete a firmware upgrade.
        /// </summary>
        /// <param name="upgrade">The upgrade to delete.</param>
        Task DeleteFirmwareUpgrade(FirmwareUpgrade upgrade);
    }
}
