using AutoMapper;
using OpenXC.Data.Models;
using OpenXC.Services.Devices;
using OpenXC.Services.Upgrades;
using OpenXC.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenXC.Web.App_Start
{
    public class AutoMapperWebProfile : Profile
    {
        public override string ProfileName
        {
            get
            {
                return "Web Profile";
            }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<LoggingDeviceInfo, LoggingDeviceViewModel>()
                .ForMember(vmType => vmType.Selected, opt => opt.Ignore());     // Ignore selected property as it is view model only.

            Mapper.CreateMap<FirmwareUpgradeInfo, FirmwareUpgradeViewModel>()
                .ForMember(vmType => vmType.UploadedFile, opt => opt.Ignore()); // Ignore UploadedFile when mapping dbType to view model type.

            Mapper.CreateMap<LoggedData, LoggedDataViewModel>();
        }
    }
}