using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenXC.Web.Models
{
    /// <summary>
    /// View model for firmware upgrades.
    /// </summary>
    public class FirmwareUpgradeViewModel
    {
        /// <summary>
        /// Upgrade identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A friendly name for the upgrade.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// A hash of the upgrade file data.
        /// </summary>
        [Display(Name = "Firmware MD5")]
        public string FileHash { get; set; }

        /// <summary>
        /// The upgrade file data.
        /// </summary>
        [Required]
        [Display(Name = "Firmware file")]
        public HttpPostedFileBase UploadedFile { get; set; }
    }
}