namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingSavedTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Saves",
                c => new
                    {
                        PostId = c.Int(nullable: false),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.PostId, t.ApplicationUserId })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: false)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.PostId)
                .Index(t => t.ApplicationUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Saves", "PostId", "dbo.Posts");
            DropForeignKey("dbo.Saves", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Saves", new[] { "ApplicationUserId" });
            DropIndex("dbo.Saves", new[] { "PostId" });
            DropTable("dbo.Saves");
        }
    }
}
