using OpenXC.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenXC.Data.DB;
using System.Data.Entity;
using System.Diagnostics;

namespace OpenXC.Services.Devices
{
    /// <summary>
    /// Implementation of devices service.
    /// </summary>
    public class DevicesService : IDevicesService
    {
        /// <summary>
        /// The database instance used by this service.
        /// </summary>
        private readonly OpenXCDbContext db;

        /// <summary>
        /// The maximum number of times a single firmware upgrade will be returned for a logging device.
        /// </summary>
        private const int MaxFirmwareUpgradeAttempts = 3;

        /// <summary>
        /// Constructor for the service.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public DevicesService(OpenXCDbContext dbContext)
        {
            db = dbContext;
        }

        /// <summary>
        /// List of LoggingDeviceInfo objects.
        /// </summary>
        private IQueryable<LoggingDeviceInfo> LoggingDeviceInfoList
        {
            get
            {
                return db.LoggingDevices
                    .Select(dbLoggingDevice => new LoggingDeviceInfo
                    {
                        Id = dbLoggingDevice.Id,
                        DeviceName = dbLoggingDevice.DeviceName,
                        FirmwareId = dbLoggingDevice.FirmwareId,
                        FirmwareUpgradeName = dbLoggingDevice.FirmwareUpgrade.Name,
                        FirmwareUpgradeAttempts = dbLoggingDevice.FirmwareUpgradeAttempts,
                        LastContactTime = dbLoggingDevice.LastContactTime,
                        HasPendingConfiguration = !String.IsNullOrEmpty(dbLoggingDevice.PendingConfigurationString),
                        LastConfigRetrieved = dbLoggingDevice.LastConfigRetrieved
                    });
            }
        }

        public Task<List<LoggingDeviceInfo>> GetLoggingDevices()
        {
            return LoggingDeviceInfoList.ToListAsync();
        }

        public async Task<LoggingDeviceInfo> GetLoggingDevice(string deviceName)
        {
            LoggingDeviceInfo loggingDevice = await LoggingDeviceInfoList
               .SingleOrDefaultAsync(dbLoggingDevice => dbLoggingDevice.DeviceName == deviceName);
            if (loggingDevice == null)
            {
                // Can't find the device.
                throw new DeviceNotFoundException(deviceName);
            }

            return loggingDevice;
        }

        public async Task<LoggingDevice> RegisterLoggingDevice(string deviceName)
        {
            LoggingDevice newDevice = new LoggingDevice
            {
                DeviceName = deviceName
            };
            try
            {
                db.Entry(newDevice).State = EntityState.Added;
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Trace.TraceError("Error saving logging device: {0}", e);
                return null;
            }
            return newDevice;
        }

        public async Task DeleteLoggingDevice(LoggingDevice device)
        {
            db.Entry(device).State = EntityState.Deleted;
            await db.SaveChangesAsync();
        }

        public async Task<bool> AssignFirmwareUpgradeToLoggers(int? firmwareUpgradeId, IEnumerable<int> loggerIds)
        {
            // Set all the loggers to the selected upgrade ID.
            foreach (int loggerId in loggerIds)
            {
                LoggingDevice device = new LoggingDevice
                {
                    Id = loggerId,
                    FirmwareUpgradeId = firmwareUpgradeId,
                    FirmwareUpgradeAttempts = firmwareUpgradeId.HasValue ? 0 : (int?)null
                };
                db.LoggingDevices.Attach(device);
                db.Entry(device).Property(dbLoggingDevice => dbLoggingDevice.FirmwareUpgradeId).IsModified = true;
                db.Entry(device).Property(dbLoggingDevice => dbLoggingDevice.FirmwareUpgradeAttempts).IsModified = true;
            }

            // Save the changes.
            try
            {
                await db.SaveChangesNoValidationAsync();
            }
            catch (Exception e)
            {
                Trace.TraceError("Failed to assign upgrade to loggers: {0}", e);
                return false;
            }
            return true;
        }

        public async Task<FirmwareUpgrade> GetNewFirmware(string deviceName, string deviceFirmwareHash)
        {
            // Save the device's reported firmware hash.
            var loggerInfo = await db.LoggingDevices
                .Where(dbLoggingDevice => dbLoggingDevice.DeviceName == deviceName)
                .Select(dbLoggingDevice => new
                {
                    LoggingDevice = dbLoggingDevice,
                    AssignedUpgradeHash = dbLoggingDevice.FirmwareUpgrade.FileHash
                })
                .SingleOrDefaultAsync();
            if (loggerInfo == null)
            {
                // Could not find the supplied device name.
                throw new DeviceNotFoundException(deviceName);
            }

            // Update firmware hash on device.
            LoggingDevice device = loggerInfo.LoggingDevice;
            device.FirmwareId = deviceFirmwareHash;

            // Get the device's upgrade data if it is different.
            if (!String.Equals(loggerInfo.AssignedUpgradeHash, deviceFirmwareHash) &&
                device.FirmwareUpgradeAttempts < MaxFirmwareUpgradeAttempts)
            {
                // Save changes to the number of upgrade attempts and the hash on the device.
                device.FirmwareUpgradeAttempts = (device.FirmwareUpgradeAttempts ?? 0) + 1;
                await db.SaveChangesAsync();

                return await db.LoggingDevices
                    .Where(dbLoggingDevice => dbLoggingDevice.Id == device.Id)
                    .Select(dbLoggingDevice => dbLoggingDevice.FirmwareUpgrade)
                    .SingleOrDefaultAsync();
            }
            else
            {
                // Save change to the hash on the device.
                await db.SaveChangesAsync();

                // No new firmware upgrade to return.
                return null;
            }
        }

