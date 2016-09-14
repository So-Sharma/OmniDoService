using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;


namespace WebRole1
{
    public class AuthenticationModule : IHttpModule
    {
        private const string XId = "X-Id";
        private const string XAuthServiceProvider = "X-Auth-Service-Provider";
        private const string VerifyCredentialsAuthorization = "X-Verify-Credentials-Authorization";
        private const string ApiTwitterCom = "api.digits.com";
        private const string ApplicationJson = "application/json";
        private const string Twitter = "Twitter";

        public void Dispose()
        {
        }

        public void Init(HttpApplication application)
        {
            var wrapper = new EventHandlerTaskAsyncHelper(AuthenticateRequest);
            application.AddOnAuthenticateRequestAsync(wrapper.BeginEventHandler, wrapper.EndEventHandler);

            wrapper = new EventHandlerTaskAsyncHelper(BeginRequest);
            application.AddOnBeginRequestAsync(wrapper.BeginEventHandler, wrapper.EndEventHandler);
        }

        private async Task BeginRequest(object sender, EventArgs e)
        {
            OmniRequestContext.InitializeRequestContext();

            return;
        }

        private async Task AuthenticateRequest(object sender, EventArgs e)
        {
            var appObject = (HttpApplication) sender;
            var contextObj = appObject.Context;

            //For UT
            var providerId = contextObj.Request.Headers[XId];
            if (!string.IsNullOrEmpty(providerId))
            {
                SetRequestContextIds(providerId);

                return;
            }
            //UT end

            var endPoint = contextObj.Request.Headers[XAuthServiceProvider];
            var authorization = contextObj.Request.Headers[VerifyCredentialsAuthorization];

            //Check if authentication parameters are null
            if (endPoint == null)
            {
                SetResponse(HttpStatusCode.Unauthorized, "Authorization service provider header is missing");
            }

            if (authorization == null)
            {
                SetResponse(HttpStatusCode.Unauthorized, "Authorization header is missing");
            }


            //Verify the X-Auth-Service-Provider header by parsing the uri and asserting the domain is api.digits.com to ensure you are calling Digits.
            var twitterEndpoint = new Uri(endPoint);

            if (string.Compare(twitterEndpoint.Authority, ApiTwitterCom, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return;
            }

            //Check if authorization info is already in cache
            var idStr = HttpRuntime.Cache.Get(authorization) as string;
            if (idStr == null)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", authorization);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));

                    var response = await client.GetAsync(endPoint);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsAsync<Result>();

                        SetRequestContextIds(result.id_str);

                        HttpRuntime.Cache.Insert(authorization, result.id_str, null, DateTime.Now.AddMinutes(10),
                                Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    else
                    {
                        SetResponse(response.StatusCode, response.ReasonPhrase);
                    }
                }
            }
            else
            {
                SetRequestContextIds(idStr);
            }

        }

        private static void SetResponse(HttpStatusCode statusCode, string reason)
        {
            var context = HttpContext.Current;
            context.Response.StatusCode = (int)statusCode;
            context.Response.Write(reason);
            context.Response.End();
        }

        private void SetRequestContextIds(string providerId)
        {
            var requestContext = OmniRequestContext.Current;
            requestContext.id = providerId;
        }

        private class Result
        {
            public string created_at;
            public string id;
            public string id_str;
        }
    }
}