namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingStatusTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NewUsers = c.Int(nullable: false),
                        ActiveUsers = c.Int(nullable: false),
                        Under18 = c.Int(nullable: false),
                        Views = c.Int(nullable: false),
                        Posts = c.Int(nullable: false),
                        LastTaken = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Status");
        }
    }
}
