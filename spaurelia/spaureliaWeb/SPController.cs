using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.SharePoint.Client;
using System.Web;
using System.Diagnostics;
using Microsoft.SharePoint.Client.WebParts;

namespace spaureliaWeb
{
    public class SPController : ApiController
    {
        // GET api/<controller>
        [WebAPIAccessTokenFilter]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [WebAPIAccessTokenFilter]
        public string Get(int id)
        {
            var accessTokenInfo = WebAPIHelper.GetAccessTokenInfo(ControllerContext);
            var ctx = accessTokenInfo.CreateUserClientContextForSPHost();
            ctx.Load(ctx.Web, w => w.Title);
            ctx.ExecuteQuery();

            return ctx.Web.Title;

            //return "value";
        }

        // POST api/<controller>
        [WebAPIAccessTokenFilter]
        public void Post([FromBody]string value)
        {
            var accessTokenInfo = WebAPIHelper.GetAccessTokenInfo(ControllerContext);


            using (var appClientContext = accessTokenInfo.CreateAppOnlyClientContextForSPHost())
            {
                Utilities.AddCssAndScripts(appClientContext, this.Request);
                Utilities.AddWebParts(appClientContext);
            }
        }

        // Lägg till i vendor-bundle.js
        // requirejs.config({ "baseUrl": "https://localhost:44336/src/"
        // Ändra vendor-bundle.js
        // aurelia.start().then(function () { return aurelia.setRoot('binding-demo'); });

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
            var accessTokenInfo = WebAPIHelper.GetAccessTokenInfo(ControllerContext);


            using (var appClientContext = accessTokenInfo.CreateAppOnlyClientContextForSPHost())
            {
                Utilities.DeleteJsLink(appClientContext);
            }
            
        }
    }
}