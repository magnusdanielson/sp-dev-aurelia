using System;
using System.Web.Mvc;

namespace spaureliaWeb
{
    public class AccessTokenFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            
            if (filterContext.HttpContext.Request.Cookies["CacheKey"] != null)
            {
                string cacheKey = filterContext.HttpContext.Request.Cookies["CacheKey"].Value;

                IAccessTokenInfo cacheItem = WebAPIContextCache.Instance.Get(cacheKey);
                if (cacheItem != null)
                {
                    if(!cacheItem.isValid())
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
                accessTokenInfo = new OnPremAccessTokenInfo(filterContext.HttpContext.Request);
            }
            else
            {
                accessTokenInfo = new CloudAccessTokenInfo(filterContext.HttpContext.Request);
            }
            filterContext.HttpContext.Request.Cookies.Add(new System.Web.HttpCookie("CacheKey", accessTokenInfo.CacheKey));
            //filterContext.HttpContext.Request.Cookies["CacheKey"].Value = accessTokenInfo.CacheKey;
            filterContext.HttpContext.Response.Cookies["CacheKey"].Value = accessTokenInfo.CacheKey;
            WebAPIContextCache.Instance.Put(accessTokenInfo.CacheKey, accessTokenInfo);
            
            //responseMessage.Headers.AddCookies(new CookieHeaderValue[] { cookie });

            // filterContext.Result = new RedirectResult(redirectUrl.AbsoluteUri);
            // filterContext.Result = new ViewResult { ViewName = "Error" };

        }
    }
}