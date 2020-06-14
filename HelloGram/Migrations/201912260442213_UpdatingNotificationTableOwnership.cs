namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatingNotificationTableOwnership : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "NotificationType", c => c.Int(nullable: false));
            AddColumn("dbo.Notifications", "OwnershipType", c => c.Int(nullable: false));
            DropColumn("dbo.Notifications", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Notifications", "Type", c => c.Int(nullable: false));
            DropColumn("dbo.Notifications", "OwnershipType");
            DropColumn("dbo.Notifications", "NotificationType");
        }
    }
}
