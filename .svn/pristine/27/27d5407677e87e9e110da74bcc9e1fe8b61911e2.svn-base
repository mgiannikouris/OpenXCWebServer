using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXC.Services.Devices
{
    /// <summary>
    /// Information about a logging device.
    /// </summary>
    public class LoggingDeviceInfo
    {
        /// <summary>
        /// Device identifier (database only).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The device name (external identifier).
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Identifier for the firmware on the device.
        /// </summary>
        public string FirmwareId { get; set; }

        /// <summary>
        /// The last time the device was in contact with the server.
        /// </summary>
        public DateTime? LastContactTime { get; set; }

        /// <summary>
        /// Does the logging device have a pending configuration change?
        /// </summary>
        public bool HasPendingConfiguration { get; set; }

        /// <summary>
        /// The last time a configuration command was retreived by the device.
        /// </summary>
        public DateTime? LastConfigRetrieved { get; set; }

        /// <summary>
        /// The name of an assigned firmware upgrade.
        /// </summary>
        public string FirmwareUpgradeName { get; set; }

        /// <summary>
        /// The number of times the assigned firmware upgrade was attempted for this logging device.
        /// </summary>
        public int? FirmwareUpgradeAttempts { get; set; }
    }
}
