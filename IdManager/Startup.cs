﻿/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Security.Claims;
using System.Threading.Tasks;
using IdentityManager.Configuration;
using IdentityManager.Core.Logging;
using IdentityManager.Extensions;
using IdentityManager.Logging;
using IdManager.IdMgr;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Twitter;
using Owin;
using Serilog;

namespace IdManager
{
    internal class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LogProvider.SetCurrentLogProvider(new DiagnosticsTraceLogProvider());
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Trace()
               .CreateLogger();
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
            });

            app.UseOpenIdConnectAuthentication(new Microsoft.Owin.Security.OpenIdConnect.OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "oidc",
                Authority = "https://localhost:44344",
                ClientId = "idmgr_client",
                RedirectUri = "https://localhost:44337",
                ResponseType = "id_token",
                UseTokenLifetime = false,
                Scope = "openid idmgr",
                SignInAsAuthenticationType = "Cookies",
                Notifications = new Microsoft.Owin.Security.OpenIdConnect.OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = n =>
                    {
                        n.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        return Task.FromResult(0);
                    },
                    RedirectToIdentityProvider = async n =>
                    {
                        if (n.ProtocolMessage.RequestType == Microsoft.IdentityModel.Protocols.OpenIdConnectRequestType.LogoutRequest)
                        {
                            var result = await n.OwinContext.Authentication.AuthenticateAsync("Cookies");
                            if (result != null)
                            {
                                var id_token = result.Identity.Claims.GetValue("id_token");
                                if (id_token != null)
                                {
                                    n.ProtocolMessage.IdTokenHint = id_token;
                                    n.ProtocolMessage.PostLogoutRedirectUri = "https://localhost:44337";
                                }
                            }
                        }
                    }
                }
            });


            var connectionString = "MembershipReboot";

            //app.Map("/admin", adminApp =>
            //{
                var factory = new IdentityManagerServiceFactory();
                factory.Configure(connectionString);

                app.UseIdentityManager(new IdentityManagerOptions()
                {
                    Factory = factory
                });
            //});

            //app.Map("/core", core =>
            //{
            //    var idSvrFactory = Factory.Configure(connectionString);
                

            //    var options = new IdentityServerOptions
            //    {
            //        SiteName = "IdentityServer3 - UserService-MembershipReboot",
                
            //        SigningCertificate = Certificate.Get(),
            //        Factory = idSvrFactory,
            //        AuthenticationOptions = new AuthenticationOptions{
            //            IdentityProviders = ConfigureAdditionalIdentityProviders,
            //        }
            //    };

            //    core.UseIdentityServer(options);
            //});
        }

        public static void ConfigureAdditionalIdentityProviders(IAppBuilder app, string signInAsType)
        {
            var google = new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                SignInAsAuthenticationType = signInAsType,
                ClientId = "767400843187-8boio83mb57ruogr9af9ut09fkg56b27.apps.googleusercontent.com",
                ClientSecret = "5fWcBT0udKY7_b6E3gEiJlze"
            };
            app.UseGoogleAuthentication(google);

            var fb = new FacebookAuthenticationOptions
            {
                AuthenticationType = "Facebook",
                SignInAsAuthenticationType = signInAsType,
                AppId = "676607329068058",
                AppSecret = "9d6ab75f921942e61fb43a9b1fc25c63"
            };
            app.UseFacebookAuthentication(fb);

            var twitter = new TwitterAuthenticationOptions
            {
                AuthenticationType = "Twitter",
                SignInAsAuthenticationType = signInAsType,
                ConsumerKey = "N8r8w7PIepwtZZwtH066kMlmq",
                ConsumerSecret = "df15L2x6kNI50E4PYcHS0ImBQlcGIt6huET8gQN41VFpUCwNjM"
            };
            app.UseTwitterAuthentication(twitter);
        }
    }
}