        public async Task AddLoggedData(string deviceName, string data)
        {
            int? loggingDeviceId = await db.LoggingDevices
                .Where(dbLoggingDevice => dbLoggingDevice.DeviceName == deviceName)
                .Select(dbLoggingDevice => (int?)dbLoggingDevice.Id)
                .SingleOrDefaultAsync();
            if (!loggingDeviceId.HasValue)
            {
                // Can't find the device.
                throw new DeviceNotFoundException(deviceName);
            }

            // Create the logged data and save it.
            LoggedData loggedData = new LoggedData
            {
                Data = data,
                LoggedTime = DateTime.UtcNow,
                LoggingDeviceId = loggingDeviceId.Value
            };
            db.LoggedData.Add(loggedData);
            await db.SaveChangesAsync();
        }


        public async Task<List<LoggedData>> GetLoggedData(string deviceName, DateTime startDate, DateTime endDate, int maxEntries)
        {
            int? loggingDeviceId = await db.LoggingDevices
                .Where(dbLoggingDevice => dbLoggingDevice.DeviceName == deviceName)
                .Select(dbLoggingDevice => (int?)dbLoggingDevice.Id)
                .SingleOrDefaultAsync();
            if (!loggingDeviceId.HasValue)
            {
                // Can't find the device.
                throw new DeviceNotFoundException(deviceName);
            }

            return await db.LoggedData
                .AsNoTracking()
                .Where(dbLoggedData => dbLoggedData.LoggingDeviceId == loggingDeviceId.Value &&
                                       startDate <= dbLoggedData.LoggedTime &&
                                       dbLoggedData.LoggedTime <= endDate)
                .OrderBy(dbLoggedData => dbLoggedData.LoggedTime)
                .Take(maxEntries)
                .ToListAsync();
        }

        public async Task<string> GetDeviceConfiguration(string deviceName)
        {
            var loggingDevice = await db.LoggingDevices
                .Where(dbLoggingDevice => dbLoggingDevice.DeviceName == deviceName)
                .Select(dbLoggingDevice => new
                {
                    Configuration = dbLoggingDevice.PendingConfigurationString
                })
                .SingleOrDefaultAsync();
            if (loggingDevice == null)
            {
                // Can't find the device.
                throw new DeviceNotFoundException(deviceName);
            }

            return loggingDevice.Configuration;
        }

        public async Task SetDeviceConfiguration(string deviceName, string configuration)
        {
            var loggerInfo = await db.LoggingDevices
                .Where(dbLoggingDevice => dbLoggingDevice.DeviceName == deviceName)
                .Select(dbLoggingDevice => new
                {
                    LoggingDeviceId = dbLoggingDevice.Id
                })
                .SingleOrDefaultAsync();
            if (loggerInfo == null)
            {
                // Could not find the supplied device name.
                throw new DeviceNotFoundException(deviceName);
            }

            LoggingDevice device = new LoggingDevice
            {
                Id = loggerInfo.LoggingDeviceId,
                PendingConfigurationString = configuration
            };
            db.LoggingDevices.Attach(device);
            db.Entry(device).Property(dbLoggingDevice => dbLoggingDevice.PendingConfigurationString).IsModified = true;
            await db.SaveChangesNoValidationAsync();
            db.Entry(device).State = EntityState.Detached;
        }

        public async Task MarkConfigurationSent(string deviceName)
        {
            LoggingDevice loggingDevice = await db.LoggingDevices
               .SingleOrDefaultAsync(dbLoggingDevice => dbLoggingDevice.DeviceName == deviceName);
            if (loggingDevice == null)
            {
                // Can't find the device.
                throw new DeviceNotFoundException(deviceName);
            }

            loggingDevice.PendingConfigurationString = null;
            loggingDevice.LastConfigRetrieved = DateTime.UtcNow;

            await db.SaveChangesAsync();
        }

        public async Task UpdateLastContactTime(string deviceName)
        {
            LoggingDevice loggingDevice = await db.LoggingDevices
              .SingleOrDefaultAsync(dbLoggingDevice => dbLoggingDevice.DeviceName == deviceName);
            if (loggingDevice == null)
            {
                // Can't find the device.
                throw new DeviceNotFoundException(deviceName);
            }

            loggingDevice.LastContactTime = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
    }
}
