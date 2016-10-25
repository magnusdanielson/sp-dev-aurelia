using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.SessionState;

namespace spaureliaWeb
{
    public class CloudAccessTokenInfo : IAccessTokenInfo
    {
        private string cacheKey;

        public string SPHostUrl { get; set; }
        public string UserAccessToken { get; set; }
        public string SPAppWebUrl { get; set; }
        public string AppAccessToken { get; set; }
        public DateTime Expires { get; set; }
        public string CacheKey
        {
            get
            {
                return WebAPIHelper.RemoveSpecialCharacters(this.cacheKey);
            }
            set
            {
                this.cacheKey = value;
            }
        }
        public string RefreshToken { get; set; }
        
        public ClientContext CreateAppOnlyClientContextForUrl(string url)
        {
            return TokenHelper.GetClientContextWithAccessToken(url, this.AppAccessToken);
        }
        public ClientContext CreateUserClientContextForSPHost()
        {
            return TokenHelper.GetClientContextWithAccessToken(this.SPHostUrl, this.UserAccessToken);
        }

        public ClientContext CreateAppOnlyClientContextForSPHost()
        {
            return TokenHelper.GetClientContextWithAccessToken(this.SPHostUrl, this.AppAccessToken);
        }

        public ClientContext CreateUserClientContextForSPAppWeb()
        {
            return TokenHelper.GetClientContextWithAccessToken(this.SPAppWebUrl, this.UserAccessToken);
        }

        public ClientContext CreateAppOnlyClientContextForSPAppWeb()
        {
            return TokenHelper.GetClientContextWithAccessToken(this.SPAppWebUrl, this.AppAccessToken);
        }



        public CloudAccessTokenInfo(HttpRequestBase request)
        {
            try
            {
                var contextToken = TokenHelper.GetContextTokenFromRequest(request);

                SharePointContextToken sharePointContextToken = TokenHelper.ReadAndValidateContextToken(contextToken, request.Url.Authority);
                this.RefreshToken = sharePointContextToken.RefreshToken;

                this.CacheKey = sharePointContextToken.CacheKey;

            this.SPHostUrl = WebAPIHelper.GetSPHostUrl(request).ToString();

                // SPAppWebUrl
                string spAppWebUrlString = TokenHelper.EnsureTrailingSlash(request.QueryString[SharePointContext.SPAppWebUrlKey]);
                Uri spAppWebUrl;
                if (!Uri.TryCreate(spAppWebUrlString, UriKind.Absolute, out spAppWebUrl) ||
                    !(spAppWebUrl.Scheme == Uri.UriSchemeHttp || spAppWebUrl.Scheme == Uri.UriSchemeHttps))
                {
                    spAppWebUrl = null;
                }
                else
                {
                    this.SPAppWebUrl = spAppWebUrl.ToString();
                }
                var accessToken = TokenHelper.GetAccessToken(this.RefreshToken, TokenHelper.SharePointPrincipal,
                                                                    new Uri(this.SPHostUrl).Authority, TokenHelper.GetRealmFromTargetUrl(
                                                                    new Uri(this.SPHostUrl)));

                var userToken = accessToken.AccessToken;

                string appToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, new Uri(this.SPHostUrl).Authority, TokenHelper.GetRealmFromTargetUrl(
                                                            new Uri(this.SPHostUrl))).AccessToken;

                this.AppAccessToken = appToken;
                this.UserAccessToken = userToken;
                this.Expires = accessToken.ExpiresOn.AddHours(12);
            }
            catch(Exception error)
            {
                //throw new Exception("Redirect user to app");
            }
            
        }
        public bool isValid()
        {
            
            if (DateTime.Now > this.Expires)
            {
                return false;
            }
            return true;
        }

        public void RefreshTokens()
        {
            try
            {
                var accessToken = TokenHelper.GetAccessToken(this.RefreshToken, TokenHelper.SharePointPrincipal,
                                                                    new Uri(this.SPHostUrl).Authority, TokenHelper.GetRealmFromTargetUrl(
                                                                    new Uri(this.SPHostUrl)));

                var userToken = accessToken.AccessToken;

                string appToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, new Uri(this.SPHostUrl).Authority, TokenHelper.GetRealmFromTargetUrl(
                                                            new Uri(this.SPHostUrl))).AccessToken;

                this.AppAccessToken = appToken;
                this.UserAccessToken = userToken;
                this.Expires = accessToken.ExpiresOn;
            }
            catch(Exception error)
            {
                //Om det här fallerar ska användaren redirectas till appen för nya tokens
            }
        }

        public ClientContext CreateUserClientContextForUrl(string targetUri)
        {
            var cloudAppAccessToken =
                TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, this.SPHostUrl, TokenHelper.GetRealmFromTargetUrl(new Uri(this.SPHostUrl)));
            var cloudAppClientContext =
                TokenHelper.GetClientContextWithAccessToken(targetUri.ToString(), cloudAppAccessToken.AccessToken);

            return cloudAppClientContext;
        }
    }

}