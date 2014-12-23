using AutoMapper;
using OpenXC.Data.Models;
using OpenXC.Services.Devices;
using OpenXC.Services.Upgrades;
using OpenXC.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace OpenXC.Web.Controllers
{
    [RoutePrefix("devices")]
    public class DevicesController : Controller
    {
        /// <summary>
        /// Service for accessing device related data.
        /// </summary>
        private readonly IDevicesService devicesService;

        /// <summary>
        /// Service for accessing upgrade related data.
        /// </summary>
        private readonly IUpgradesService upgradesService;

        /// <summary>
        /// Maximum logged data entries to return at one time.
        /// </summary>
        private const int MaxLoggedDataEntries = 1000;

        /// <summary>
        /// Create a DevicesController controller.
        /// </summary>
        /// <param name="devicesService">Service used for accessing device related data.</param>
        /// <param name="upgradesService">Service used for accessing upgrade related data.</param>
        public DevicesController(IDevicesService devicesService, IUpgradesService upgradesService)
        {
            this.devicesService = devicesService;
            this.upgradesService = upgradesService;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult> Index()
        {
            List<LoggingDeviceInfo> devices = await devicesService.GetLoggingDevices();
            List<FirmwareUpgradeInfo> upgrades = await upgradesService.GetFirmwareUpgrades();

            LoggingDeviceListViewModel vm = new LoggingDeviceListViewModel
            {
                FirmwareUpgrades = upgrades,
                LoggingDevices = Mapper.Map<List<LoggingDeviceInfo>, List<LoggingDeviceViewModel>>(devices)
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("assign")]
        public async Task<ActionResult> AssignUpgrade(LoggingDeviceListViewModel vm)
        {
            if (vm != null && vm.LoggingDevices != null && vm.LoggingDevices.Count > 0)
            {
                IEnumerable<int> selectedLoggers = vm.LoggingDevices
                    .Where(device => device.Selected)
                    .Select(device => device.Id);
                await devicesService.AssignFirmwareUpgradeToLoggers(vm.SelectedFirmwareUpgradeId, selectedLoggers);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("register")]
        public ActionResult Register()
        {
            return View(new LoggingDeviceViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("register")]
        public async Task<ActionResult> Register(LoggingDeviceViewModel newDevice)
        {
            if (ModelState.IsValid)
            {
                LoggingDevice device = await devicesService.RegisterLoggingDevice(newDevice.DeviceName, newDevice.FriendlyName);
                if (device != null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("RegisterDeviceFailure", "Failed to register device. Has the device already been registered?");
                }
            }

            return View(newDevice);
        }

        [HttpGet]
        [Route("{deviceName}/data")]
        public ActionResult Data(string deviceName)
        {
            DateTime today = DateTime.UtcNow.Date;
            return View(new LoggedDataQueryViewModel
            {
                DeviceName = deviceName,
                EndDate = today.AddDays(1),
                StartDate = today,
                LoggedData = new List<LoggedDataViewModel>()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{deviceName}/data")]
        public async Task<ActionResult> Data(LoggedDataQueryViewModel vm)
        {
            if (ModelState.IsValid)
            {
                List<LoggedData> loggedData = await devicesService.GetLoggedData(vm.DeviceName, vm.StartDate, vm.EndDate, MaxLoggedDataEntries + 1);
                vm.MaxEntriesExceeded = (loggedData.Count > MaxLoggedDataEntries);
                vm.LoggedData = Mapper.Map<IEnumerable<LoggedData>, List<LoggedDataViewModel>>(loggedData.Take(MaxLoggedDataEntries));
                return View(vm);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{deviceName}/downloadcsv")]
        public async Task<FileContentResult> DownloadCsv(LoggedDataQueryViewModel vm)
        {
            if (ModelState.IsValid)
            {
                List<LoggedData> loggedData = await devicesService.GetLoggedData(vm.DeviceName, vm.StartDate, vm.EndDate, MaxLoggedDataEntries + 1);
                string csvResult = string.Empty;
                foreach (LoggedData data in loggedData)
                {
                    string date = data.LoggedTime.ToString();
                    string json = data.Data;
                    json = string.Format("\"{0}\"", json.Replace("\"", "\"\""));
                    csvResult = csvResult + string.Format("{0},{1}{2}", date, json, Environment.NewLine);
                }
                return File(new System.Text.UTF8Encoding().GetBytes(csvResult), "text/csv", string.Format("{0}_loggedData.csv", vm.DeviceName));
            }
            return null;
        }

        [HttpGet]
        [Route("{deviceName}/delete")]
        public async Task<ActionResult> Delete(string deviceName)
        {
            LoggingDeviceInfo loggingDevice = await devicesService.GetLoggingDevice(deviceName);
            return View(Mapper.Map<LoggingDeviceInfo, LoggingDeviceViewModel>(loggingDevice));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{deviceName}/delete")]
        public async Task<ActionResult> Delete(LoggingDeviceViewModel vm)
        {
            LoggingDevice device = new LoggingDevice
            {
                Id = vm.Id
            };
            await devicesService.DeleteLoggingDevice(device);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("{deviceName}/configuration")]
        public async Task<ActionResult> Configuration(string deviceName)
        {
            return View(new DeviceConfigurationViewModel
            {
                DeviceName = deviceName,
                Configuration = await devicesService.GetDeviceConfiguration(deviceName)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{deviceName}/configuration")]
        public async Task<ActionResult> Configuration(DeviceConfigurationViewModel vm)
        {
            await devicesService.SetDeviceConfiguration(vm.DeviceName, vm.Configuration);
            return RedirectToAction("Index");
        }
    }
}