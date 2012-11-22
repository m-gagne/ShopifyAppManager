namespace ShopifyAppManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShopifyApp",
                c => new
                    {
                        ApplicationGuid = c.Guid(nullable: false, identity: true),
                        ApplicationName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ApplicationGuid);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShopifyApp");
        }
    }
}
