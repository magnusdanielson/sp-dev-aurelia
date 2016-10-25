using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Web;
using System.Web.SessionState;

namespace spaureliaWeb
{
    public class OnPremAccessTokenInfo : IAccessTokenInfo
    {
        
        public string SPHostUrl { get; set; }
        public string UserAccessToken { get; set; }
        public string SPAppWebUrl { get; set; }
        public string AppAccessToken { get; set; }
        public DateTime Expires { get; set; }
        public string CacheKey { get; set; }
        public string RefreshToken { get; set; }

        WindowsIdentity logonUserIdentity;
        
        
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

        public ClientContext CreateUserClientContextForUrl(string targetUri)
        {
            var onPremUserClientContext =
                TokenHelper.GetS2SClientContextWithWindowsIdentity(new Uri(this.SPHostUrl), logonUserIdentity);

            return onPremUserClientContext;
        }

        public OnPremAccessTokenInfo(HttpRequestBase request)
        {
            WindowsIdentity logonUserIdentity = request.LogonUserIdentity;

            this.SPHostUrl = WebAPIHelper.GetSPHostUrl(request).ToString();

            string onPremAppOnlyAccessToken =
                TokenHelper.GetS2SAccessTokenWithWindowsIdentity(new Uri(this.SPHostUrl), null);

            string onPremUserAccessToken = TokenHelper.GetS2SAccessTokenWithWindowsIdentity(new Uri(this.SPHostUrl), logonUserIdentity);

            this.RefreshToken = null;
            this.CacheKey = logonUserIdentity.User.Value;



            // SPAppWebUrl
            string spAppWebUrlString = TokenHelper.EnsureTrailingSlash(request.QueryString[WebAPIHelper.SPAppWebUrlKey]);
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
            this.AppAccessToken = onPremAppOnlyAccessToken;
            this.UserAccessToken = onPremUserAccessToken;
            this.Expires = DateTime.Now.AddDays(1);
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
            string onPremAppOnlyAccessToken =
                TokenHelper.GetS2SAccessTokenWithWindowsIdentity(new Uri(this.SPHostUrl), null);

            string onPremUserAccessToken = TokenHelper.GetS2SAccessTokenWithWindowsIdentity(new Uri(this.SPHostUrl), logonUserIdentity);


            this.AppAccessToken = onPremAppOnlyAccessToken;
            this.UserAccessToken = onPremUserAccessToken;
            this.Expires = DateTime.Now.AddDays(1);
        }


    }

}