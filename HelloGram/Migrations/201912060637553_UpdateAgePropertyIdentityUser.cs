namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAgePropertyIdentityUser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "Age", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "Age", c => c.DateTime(nullable: false));
        }
    }
}
