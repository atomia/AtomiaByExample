using System.Web.Mvc;
using System.Web.Routing;
using Atomia.Web.Base.ActionFilters;
using Atomia.Web.Plugin.HCP.Authorization;
using Atomia.Web.Plugin.HCP.Authorization.ActionFilterAttributes;
using Atomia.Web.Plugin.HCP.Provisioning;
using Atomia.Web.Plugin.HCP.Provisioning.ActionFilterAttributes;
using Atomia.Web.Plugin.HCP.Provisioning.Controllers;

namespace Atomia.Web.Plugin.Example.Controllers
{
    [HCPHandleError(Order = 1)]
    [Internationalization(Order = 2)]
    public class ExamplePartialsController : MainController
    {
        [AtomiaProvisioningAuthorize(Roles = "Administrators", ModuleName = "Provisioning", ObjectTypes = "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", Operation = AuthorizationConstants.ListServices)]
        public ActionResult Loader()
        {
            ViewData["ShowExamples"] = true;

            /* Normally you would check whether the Example service can be added. 
               This depends on having it configured in atomiaServices and the provisioning description. */
            // ViewData["ShowExamples"] = PackageLimiter.CheckGlobalAddingPossibilities("Example", RouteData.Values).isPossible;

            return PartialView();
        }
    }
}
