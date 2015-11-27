# OpenXC WebServer #
- [Quick Start](#quick-start)
- [Managing devices](#managing-devices)
 - [Viewing device data](#viewing-device-data)
 - [Sending a configuration string to the device](#sending-a-configuration-string-to-the-device)
 - [Deleting devices](#deleting-devices)
- [Firmware upgrades](#firmware-upgrades)
 - [Applying firmware upgrades to logging devices](#applying-firmware-upgrades-to-logging-devices)
- [Managing users](#managing-users)
- [Device API](#device-API)
- [Deploying the web application](#deploying-the-web-application)

## Quick Start ##
**Requires**: [Visual Studio 2013](https://www.visualstudio.com/downloads/download-visual-studio-vs) (Community or Express for Web are both fine)

 1. Open the solution with Visual Studio.
 2. Set **OpenXC.Web** as the StartUp project.
 3. Build the solution (`Ctrl+Shift+B`). This will download all of the required NuGet packages.
 4. Initialize the database.
   1. By default, a LocalDb instance is used. You can leave it as-is or you can change the `OpenXCDbEntities` connection string in **OpenXC.Web/Web.config** to any database provider supported by Entity Framework 6.1.
   2. Run the code-first database migrations to set-up the database. In Visual Studio, open the "Package Manager Console" and type "`update-database`". This also runs the "`Seed()`" method in **OpenXC.Data/Migrations.Configuration.cs**. This is important because it seeds the database with an initial user that will be required to access the database. The default user name and password can be found in the Seed method. This can be changed and it is highly recommended that this default user be removed once new users are created in any public facing installations.
 5. Launch the web application (`F5`). The splash page (and top navigation menu) will allow you to access the various functions of the web application. All areas require you to be logged in.
 
## Managing devices ##
Device management endpoint: `<host:port>/devices`

In order for the server to accept data from OpenXC cellular devices, they must first be registered. To register a new device, click "Register new device" and enter the "Device ID" (Required, IMEI printed on the device), and "Device name" (a "friendly name" to make it easier to identify the device).
Once a device has been registered, it will appear in the device list at `<host:port>/devices`.

### Viewing device data ###
To view the data uploaded to the server from a device, click on the "View Data" link from the device list or go to `<host:port>/devices/<Device ID>/data`. By default, the page will have the current calendar day selected in the date picker but will not load any data until the "Query Data" button is pressed.
 
### Sending a configuration string to the device ###
To send a configuration string to the device, click on the "Send Configuration" link from the device list or go to `<host:port>/devices/<Device ID>/configuration`. See [OpenXC JSON message format](https://github.com/openxc/openxc-message-format/blob/master/JSON.mkd) for the format of commands that can be sent to the device. The configuration will be queued and the device will apply it the next time it checks-in with the server. Only one configuration string will be saved at a time so entering a new one will overwrite one that is still pending.

### Deleting devices ###
To delete a device and all logged data from the server, click on the "Delete" link from the device list or go to `<host:port/devices/<Device ID>/delete`.

## Firmware upgrades ##
Firmware upgrade management endpoint: `<host:port>/upgrades`

OpenXC Cellular C5s can flash firmware OTA by creating a firmware upgrade package and assigning it to devices. When the devices next check-in with the server, if a firmware update is pending, they will attempt to download and apply the upgrade. To create a new firmware upgrade package, click "Create new upgrade", give the upgrade a name, and upload a valid OpenXC C5 firmware hex file. Once the file has been uploaded, it can be downloaded or deleted from the firmware upgrade list.

### Applying firmware upgrades to logging devices ###
Firmware upgrades are applied to logging devices via the device management list (`<host:port>/devices`).

 1. Choose a firmware upgrade from the drop-down list above the logging devices list.
 2. Use the check-boxes in the logging devices table to select the devices to which to apply the upgrade.
 3. Click "Assign to selected loggers".
The firmware upgrade will be queued for the selected logging devices. They will attempt to apply it the next time they check-in with the server.

## Managing users ##
User management endpoint: `<host:port>/users`

User management is very simple. You can create new users by clicking the "Create new user" button, change user passwords but clicking the "Change password" link, or delete the user by clicking the "Delete" link. The only purpose of users in the demo application is to control access to the web application but it could be extended with user roles, device ownership, etc.

## Device API ##
The OpenXC Cellular C5 interacts with three API endpoints in the demo server:
    
`POST <host:port>/api/<Device ID>/data`

- The device will post either JSON (application/json) or Protobuf (application/x-protobuf) encoded data to this endpoint.
- Regardless of the data type, JSON will be saved to the database.

`GET <host:port>/api/<Device ID>/configure`

- The device will query this endpoint to see if any configuration commands are pending.
- If a saved configuration string contains multiple JSON commands, the server will insert a null character (`\0`) between each command to satisfy the command parser on the device.

`GET <host:port>/api/<Device ID>/firmware`

- The device will query this endpoint to see if any firmware upgrades are pending.
- The device supplies an `eTag` with the request that corresponds to the MD5 hash of its current firmware. If this differs from the firmware upgrade assigned to the device, the server will return the new firmware. If the `eTag` matches the currently assigned upgrade or if there is no upgrade assigned, the server will not return anything to the device.

## Deploying the web application ##
For assistance in deploying the web application to an external hosting provider, see [How to: Deploy a Web Project Using One-Click Publish in Visual Studio](https://msdn.microsoft.com/en-us/library/dd465337(v=vs.110).aspx).