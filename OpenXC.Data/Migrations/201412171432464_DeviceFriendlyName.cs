namespace OpenXC.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeviceFriendlyName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoggingDevice", "FriendlyName", c => c.String(nullable: false, maxLength: 64));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoggingDevice", "FriendlyName");
        }
    }
}
