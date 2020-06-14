namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingPreferencesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Preferences",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NotificationSound = c.Boolean(nullable: false),
                        DarkTheme = c.Boolean(nullable: false),
                        ActiveState = c.Boolean(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, true)
                .Index(t => t.ApplicationUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Preferences", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Preferences", new[] { "ApplicationUserId" });
            DropTable("dbo.Preferences");
        }
    }
}
