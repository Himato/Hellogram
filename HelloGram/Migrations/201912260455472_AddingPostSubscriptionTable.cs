namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingPostSubscriptionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PostSubscriptions",
                c => new
                    {
                        PostId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.PostId, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: false)
                .Index(t => t.PostId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PostSubscriptions", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PostSubscriptions", "PostId", "dbo.Posts");
            DropIndex("dbo.PostSubscriptions", new[] { "UserId" });
            DropIndex("dbo.PostSubscriptions", new[] { "PostId" });
            DropTable("dbo.PostSubscriptions");
        }
    }
}
