namespace ShopifyAppManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOwner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShopifyApp", "Owner", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShopifyApp", "Owner");
        }
    }
}
