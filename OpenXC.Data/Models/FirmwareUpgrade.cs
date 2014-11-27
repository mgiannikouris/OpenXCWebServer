using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXC.Data.Models
{
    /// <summary>
    /// A firmware upgrade object.
    /// </summary>
    public class FirmwareUpgrade
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

        /// <summary>
        /// The upgrade file data.
        /// </summary>
        public byte[] FileData { get; set; }

        /// <summary>
        /// Logging devices to which this firmware upgarde is assigned.
        /// </summary>
        public ICollection<LoggingDevice> LoggingDevices { get; set; }
    }
}
