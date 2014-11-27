using OpenXC.Services.Upgrades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenXC.Web.Models
{
    /// <summary>
    /// View model for assigning upgrades
    /// </summary>
    public class AssignUpgradeViewModel
    {
        public LoggingDeviceViewModel LoggingDevice { get; set; }

        public List<FirmwareUpgradeInfo> AvailableUpgrades { get; set; }
    }
}