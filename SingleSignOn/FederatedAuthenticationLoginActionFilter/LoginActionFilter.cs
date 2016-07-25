// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginActionFilter.cs" company="Atomia AB">
//   Copyright (C) 2010 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   Defines the LoginActionFilter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Atomia.Identity.Base.StsExtendabilityBase;
using Atomia.Custom.FederatedAuthenticationLoginActionFilter.Configuration;
using Microsoft.IdentityModel.Protocols.WSFederation;

namespace Atomia.Custom.FederatedAuthenticationLoginActionFilter
{
    /// <summary>
    /// Defines the LoginActionFilter type.
    /// </summary>
    public class LoginActionFilter : ILoginActionFilter
    {
        /// <summary>
        /// Called when [action executed].
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        /// <summary>
        /// Called when [action executing].
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.RequestContext.HttpContext.Request;
            if (request.UrlReferrer != null &&
                LoginActionFilterConfiguration.Instance != null &&
                !string.IsNullOrEmpty(LoginActionFilterConfiguration.Instance.Host) &&
                !string.IsNullOrEmpty(LoginActionFilterConfiguration.Instance.Url) &&
                !string.IsNullOrEmpty(LoginActionFilterConfiguration.Instance.Realm) &&
                request.UrlReferrer.Host == LoginActionFilterConfiguration.Instance.Host &&
                request.QueryString.AllKeys.Any(key => key == "ReturnUrl"))
            {
                SignInRequestMessage signInMessage = new SignInRequestMessage(
                    new Uri(LoginActionFilterConfiguration.Instance.Url),
                    LoginActionFilterConfiguration.Instance.Realm,
                    request.QueryString["ReturnUrl"])
                {
                    Context = request.QueryString.ToString()
                };

                filterContext.Result = new RedirectResult(signInMessage.RequestUrl);
            }
        }

        /// <summary>
        /// Called when [result executed].
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        /// <summary>
        /// Called when [result executing].
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }
    }
}