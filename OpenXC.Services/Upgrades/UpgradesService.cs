using OpenXC.Data.DB;
using OpenXC.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXC.Services.Upgrades
{
    /// <summary>
    /// Implementation of upgrades service.
    /// </summary>
    public class UpgradesService : IUpgradesService
    {
        /// <summary>
        /// The database instance used by this service.
        /// </summary>
        private readonly OpenXCDbContext db;

        /// <summary>
        /// Constructor for the service.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public UpgradesService(OpenXCDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// List of firmware upgrade infos.
        /// </summary>
        private IQueryable<FirmwareUpgradeInfo> FirmwareUpgradeInfoList
        {
            get
            {
                return db.FirmwareUpgrades
                    .Select(dbUpgrade => new FirmwareUpgradeInfo
                    {
                        Id = dbUpgrade.Id,
                        Name = dbUpgrade.Name,
                        FileHash = dbUpgrade.FileHash
                    });
            }
        }

        /// <summary>
        /// List of Firmware upgrades
        /// </summary>
        private IQueryable<FirmwareUpgrade> FirmwareUpgradeList
        {
            get
            {
                return db.FirmwareUpgrades;
            }
        }

        public Task<List<FirmwareUpgradeInfo>> GetFirmwareUpgrades()
        {
            return FirmwareUpgradeInfoList.ToListAsync();
        }

        public async Task<FirmwareUpgradeInfo> GetFirmwareUpgrade(int upgradeId)
        {
            FirmwareUpgradeInfo firmwareUpgrade = await FirmwareUpgradeInfoList
               .SingleOrDefaultAsync(dbUpgrade => dbUpgrade.Id == upgradeId);
            if (firmwareUpgrade == null)
            {
                // Can't find the upgrade.
                throw new UpgradeNotFoundException(upgradeId);
            }

            return firmwareUpgrade;
        }

        public async Task<FirmwareUpgrade> CreateFirmwareUpgrade(FirmwareUpgrade upgrade)
        {
            try
            {
                db.Entry(upgrade).State = EntityState.Added;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Trace.TraceError("Error saving logging device: {0}", e);
                return null;
            }
            return upgrade;
        }

        public async Task<FirmwareUpgrade> GetFirmwareUpgradeFile(int upgradeId)
        {
            FirmwareUpgrade firmwareUpgrade = await FirmwareUpgradeList
                .SingleOrDefaultAsync(upgrade => upgrade.Id == upgradeId);

            if (firmwareUpgrade == null)
            {
                // Can't find the upgrade.
                throw new UpgradeNotFoundException(upgradeId);
            }

            return firmwareUpgrade;
        }

        public async Task DeleteFirmwareUpgrade(FirmwareUpgrade upgrade)
        {
            // Attach the upgrade object.
            db.FirmwareUpgrades.Attach(upgrade);

            // Load related logging devices so they can have the upgrade unassigned.
            await db.Entry(upgrade).Collection(dbUpgrade => dbUpgrade.LoggingDevices).LoadAsync();

            // Delete upgrade.
            db.Entry(upgrade).State = EntityState.Deleted;
            await db.SaveChangesAsync();
        }
    }
}
