using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenXC.Web.Models
{
    /// <summary>
    /// View model for logging devices.
    /// </summary>
    public class LoggingDeviceViewModel
    {
        /// <summary>
        /// Device identifier (database only).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Is the object selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// The device Id (external identifier).
        /// </summary>
        [Display(Name = "Device ID")]
        [Required]
        [MaxLength(64)]
        public string DeviceName { get; set; }

        /// <summary>
        /// The device name
        /// </summary>
        [Display(Name = "Device name")]
        [Required]
        [MaxLength(64)]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Identifier for the firmware on the device.
        /// </summary>
        [Display(Name = "Firmware hash")]
        public string FirmwareId { get; set; }

        /// <summary>
        /// The last time the device was in contact with the server.
        /// </summary>
        [Display(Name = "Last contact time")]
        public DateTime? LastContactTime { get; set; }

        /// <summary>
        /// Does the logging device have a pending configuration change?
        /// </summary>
        [Display(Name = "Has pending configuration?")]
        public bool HasPendingConfiguration { get; set; }

        /// <summary>
        /// The last time a configuration command was retreived by the device.
        /// </summary>
        [Display(Name = "Last configuration retrieved")]
        public DateTime? LastConfigRetrieved { get; set; }

        /// <summary>
        /// The name of an assigned firmware upgrade.
        /// </summary>
        [Display(Name = "Firmware upgrade name")]
        public string FirmwareUpgradeName { get; set; }

        /// <summary>
        /// The number of times the currently assigned firmware upgrade has been attempted.
        /// </summary>
        [Display(Name = "Upgrade attempts")]
        public string FirmwareUpgradeAttempts { get; set; }
    }
}