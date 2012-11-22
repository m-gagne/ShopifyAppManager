using ShopifyAPIAdapterLibrary;
using ShopifyAppManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;
using WebMatrix.WebData;

namespace ShopifyAppManager.Controllers
{
    [Authorize]
    public class ShopifyController : Controller
    {
        //
        // GET: /Shopify
        public ActionResult Index()
        {
            using (ShopifyAppsContext context = new ShopifyAppsContext())
            {
                ViewData["Applications"] = context.ShopifyApps.Where(a => a.Owner == User.Identity.Name).ToList();
                
            } 

            return View();
        }

        //
        // GET: /Shopify/Test
        public ActionResult Test()
        {
            var appID = Request.Params["id"];
            using (ShopifyAppsContext context = new ShopifyAppsContext())
            {
                var app = context.ShopifyApps.Find(new Guid(appID));
                ViewData["App"] = app;

                ShopifyAuthorizationState authState = new ShopifyAuthorizationState
                {
                    ShopName = app.ShopName,
                    AccessToken = app.AccessToken
                };
               
                var api = new ShopifyAPIClient(authState, new JsonDataTranslator());

                // The JSON Data Translator will automatically decode the JSON for you
                ViewData["ProductData"] = api.Get("/admin/products.json");
            }
            return View();
        }

        //
        // GET: /Shopify/Create
        [HttpGet]
        public ActionResult New()
        {
            return View();
        }

        // POST: /Shopify/Create
        [HttpPost]
        public JsonResult Create()
        {
            var name = Request.Params["Name"].Trim();

            ShopifyApp app;
            using (ShopifyAppsContext context = new ShopifyAppsContext())
            {
                app = context.ShopifyApps.Create();
                app.ApplicationName = name;
                app.Owner = User.Identity.Name;
                context.ShopifyApps.Add(app);
                context.SaveChanges();
            }

            var root = Request.Url.GetLeftPart(UriPartial.Authority);
            app.InitializeUrls(root);
            return Json(app);
        }


        // DELETE: /Shopify/Destroy
        [HttpDelete]
        public System.Web.Mvc.EmptyResult Destroy()
        {
            var appId = Request.Params["id"];

            using (ShopifyAppsContext context = new ShopifyAppsContext())
            {
                var app = context.ShopifyApps.Where(a => a.Owner == User.Identity.Name && a.ApplicationGuid == new Guid(appId)).FirstOrDefault();
                if (app != null)
                {
                    context.ShopifyApps.Remove(app);
                    context.SaveChanges();
                }
            }

            return new EmptyResult();
        }

        //
        // GET: /Shopify/init
        public ActionResult Init()
        {
            string appGuid = Request.Params["AppGuid"];
            string shopName = Request.Params["ShopName"];
            string apiKey = Request.Params["ApiKey"];
            string sharedSecret = Request.Params["SharedSecret"];
            
            var root = Request.Url.GetLeftPart(UriPartial.Authority);
            // you will need to pass a URL that will handle the response from Shopify when it passes you the code parameter
            Uri returnURL = new Uri(string.Format("{0}/Shopify/finalize?appGuid={1}&shopName={2}&apiKey={3}&sharedSecret={4}",
                root,
                appGuid,
                shopName,
                apiKey,
                sharedSecret));

            var authorizer = new ShopifyAPIAuthorizer(shopName, apiKey, sharedSecret);

            // get the Authorization URL and redirect the user
            var authUrl = authorizer.GetAuthorizationURL(new string[] {
                "read_content",
                "write_content",
                "read_themes",
                "write_themes",
                "read_products",
                "write_products",
                "read_customers",
                "write_customers",
                "read_orders",
                "write_orders",
                "read_script_tags",
                "write_script_tags" },
                returnURL.ToString());
            return Redirect(authUrl);
        }

        //
        // GET: /Shopify/finalize
        public ActionResult Finalize()
        {
            string appGuid = Request.Params["AppGuid"];
            string shopName = Request.Params["ShopName"];
            string apiKey = Request.Params["ApiKey"];
            string sharedSecret = Request.Params["SharedSecret"];

            var authorizer = new ShopifyAPIAuthorizer(shopName, apiKey, sharedSecret);

            // get the following variables from the Query String of the request
            string error = Request.QueryString["error"];
            string code = Request.Params["code"];
            string shop = Request.QueryString["shop"];

            // check for an error first
            if (!String.IsNullOrEmpty(error))
            {
                this.TempData["Error"] = error;
                return RedirectToAction("Login");
            }

            // make sure we have the code
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(shop))
                return RedirectToAction("Index", "Home");

            // get the authorization state
            ShopifyAuthorizationState authState = authorizer.AuthorizeClient(code);

            if (authState != null && authState.AccessToken != null)
            {
                using (ShopifyAppsContext context = new ShopifyAppsContext())
                {
                    var app = context.ShopifyApps.Find(new Guid(appGuid));
                    if (app != null)
                    {
                        app.ShopName = shopName;
                        app.ApiKey = apiKey;
                        app.SharedSecret = sharedSecret;
                        app.AccessToken = authState.AccessToken;
                        context.SaveChanges();
                    }
                }

            }

            return RedirectToAction("Index");
        }

    }
}
