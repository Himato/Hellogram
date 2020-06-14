namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingMessengerCoreTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logins",
                c => new
                    {
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        ConnectionId = c.String(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        IsFinished = c.Boolean(nullable: false),
                        Count = c.Int(false),
                    })
                .PrimaryKey(t => t.ApplicationUserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false),
                        Attachment = c.String(),
                        AttachmentSize = c.String(),
                        ReplyId = c.Int(),
                        IsFile = c.Boolean(nullable: false),
                        IsImage = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        SentDateTime = c.DateTime(nullable: false),
                        SeenDateTime = c.DateTime(),
                        SenderId = c.String(maxLength: 128),
                        ReceiverId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ReceiverId)
                .ForeignKey("dbo.Messages", t => t.ReplyId)
                .ForeignKey("dbo.AspNetUsers", t => t.SenderId)
                .Index(t => t.ReplyId)
                .Index(t => t.SenderId)
                .Index(t => t.ReceiverId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "SenderId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "ReplyId", "dbo.Messages");
            DropForeignKey("dbo.Messages", "ReceiverId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Logins", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Messages", new[] { "ReceiverId" });
            DropIndex("dbo.Messages", new[] { "SenderId" });
            DropIndex("dbo.Messages", new[] { "ReplyId" });
            DropIndex("dbo.Logins", new[] { "ApplicationUser_Id" });
            DropTable("dbo.Messages");
            DropTable("dbo.Logins");
        }
    }
}
