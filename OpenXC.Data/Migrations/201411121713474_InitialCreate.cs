namespace OpenXC.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FirmwareUpgrade",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                        FileHash = c.String(nullable: false),
                        FileData = c.Binary(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LoggingDevice",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceName = c.String(nullable: false, maxLength: 64),
                        FirmwareId = c.String(),
                        LastContactTime = c.DateTime(),
                        PendingConfigurationString = c.String(),
                        LastConfigRetrieved = c.DateTime(),
                        FirmwareUpgradeId = c.Int(),
                        FirmwareUpgradeAttempts = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FirmwareUpgrade", t => t.FirmwareUpgradeId)
                .Index(t => t.DeviceName, unique: true)
                .Index(t => t.FirmwareUpgradeId);
            
            CreateTable(
                "dbo.LoggedData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoggedTime = c.DateTime(nullable: false),
                        Data = c.String(),
                        LoggingDeviceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LoggingDevice", t => t.LoggingDeviceId, cascadeDelete: true)
                .Index(t => t.LoggedTime)
                .Index(t => t.LoggingDeviceId);
            
            CreateTable(
                "dbo.WebUser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 64),
                        PasswordHash = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LoggedData", "LoggingDeviceId", "dbo.LoggingDevice");
            DropForeignKey("dbo.LoggingDevice", "FirmwareUpgradeId", "dbo.FirmwareUpgrade");
            DropIndex("dbo.WebUser", new[] { "UserName" });
            DropIndex("dbo.LoggedData", new[] { "LoggingDeviceId" });
            DropIndex("dbo.LoggedData", new[] { "LoggedTime" });
            DropIndex("dbo.LoggingDevice", new[] { "FirmwareUpgradeId" });
            DropIndex("dbo.LoggingDevice", new[] { "DeviceName" });
            DropTable("dbo.WebUser");
            DropTable("dbo.LoggedData");
            DropTable("dbo.LoggingDevice");
            DropTable("dbo.FirmwareUpgrade");
        }
    }
}
