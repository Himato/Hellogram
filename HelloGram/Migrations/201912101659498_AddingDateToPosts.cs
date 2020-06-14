namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingDateToPosts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "Date");
        }
    }
}
