using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenXC.Services.Devices
{
    /// <summary>
    /// Exception thrown when a device is not found.
    /// </summary>
    public class DeviceNotFoundException : Exception
    {
        /// <summary>
        /// The device name that could not be found.
        /// </summary>
        public string DeviceName { get; private set; }

        /// <summary>
        /// Create a DeviceNotFoundException.
        /// </summary>
        /// <param name="deviceName">The name of the device that could not be found.</param>
        public DeviceNotFoundException(string deviceName)
        {
            DeviceName = deviceName;
        }

        public override string Message
        {
            get
            {
                return String.Format("Could not find device {0}", DeviceName);
            }
        }
    }
}
