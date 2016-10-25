using System;
using Microsoft.SharePoint.Client;

namespace spaureliaWeb
{
    public interface IAccessTokenInfo
    {
        string AppAccessToken { get; set; }
        DateTime Expires { get; set; }
        string RefreshToken { get; set; }
        string SPAppWebUrl { get; set; }
        string SPHostUrl { get; set; }
        string UserAccessToken { get; set; }
        string CacheKey { get; set; }
        ClientContext CreateAppOnlyClientContextForSPAppWeb();
        ClientContext CreateAppOnlyClientContextForSPHost();
        ClientContext CreateAppOnlyClientContextForUrl(string url);
        ClientContext CreateUserClientContextForUrl(string url);
        ClientContext CreateUserClientContextForSPAppWeb();
        ClientContext CreateUserClientContextForSPHost();
        bool isValid();

        void RefreshTokens();
    }
}