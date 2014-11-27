using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXC.Data.Models
{
    /// <summary>
    /// Data that has been logged.
    /// </summary>
    public class LoggedData
    {
        /// <summary>
        /// Database identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The date/time the data was logged (UTC).
        /// </summary>
        public DateTime LoggedTime { get; set; }

        /// <summary>
        /// The data that was logged.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Device ID for which data was logged.
        /// </summary>
        public int LoggingDeviceId { get; set; }

        /// <summary>
        /// Device for which data was logged.
        /// </summary>
        public LoggingDevice LoggingDevice { get; set; }
    }
}
