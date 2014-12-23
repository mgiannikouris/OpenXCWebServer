using Newtonsoft.Json;
using openxc;
using OpenXC.Data.Models;
using OpenXC.Services.Devices;
using OpenXC.Web.Filters;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using OpenXC.Web.Utilities;

namespace OpenXC.Web.Controllers
{
    [DeviceApiExceptionFilter]
    [RoutePrefix("api")]
    public class DeviceApiController : ApiController
    {
        /// <summary>
        /// Service for accessing device related data.
        /// </summary>
        private readonly IDevicesService devicesService;

        /// <summary>
        /// Create a DevicesController controller.
        /// </summary>
        /// <param name="devicesService">Service used for accessing device related data.</param>
        public DeviceApiController(IDevicesService devicesService)
        {
            this.devicesService = devicesService;
        }

        /// <summary>
        /// Endpoint for devices to post data to the server.
        /// </summary>
        [Route("{deviceName}/data")]
        public async Task<HttpResponseMessage> PostData(string deviceName)
        {
            MediaTypeHeaderValue contentType = Request.Content.Headers.ContentType;
            string data = null;
            if (contentType != null && String.Equals("application/json", contentType.MediaType, StringComparison.InvariantCultureIgnoreCase))
            {
                // Save the JSON directly.
                data = await Request.Content.ReadAsStringAsync();
            }
            else if (contentType != null && String.Equals("application/x-protobuf", contentType.MediaType, StringComparison.InvariantCultureIgnoreCase))
            {
                // Attempt protobuf stuff.
                using (Stream s = await Request.Content.ReadAsStreamAsync())
                {
                    IEnumerable<VehicleMessage> messages = Serializer.DeserializeItems<VehicleMessage>(s, PrefixStyle.Base128, 0);
                    data = JsonConvert.SerializeObject(messages);
                }
            }
            else
            {
                // Media type is not supported.
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            await devicesService.AddLoggedData(deviceName, data);
            await devicesService.UpdateLastContactTime(deviceName);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Endpoint for devices to retrieve configuration commands.
        /// </summary>
        [Route("{deviceName}/configure")]
        public async Task<HttpResponseMessage> GetConfiguration(string deviceName)
        {
            string configuration = await devicesService.GetDeviceConfiguration(deviceName);
            await devicesService.MarkConfigurationSent(deviceName);
            await devicesService.UpdateLastContactTime(deviceName);

            if (!String.IsNullOrWhiteSpace(configuration))
            {
                try
                {
                    // Parse out the JSON and add null characters after each command to satisfy the command parser.
                    var parsedJson = JContainer.Parse(configuration);
                    StringBuilder sb = new StringBuilder();
                    if (parsedJson is JObject)
                    {
                        // Single object.
                        sb.Append(parsedJson.ToString(Formatting.None));
                        sb.Append('\0');
                    }
                    else if (parsedJson is JArray)
                    {
                        // Multiple objects.
                        foreach (var command in parsedJson)
                        {
                            sb.Append(command.ToString(Formatting.None));
                            sb.Append('\0');
                        }
                    }

                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(sb.ToString(), Encoding.ASCII, "application/json")
                    };
                }
                catch (Exception e)
                {
                    Trace.TraceInformation("Configuration was not valid JSON. {0}", e);
                }
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(configuration, Encoding.ASCII, "text/plain")
                };
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// Endpoint for devices to retrieve firmware.
        /// </summary>
        [Route("{deviceName}/firmware")]
        public async Task<HttpResponseMessage> GetFirmware(string deviceName)
        {
            string tag = Request.Headers.IfNoneMatch
                .Select(eTag => eTag.Tag.Replace("\"", ""))
                .FirstOrDefault();
            FirmwareUpgrade upgrade = await devicesService.GetNewFirmware(deviceName, tag);
            await devicesService.UpdateLastContactTime(deviceName);

            if (upgrade != null)
            {
                // run HEXMOD on the upgrade file (get flash attributes Start, Length, CRC16-CCITT (iReflected, oReflected))
                HexFileUtility.FlashCRCData flashCRC = HexFileUtility.ProcessHEX(upgrade.FileData);

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(upgrade.FileData.Concat(flashCRC.bytesToAppend).ToArray());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Headers.ETag = new EntityTagHeaderValue(String.Format("\"{0}\"", upgrade.FileHash), false);
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
        }
    }
}
