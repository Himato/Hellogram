namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LittleUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.AspNetUsers", "PasswordHash", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "PasswordHash", c => c.String(nullable: false, maxLength: 32));
            AlterColumn("dbo.AspNetUsers", "Name", c => c.String(nullable: false, maxLength: 32));
        }
    }
}
