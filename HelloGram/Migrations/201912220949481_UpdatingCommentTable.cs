namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatingCommentTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Comments", new[] { "ApplicationUserId" });
            DropPrimaryKey("dbo.Comments");
            AddColumn("dbo.Comments", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Comments", "ApplicationUserId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.Comments", "Id");
            CreateIndex("dbo.Comments", "ApplicationUserId");
            AddForeignKey("dbo.Comments", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Comments", new[] { "ApplicationUserId" });
            DropPrimaryKey("dbo.Comments");
            AlterColumn("dbo.Comments", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Comments", "Id");
            AddPrimaryKey("dbo.Comments", new[] { "PostId", "ApplicationUserId" });
            CreateIndex("dbo.Comments", "ApplicationUserId");
            AddForeignKey("dbo.Comments", "ApplicationUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
