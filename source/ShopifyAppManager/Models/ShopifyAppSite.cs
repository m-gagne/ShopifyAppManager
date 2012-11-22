using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace ShopifyAppManager.Models
{
    public class ShopifyAppsContext : DbContext
    {
        public ShopifyAppsContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<ShopifyApp> ShopifyApps { get; set; }
    }

    [Table("ShopifyApp")]
    public class ShopifyApp
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid ApplicationGuid { get; set; }
        [Required]
        public string ApplicationName { get; set; }
        public string ShopName { get; set; }
        public string ApiKey { get; set; }
        public string SharedSecret { get; set; }
        public string AccessToken { get; set; }
        public string Owner { get; set; }
        [NotMapped]
        public string ApplicationUrl { get; set; }
        [NotMapped]
        public string ApplicationReturnUrl { get; set; }

        public void InitializeUrls( string root )
        {
            this.ApplicationUrl = string.Format("{0}/Shopify/App/{1}", root, this.ApplicationGuid);
            this.ApplicationReturnUrl = string.Format("{0}/Shopify/App/{1}/Finalize", root, this.ApplicationGuid);
        }
    }   
}