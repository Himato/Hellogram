namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendingIdentityUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "Age", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "City", c => c.String(maxLength: 32));
            AddColumn("dbo.AspNetUsers", "Country", c => c.String(maxLength: 32));
            AddColumn("dbo.AspNetUsers", "University", c => c.String(maxLength: 64));
            AddColumn("dbo.AspNetUsers", "Company", c => c.String(maxLength: 32));
            AddColumn("dbo.AspNetUsers", "Position", c => c.String(maxLength: 32));
            AddColumn("dbo.AspNetUsers", "About", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "About");
            DropColumn("dbo.AspNetUsers", "Position");
            DropColumn("dbo.AspNetUsers", "Company");
            DropColumn("dbo.AspNetUsers", "University");
            DropColumn("dbo.AspNetUsers", "Country");
            DropColumn("dbo.AspNetUsers", "City");
            DropColumn("dbo.AspNetUsers", "Age");
            DropColumn("dbo.AspNetUsers", "CreatedAt");
        }
    }
}
