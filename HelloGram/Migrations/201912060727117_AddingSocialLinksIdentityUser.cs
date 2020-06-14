namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingSocialLinksIdentityUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FacebookLink", c => c.String());
            AddColumn("dbo.AspNetUsers", "TwitterLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "TwitterLink");
            DropColumn("dbo.AspNetUsers", "FacebookLink");
        }
    }
}
