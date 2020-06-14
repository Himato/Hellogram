namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingReactionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reactions",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        PostId = c.Int(nullable: false),
                        IsLike = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUserId, t.PostId })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .ForeignKey("dbo.Posts", t => t.PostId, cascadeDelete: false)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.PostId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reactions", "PostId", "dbo.Posts");
            DropForeignKey("dbo.Reactions", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Reactions", new[] { "PostId" });
            DropIndex("dbo.Reactions", new[] { "ApplicationUserId" });
            DropTable("dbo.Reactions");
        }
    }
}
