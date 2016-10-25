using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;

namespace spaureliaWeb
{
    public static class WebAPIHelper
    {
        public const string SPHostUrlKey = "SPHostUrl";
        public const string SPAppWebUrlKey = "SPAppWebUrl";
        public const string SPLanguageKey = "SPLanguage";
        public const string SPClientTagKey = "SPClientTag";
        public const string SPProductNumberKey = "SPProductNumber";

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Gets the SharePoint host url from QueryString of the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The specified HTTP request.</param>
        /// <returns>The SharePoint host url. Returns <c>null</c> if the HTTP request doesn't contain the SharePoint host url.</returns>
        public static Uri GetSPHostUrl(HttpRequestBase httpRequest)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException("httpRequest");
            }

            string spHostUrlString = TokenHelper.EnsureTrailingSlash(httpRequest.QueryString[SPHostUrlKey]);
            Uri spHostUrl;
            if (Uri.TryCreate(spHostUrlString, UriKind.Absolute, out spHostUrl) &&
                (spHostUrl.Scheme == Uri.UriSchemeHttp || spHostUrl.Scheme == Uri.UriSchemeHttps))
            {
                return spHostUrl;
            }

            // Skriv kod som hämtar från databas istället utifrån Refere
            //httpRequest.UrlReferrer
            return null;
        }
        //public const string SERVICES_TOKEN = "servicesToken";


        /// <summary>
        /// Checks if this request has a servicesToken cookie. To be used from inside the WebAPI.
        /// </summary>
        /// <param name="httpControllerContext">Information about the HTTP request that reached the WebAPI controller</param>
        /// <returns>True if cookie is available and not empty, false otherwise</returns>
        public static bool HasCacheEntry(HttpControllerContext httpControllerContext)
        {
            if (httpControllerContext == null)
                throw new ArgumentNullException("httpControllerContext");

            string cacheKey = GetCacheKey(httpControllerContext.Request);

            if (!String.IsNullOrEmpty(cacheKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GetCacheKey(HttpRequestMessage request)
        {
            var cookie = request.Headers.GetCookies("CacheKey").FirstOrDefault();
            string cacheKey = null;
            if (cookie != null)
            {
                cacheKey = cookie["CacheKey"].Value;
            }
            else
            {
                HttpContextWrapper filterContext = (HttpContextWrapper)request.Properties["MS_HttpContext"];

                if (filterContext.Request.Cookies["CacheKey"] != null)
                {
                    cacheKey = filterContext.Request.Cookies["CacheKey"].Value;
                }


            }
            return cacheKey;
        }



        

        /// <summary>
        /// Creates a ClientContext token for the incoming WebAPI request. This is done by 
        /// - looking up the servicesToken
        /// - extracting the cacheKey 
        /// - get the AccessToken from cache. If the AccessToken is expired a new one is requested using the refresh token
        /// - creation of a ClientContext object based on the AccessToken
        /// </summary>
        /// <param name="httpControllerContext">Information about the HTTP request that reached the WebAPI controller</param>
        /// <returns>A valid ClientContext object</returns>
        public static IAccessTokenInfo GetAccessTokenInfo(HttpControllerContext httpControllerContext)
        {
            if (httpControllerContext == null)
                throw new ArgumentNullException("httpControllerContext");

            string cacheKey = GetCacheKey(httpControllerContext.Request);

            if (!String.IsNullOrEmpty(cacheKey))
            {
                IAccessTokenInfo cacheItem = WebAPIContextCache.Instance.Get(cacheKey);

                if(!cacheItem.isValid())
                {
                    cacheItem.RefreshTokens();
                    WebAPIContextCache.Instance.Put(cacheKey, cacheItem);
                }
                    
                 
            return cacheItem;
            }
            else
            {
                throw new Exception("The cookie with the cachekey was not found...nothing can be retrieved from cache, so no clientcontext can be created.");
            }
        }

        internal static IAccessTokenInfo GetAccessTokenInfo(HttpRequestBase request)
        {
            string cacheKey = null;
            if (request.Cookies["CacheKey"] != null)
            {
                cacheKey = request.Cookies["CacheKey"].Value;
            }
            return  WebAPIContextCache.Instance.Get(cacheKey);
        }
    }
}