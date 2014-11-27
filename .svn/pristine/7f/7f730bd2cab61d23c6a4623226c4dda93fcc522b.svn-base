using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXC.Data.Models
{
    /// <summary>
    /// A logging device.
    /// </summary>
    public class LoggingDevice
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
        /// A configuration string that has yet to be retreived by the logging device.
        /// </summary>
        public string PendingConfigurationString { get; set; }

        /// <summary>
        /// The last time the logging device retrieved a configuration string.
        /// </summary>
        public DateTime? LastConfigRetrieved { get; set; }

        /// <summary>
        /// ID of firmeware upgrade assigned to this logging device.
        /// </summary>
        public int? FirmwareUpgradeId { get; set; }

        /// <summary>
        /// The number of times the currently assigned firmware upgrade has been attempted.
        /// </summary>
        public int? FirmwareUpgradeAttempts { get; set; }

        /// <summary>
        /// The firmware upgrade assigned to this logging device.
        /// </summary>
        public FirmwareUpgrade FirmwareUpgrade { get; set; }

        /// <summary>
        /// Data logged by this logging device.
        /// </summary>
        public ICollection<LoggedData> LoggedData { get; set; }
    }
}
