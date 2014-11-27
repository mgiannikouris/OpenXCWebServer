using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenXC.Web.Models
{
    /// <summary>
    /// View model for specifying device configuration.
    /// </summary>
    public class DeviceConfigurationViewModel
    {
        /// <summary>
        /// The device name.
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// The configuration string to send to the device.
        /// </summary>
        public string Configuration { get; set; }
    }
}