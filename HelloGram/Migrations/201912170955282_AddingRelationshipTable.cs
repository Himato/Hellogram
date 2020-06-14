namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingRelationshipTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Relationships",
                c => new
                    {
                        FollowerId = c.String(nullable: false, maxLength: 128),
                        FollowingId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.FollowerId, t.FollowingId })
                .ForeignKey("dbo.AspNetUsers", t => t.FollowerId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.FollowingId, cascadeDelete: false)
                .Index(t => t.FollowerId)
                .Index(t => t.FollowingId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Relationships", "FollowingId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Relationships", "FollowerId", "dbo.AspNetUsers");
            DropIndex("dbo.Relationships", new[] { "FollowingId" });
            DropIndex("dbo.Relationships", new[] { "FollowerId" });
            DropTable("dbo.Relationships");
        }
    }
}
