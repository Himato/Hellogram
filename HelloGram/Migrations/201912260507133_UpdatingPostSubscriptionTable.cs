namespace HelloGram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatingPostSubscriptionTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PostSubscriptions", "IsOff", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PostSubscriptions", "IsOff");
        }
    }
}
