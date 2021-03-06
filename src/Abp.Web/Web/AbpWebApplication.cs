﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using Adorable.Collections.Extensions;
using Adorable.Dependency;
using Adorable.Localization;
using Adorable.MultiTenancy;
using Adorable.Reflection;
using Adorable.Runtime.Security;
using Adorable.Threading;

namespace Adorable.Web
{
    /// <summary>
    /// This class is used to simplify starting of ABP system using <see cref="AbpBootstrapper"/> class..
    /// Inherit from this class in global.asax instead of <see cref="HttpApplication"/> to be able to start ABP system.
    /// </summary>
    public abstract class AbpWebApplication : HttpApplication
    {
        /// <summary>
        /// Gets a reference to the <see cref="AbpBootstrapper"/> instance.
        /// </summary>
        protected AbpBootstrapper AbpBootstrapper { get; private set; }

        protected AbpWebApplication()
        {
            AbpBootstrapper = new AbpBootstrapper();
        }

        /// <summary>
        /// This method is called by ASP.NET system on web application's startup.
        /// </summary>
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            ThreadCultureSanitizer.Sanitize();

            AbpBootstrapper.IocManager.RegisterIfNot<IAssemblyFinder, WebAssemblyFinder>();
            AbpBootstrapper.Initialize();
        }

        /// <summary>
        /// This method is called by ASP.NET system on web application shutdown.
        /// </summary>
        protected virtual void Application_End(object sender, EventArgs e)
        {
            AbpBootstrapper.Dispose();
        }

        /// <summary>
        /// This method is called by ASP.NET system when a session starts.
        /// </summary>
        protected virtual void Session_Start(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This method is called by ASP.NET system when a session ends.
        /// </summary>
        protected virtual void Session_End(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This method is called by ASP.NET system when a request starts.
        /// </summary>
        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
            var langCookie = Request.Cookies["Adorable.Localization.CultureName"];
            if (langCookie != null && GlobalizationHelper.IsValidCultureCode(langCookie.Value))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(langCookie.Value);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(langCookie.Value);
            }
            else if (!Request.UserLanguages.IsNullOrEmpty())
            {
                var firstValidLanguage = Request
                    .UserLanguages
                    .FirstOrDefault(GlobalizationHelper.IsValidCultureCode);

                if (firstValidLanguage != null)
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(firstValidLanguage);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(firstValidLanguage);
                }
            }
        }

        /// <summary>
        /// This method is called by ASP.NET system when a request ends.
        /// </summary>
        protected virtual void Application_EndRequest(object sender, EventArgs e)
        {

        }

        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            TrySetTenantId();
        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Tries to set current tenant Id.
        /// </summary>
        protected virtual void TrySetTenantId()
        {
            var claimsPrincipal = User as ClaimsPrincipal;
            if (claimsPrincipal == null)
            {
                return;
            }

            var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                return;
            }

            var tenantIdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.TenantId);
            if (tenantIdClaim != null)
            {
                return;
            }

            var tenantId = ResolveTenantIdOrNull();
            if (!tenantId.HasValue)
            {
                return;
            }

            claimsIdentity.AddClaim(new Claim(AbpClaimTypes.TenantId, tenantId.Value.ToString(CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Resolves current tenant id or returns null if can not.
        /// </summary>
        protected virtual int? ResolveTenantIdOrNull()
        {
            using (var tenantIdResolver = AbpBootstrapper.IocManager.ResolveAsDisposable<ITenantIdResolver>())
            {
                return tenantIdResolver.Object.TenantId;
            }
        }
    }
}
