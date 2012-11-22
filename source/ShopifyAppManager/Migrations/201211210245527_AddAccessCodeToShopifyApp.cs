namespace ShopifyAppManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccessCodeToShopifyApp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShopifyApp", "AccessToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShopifyApp", "AccessToken");
        }
    }
}
