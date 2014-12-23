using AutoMapper;
using OpenXC.Data.Models;
using OpenXC.Services.Upgrades;
using OpenXC.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OpenXC.Web.Utilities;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace OpenXC.Web.Controllers
{
    [RoutePrefix("upgrades")]
    public class UpgradesController : Controller
    {
        /// <summary>
        /// Service for accessing upgrade related data.
        /// </summary>
        private readonly IUpgradesService upgradesService;

        /// <summary>
        /// Create a DevicesController controller.
        /// </summary>
        /// <param name="upgradesService">Service used for accessing upgrade related data.</param>
        public UpgradesController(IUpgradesService upgradesService)
        {
            this.upgradesService = upgradesService;
        }

        [Route("")]
        public async Task<ActionResult> Index()
        {
            List<FirmwareUpgradeInfo> upgrades = await upgradesService.GetFirmwareUpgrades();
            return View(Mapper.Map<List<FirmwareUpgradeInfo>, List<FirmwareUpgradeViewModel>>(upgrades));
        }

        [HttpGet]
        [Route("create")]
        public ActionResult Create()
        {
            return View(new FirmwareUpgradeViewModel()); 
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create(FormCollection form)
        {
            FirmwareUpgradeViewModel vm = new FirmwareUpgradeViewModel();
            if (TryUpdateModel(vm))
            {
                byte[] upgradeBytes = null;
                using (BinaryReader r = new BinaryReader(vm.UploadedFile.InputStream))
                {
                    upgradeBytes = r.ReadBytes(vm.UploadedFile.ContentLength);
                }

                // Calculate MD5 of the file.
                string fileMD5 = HexFileUtility.GetMd5Hash(upgradeBytes);

                FirmwareUpgrade upgrade = new FirmwareUpgrade
                {
                    Name = vm.Name,
                    FileHash = fileMD5,
                    FileData = upgradeBytes,
                };
                upgrade = await upgradesService.CreateFirmwareUpgrade(upgrade);
                if (upgrade == null)
                {
                    ModelState.AddModelError("UpgradeCreationFailed", "Failed to create upgrade. Does an upgrade with this name already exist?");
                    return View(vm);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("{id}/download")]
        public async Task<ActionResult> DownloadFirmwareFile(int id)
        {
            FirmwareUpgrade upgrade = await upgradesService.GetFirmwareUpgradeFile(id);
            
            if (upgrade != null)
            {
                return File(upgrade.FileData, "application/octet-stream", string.Format("{0}_upgradeFile.hex", upgrade.Name));
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        [Route("{id}/delete")]
        public async Task<ActionResult> Delete(int id)
        {
            FirmwareUpgradeInfo upgradeInfo = await upgradesService.GetFirmwareUpgrade(id);
            return View(Mapper.Map<FirmwareUpgradeInfo, FirmwareUpgradeViewModel>(upgradeInfo));
        }

        [HttpPost]
        [Route("{id}/delete")]
        public async Task<ActionResult> Delete(FirmwareUpgradeViewModel vm)
        {
            FirmwareUpgrade upgrade = new FirmwareUpgrade
            {
                Id = vm.Id
            };
            await upgradesService.DeleteFirmwareUpgrade(upgrade);
            return RedirectToAction("Index");
        }

    }
}