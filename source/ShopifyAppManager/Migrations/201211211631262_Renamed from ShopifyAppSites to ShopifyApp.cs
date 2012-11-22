namespace ShopifyAppManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedfromShopifyAppSitestoShopifyApp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShopifyApp",
                c => new
                    {
                        ApplicationGuid = c.Guid(nullable: false, identity: true),
                        ApplicationName = c.String(nullable: false),
                        ShopName = c.String(),
                        ApiKey = c.String(),
                        SharedSecret = c.String(),
                        AccessToken = c.String(),
                        Owner = c.String(),
                    })
                .PrimaryKey(t => t.ApplicationGuid);
            
            DropTable("dbo.ShopifyAppSite");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ShopifyAppSite",
                c => new
                    {
                        ApplicationGuid = c.Guid(nullable: false, identity: true),
                        ApplicationName = c.String(nullable: false),
                        AccessToken = c.String(),
                        Owner = c.String(),
                    })
                .PrimaryKey(t => t.ApplicationGuid);
            
            DropTable("dbo.ShopifyApp");
        }
    }
}
