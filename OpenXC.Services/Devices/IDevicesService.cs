using OpenXC.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXC.Services.Devices
{
    /// <summary>
    /// Service for interacting with logging devices.
    /// </summary>
    public interface IDevicesService
    {
        /// <summary>
        /// Get a list of all devices.
        /// </summary>
        /// <returns>List of devices.</returns>
        /// <remarks>The devices' logged data is not loaded.</remarks>
        Task<List<LoggingDeviceInfo>> GetLoggingDevices();

        /// <summary>
        /// Get a logging device.
        /// </summary>
        /// <param name="deviceName">The logging device name.</param>
        /// <exception cref="OpenXC.Services.Devices.DeviceNotFoundException">Thrown if device with <code>deviceName</code> does not exist.</exception>
        /// <returns>The logging device.</returns>
        Task<LoggingDeviceInfo> GetLoggingDevice(string deviceName);

        /// <summary>
        /// Register a new logging device.
        /// </summary>
        /// <param name="deviceId">The device's Id.</param>
        /// <param name="deviceName">The device's name.</param>
        /// <returns>The created logging device.</returns>
        Task<LoggingDevice> RegisterLoggingDevice(string deviceId, string deviceName);

        /// <summary>
        /// Delete a logging device.
        /// </summary>
        /// <param name="device">The device to delete.</param>
        /// <exception cref="OpenXC.Services.Devices.DeviceNotFoundException">Thrown if device with <code>deviceName</code> does not exist.</exception>
        Task DeleteLoggingDevice(LoggingDevice device);

        /// <summary>
        /// Assign a firmware upgrade to a set of loggers.
        /// </summary>
        /// <param name="firmwareUpgradeId">The ID of the upgrade to assign. If null, upgrades will be unassigned.</param>
        /// <param name="loggerIds">The logger IDs.</param>
        /// <returns>True if successful.</returns>
        Task<bool> AssignFirmwareUpgradeToLoggers(int? firmwareUpgradeId, IEnumerable<int> loggerIds);

        /// <summary>
        /// Get the assigned firmware upgrade for the specified logging device.
        /// </summary>
        /// <param name="deviceName">The logging device name.</param>
        /// <param name="deviceFirmwareHash">The current hash the device has.</param>
        /// <exception cref="OpenXC.Services.Devices.DeviceNotFoundException">Thrown if device with <code>deviceName</code> does not exist.</exception>
        /// <returns>The assigned firmware upgrade if one exists and doesn't match the device's current firmware hash.</returns>
        Task<FirmwareUpgrade> GetNewFirmware(string deviceName, string deviceFirmwareHash);

        /// <summary>
        /// Add logged data to the specified logging device.
        /// </summary>
        /// <param name="deviceName">The logging device name.</param>
        /// <param name="data">The data to add.</param>
        /// <exception cref="OpenXC.Services.Devices.DeviceNotFoundException">Thrown if device with <code>deviceName</code> does not exist.</exception>
        Task AddLoggedData(string deviceName, string data);

        /// <summary>
        /// Get logged data for the specified logging device within a given date range.
        /// </summary>
        /// <param name="deviceName">The logging device name.</param>
        /// <param name="startDate">The start of the range.</param>
        /// <param name="endDate">The end of the range.</param>
        /// <param name="maxResults">The maximum number of entries to return.</param>
        /// <exception cref="OpenXC.Services.Devices.DeviceNotFoundException">Thrown if device with <code>deviceName</code> does not exist.</exception>
        /// <returns>List of all data logged in the date range.</returns>
        Task<List<LoggedData>> GetLoggedData(string deviceName, DateTime startDate, DateTime endDate, int maxEntries);

        /// <summary>
        /// Get any pending configuration command to send to the device.
        /// </summary>
        /// <param name="deviceName">The logging device name.</param>
        /// <exception cref="OpenXC.Services.Devices.DeviceNotFoundException">Thrown if device with <code>deviceName</code> does not exist.</exception>
        /// <returns>Pending configuration command, if one exists.</returns>
        Task<string> GetDeviceConfiguration(string deviceName);

        /// <summary>
        /// Set the configuration string to send to the device.
        /// </summary>
        /// <param name="deviceName">The logging device name.</param>
        /// <param name="configuration">The configuration string to send.</param>
        /// <exception cref="OpenXC.Services.Devices.DeviceNotFoundException">Thrown if device with <code>deviceName</code> does not exist.</exception>
        Task SetDeviceConfiguration(string deviceName, string configuration);

        /// <summary>
        /// Note that configuration has been sent.
        /// </summary>
        /// <param name="deviceName">The logging device name.</param>
        /// <exception cref="OpenXC.Services.Devices.DeviceNotFoundException">Thrown if device with <code>deviceName</code> does not exist.</exception>
        Task MarkConfigurationSent(string deviceName);

        /// <summary>
        /// Update the last time the device was in contact with the server.
        /// </summary>
        /// <param name="deviceName">The logging device name.</param>
        /// <exception cref="OpenXC.Services.Devices.DeviceNotFoundException">Thrown if device with <code>deviceName</code> does not exist.</exception>
        Task UpdateLastContactTime(string deviceName);
    }
}
