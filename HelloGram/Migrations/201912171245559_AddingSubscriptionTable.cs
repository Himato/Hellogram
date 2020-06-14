namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingSubscriptionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.CategoryId })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: false)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.CategoryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subscriptions", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.Subscriptions", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Subscriptions", new[] { "CategoryId" });
            DropIndex("dbo.Subscriptions", new[] { "ApplicationUserId" });
            DropTable("dbo.Subscriptions");
        }
    }
}
