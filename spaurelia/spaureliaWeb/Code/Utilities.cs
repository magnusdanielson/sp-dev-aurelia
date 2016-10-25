using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace spaureliaWeb
{
    public static class DIConstants
    {
        public static string RibbonDescription = "duniteRibbonDescription";
        public static string CssDescription = "duniteCssDescription";
    }
        public static class Utilities
    {
        public static string AddWebPart(ClientContext clientContext, ListItem listItem, LimitedWebPartManager wpm, string wikiField, string wpFilename, string zone)
        {
            string filepath = HttpContext.Current.Server.MapPath("~/Configuration/" + wpFilename);
            string webPartFileContents = System.IO.File.ReadAllText(filepath);
            WebPartDefinition wpd = wpm.ImportWebPart(webPartFileContents);
            WebPartDefinition wpdNew = wpm.AddWebPart(wpd.WebPart, "wpz", 0);
            clientContext.Load(wpdNew);
            clientContext.ExecuteQuery();

            // Create reference to WebPart in HTML
            string marker = String.Format(CultureInfo.InvariantCulture, "<div class=\"ms-rtestate-read ms-rte-wpbox\" contentEditable=\"false\"><div class=\"ms-rtestate-read {0}\" id=\"div_{0}\"></div><div style='display:none' id=\"vid_{0}\"></div></div>", new object[] { wpdNew.Id.ToString("D") });
            wikiField = wikiField.Replace(zone, zone + marker);

            return wikiField;
        }

        public static void AddCssLink(ClientContext ctx, string[] cssLinks)
        {
            Web web = ctx.Web;
            var existingActions = web.UserCustomActions;
            ctx.Load(existingActions);
            ctx.ExecuteQuery();
            var actions = existingActions.ToArray();
            foreach (var action in actions)
            {
                if (action.Description == DIConstants.CssDescription &&
                    action.Location == "ScriptLink")
                {
                    action.DeleteObject();
                    ctx.ExecuteQuery();
                }
            }
            StringBuilder sb = new StringBuilder();

            foreach (string cssLink in cssLinks)
            {
                sb.Append(cssLink);
            }

            var newAction = existingActions.Add();
            newAction.Description = DIConstants.CssDescription;
            newAction.Location = "ScriptLink";

            newAction.ScriptBlock = sb.ToString();
            newAction.Update();
            ctx.Load(web, s => s.UserCustomActions);
            ctx.ExecuteQuery();
        }

        public static string CreateCssLinkJs(string cssUrl)
        {

            string revision = Guid.NewGuid().ToString().Replace("-", "");
            string cssLink = string.Format("{0}?rev={1}", cssUrl, revision);

            StringBuilder scripts = new StringBuilder(@"(function(){
                var headID = document.getElementsByTagName('head')[0]; 
                var");

            scripts.Append(" newCss = document.createElement('link');newCss.rel = 'stylesheet';newCss.href = '");
            scripts.Append(cssLink);
            scripts.Append("'; headID.appendChild(newCss);})();");
            return scripts.ToString();
        }

        public static void AddJsLink(ClientContext ctx, string[] scriptBlocks)
        {
            Web web = ctx.Web;
            //ctx.Load(web);
            //ctx.ExecuteQuery();

            var existingActions = web.UserCustomActions;
            ctx.Load(existingActions);
            ctx.ExecuteQuery();
            var actions = existingActions.ToArray();
            foreach (var action in actions)
            {
                if (action.Description == DIConstants.RibbonDescription &&
                    action.Location == "ScriptLink")
                {
                    action.DeleteObject();
                    ctx.ExecuteQuery();
                }
            }
            StringBuilder sb = new StringBuilder();

            foreach (string scriptBlock in scriptBlocks)
            {
                sb.Append(scriptBlock);
            }

            var newAction = existingActions.Add();
            newAction.Description = DIConstants.RibbonDescription;
            newAction.Location = "ScriptLink";

            newAction.ScriptBlock = sb.ToString();
            newAction.Update();
            ctx.Load(web, s => s.UserCustomActions);
            ctx.ExecuteQuery();
        }

        public static string CreateScriptLinkJs(HttpRequestMessage request, string jsFileName)
        {
            string scenarioUrl = String.Format("{0}://{1}:{2}", request.RequestUri.Scheme, request.RequestUri.DnsSafeHost, request.RequestUri.Port);
            string revision = Guid.NewGuid().ToString().Replace("-", "");
            string jsLink = string.Format("{0}/{1}?rev={2}", scenarioUrl, jsFileName, revision);

            StringBuilder scripts = new StringBuilder(@"(function(){
                var headID = document.getElementsByTagName('head')[0]; 
                var");
            scripts.Append(" newScript = document.createElement('script');newScript.type = 'text/javascript';newScript.async = false;newScript.setAttribute('data-main', 'aurelia-bootstrapper');newScript.src = '");
            scripts.Append(jsLink);
            scripts.Append("'; headID.appendChild(newScript);})();");
            return scripts.ToString();
        }

        public static void DeleteJsLink(ClientContext ctx)
        {
            Web web = ctx.Web;
            ctx.Load(web, w => w.UserCustomActions);
            ctx.ExecuteQuery();

            for (int i = web.UserCustomActions.Count - 1; i >= 0; i--)
            {
                var action = web.UserCustomActions[i];
                if (action.Description == DIConstants.CssDescription &&
                    action.Location == "ScriptLink")
                {
                    action.DeleteObject();
                    ctx.ExecuteQuery();
                }
                if (action.Description == DIConstants.RibbonDescription &&
                    action.Location == "ScriptLink")
                {
                    action.DeleteObject();
                    ctx.ExecuteQuery();
                }
            }
        }

        public static void AddWebParts(ClientContext clientContext)
        {
            try
            {
                string filepath = HttpContext.Current.Server.MapPath("~/Configuration/startWiki.htm");
                string wikiField = System.IO.File.ReadAllText(filepath);

                // Get references to WebPartManager and HTML
                List list = clientContext.Web.Lists.GetByTitle("Webbplatssidor");
                CamlQuery camlQuery = new CamlQuery();
                ListItemCollection listItems = list.GetItems(camlQuery);
                clientContext.Load(listItems,
                                     items => items.Include(
                                      item => item.DisplayName, item => item["WikiField"]).Where(
                                       item => item.DisplayName == "Startsida"));
                clientContext.ExecuteQuery();
                LimitedWebPartManager wpm = listItems[0].File.GetLimitedWebPartManager(PersonalizationScope.Shared);

                // Remove all webparts
                var webParts = wpm.WebParts;
                clientContext.Load(webParts);
                clientContext.ExecuteQuery();

                for (int i = webParts.Count - 1; i >= 0; i--)
                {
                    var webPart = webParts[i];
                    clientContext.Load(webPart);
                    clientContext.ExecuteQuery();
                    webPart.DeleteWebPart();
                    clientContext.ExecuteQuery();
                }

                // Add WebPart, bottom wp added first.
                wikiField = Utilities.AddWebPart(clientContext, listItems[0], wpm, wikiField, "Aurelia.webpart", "LEFT");

                // Update
                listItems[0]["WikiField"] = wikiField.Replace("LEFT", "").Replace("RIGHT", "");
                listItems[0].Update();
                clientContext.ExecuteQuery();
            }
            catch (Exception x)
            {
                Debug.WriteLine("---EXCEPTION---AddWebPart(): {0}", new object[] { x.Message });
            }
        }

        public static void AddCssAndScripts(ClientContext clientContext, HttpRequestMessage request)
        {
            Utilities.AddJsLink(clientContext, new string[] { Utilities.CreateScriptLinkJs(request, "scripts/vendor-bundle.js") });
        }
    }
}