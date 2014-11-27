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

                // run HEXMOD on the upgrade file (get flash attributes Start, Length, CRC16-CCITT (iReflected, oReflected))
                HexFileUtility.FlashCRCData flashCRC = HexFileUtility.ProcessHEX(upgradeBytes);

                // Calculate MD5 of the file.
                string fileMD5 = HexFileUtility.GetMd5Hash(upgradeBytes);
                if (!String.IsNullOrWhiteSpace(vm.FileHash) && !String.Equals(vm.FileHash, fileMD5, StringComparison.InvariantCultureIgnoreCase))
                {
                    // User specified MD5 of the file but it didn't match what the server received.
                    ModelState.AddModelError(
                        "FileHash",
                        String.Format("Uploaded file MD5 ({0}) did not match supplied value.", fileMD5));
                    return View(vm);
                }

                FirmwareUpgrade upgrade = new FirmwareUpgrade
                {
                    Name = vm.Name,
                    FileHash = flashCRC.flashMD5,
                    FileData = upgradeBytes.Concat(flashCRC.bytesToAppend).ToArray(),
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