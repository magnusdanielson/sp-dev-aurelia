using System;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace spaureliaWeb
{
    public class WebAPIAccessTokenFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }
            HttpContextWrapper filterContext = (HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"];
            
            if (filterContext.Request.Cookies["CacheKey"] != null)
            {
                string cacheKey = filterContext.Request.Cookies["CacheKey"].Value;

                IAccessTokenInfo cacheItem = WebAPIContextCache.Instance.Get(cacheKey);
                if (cacheItem != null)
                {
                    if (!cacheItem.isValid())
                    {
                        cacheItem.RefreshTokens();
                        WebAPIContextCache.Instance.Put(cacheKey, cacheItem);
                    }
                    return;
                }
                else
                {
                    // Webbservern har startat om och cachen är tömd
                    //throw new Exception("IAccessTokenInfo is not valid");
                }
            }

            // Första anropet, skapa AccessTokenInfo
            IAccessTokenInfo accessTokenInfo = null;
            if (TokenHelper.IsHighTrustApp())
            {
                accessTokenInfo = new OnPremAccessTokenInfo(filterContext.Request);
            }
            else
            {
                accessTokenInfo = new CloudAccessTokenInfo(filterContext.Request);
            }
            filterContext.Response.Cookies["CacheKey"].Value = accessTokenInfo.CacheKey;
            filterContext.Request.Cookies["CacheKey"].Value = accessTokenInfo.CacheKey;
            WebAPIContextCache.Instance.Put(accessTokenInfo.CacheKey, accessTokenInfo);

            //responseMessage.Headers.AddCookies(new CookieHeaderValue[] { cookie });

            // filterContext.Result = new RedirectResult(redirectUrl.AbsoluteUri);
            // filterContext.Result = new ViewResult { ViewName = "Error" };

        }
    }
}