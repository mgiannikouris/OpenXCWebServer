using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OpenXC.Web.Models
{
    /// <summary>
    /// View model for querying logged data.
    /// </summary>
    public class LoggedDataQueryViewModel
    {
        /// <summary>
        /// The device for which to load logged data.
        /// </summary>
        public string DeviceName { get; set; }
        
        /// <summary>
        /// The start date to query for logged data.
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end date to query for logged data.
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// There are more logged data entries than can be returned at once.
        /// </summary>
        public bool MaxEntriesExceeded { get; set; }

        /// <summary>
        /// The logged data.
        /// </summary>
        public List<LoggedDataViewModel> LoggedData { get; set; }
    }
}