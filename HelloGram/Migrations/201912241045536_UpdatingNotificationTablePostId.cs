namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatingNotificationTablePostId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Notifications", "PostId", "dbo.Posts");
            DropIndex("dbo.Notifications", new[] { "PostId" });
            AlterColumn("dbo.Notifications", "PostId", c => c.Int());
            CreateIndex("dbo.Notifications", "PostId");
            AddForeignKey("dbo.Notifications", "PostId", "dbo.Posts", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "PostId", "dbo.Posts");
            DropIndex("dbo.Notifications", new[] { "PostId" });
            AlterColumn("dbo.Notifications", "PostId", c => c.Int(nullable: false));
            CreateIndex("dbo.Notifications", "PostId");
            AddForeignKey("dbo.Notifications", "PostId", "dbo.Posts", "Id", cascadeDelete: true);
        }
    }
}
