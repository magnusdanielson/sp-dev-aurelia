using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace spaureliaWeb.Controllers
{
    public class HomeController : Controller
    {
        [AccessTokenFilter]
        public ActionResult Index()
        {
            User spUser = null;

            var accessTokenInfo = WebAPIHelper.GetAccessTokenInfo(Request);


            using (var clientContext = accessTokenInfo.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    spUser = clientContext.Web.CurrentUser;

                    clientContext.Load(spUser, user => user.Title);

                    clientContext.ExecuteQuery();

                    ViewBag.UserName = spUser.Title;
                }
            }
            HttpCookie cookie = new HttpCookie("CacheKey", accessTokenInfo.CacheKey);
            Response.SetCookie(cookie);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
