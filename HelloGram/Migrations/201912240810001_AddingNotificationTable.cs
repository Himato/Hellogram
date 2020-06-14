namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingNotificationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SenderId = c.String(maxLength: 128),
                        ReceiverId = c.String(maxLength: 128),
                        Type = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ReceiverId)
                .ForeignKey("dbo.AspNetUsers", t => t.SenderId)
                .Index(t => t.SenderId)
                .Index(t => t.ReceiverId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "SenderId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Notifications", "ReceiverId", "dbo.AspNetUsers");
            DropIndex("dbo.Notifications", new[] { "ReceiverId" });
            DropIndex("dbo.Notifications", new[] { "SenderId" });
            DropTable("dbo.Notifications");
        }
    }
}
