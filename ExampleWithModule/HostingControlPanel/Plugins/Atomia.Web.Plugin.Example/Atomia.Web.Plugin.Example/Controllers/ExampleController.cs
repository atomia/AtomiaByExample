using Atomia.Web.Base.ActionFilters;
using Atomia.Web.Plugin.Example.Helpers;
using Atomia.Web.Plugin.HCP.Authorization;
using Atomia.Web.Plugin.HCP.Authorization.ActionFilterAttributes;
using Atomia.Web.Plugin.HCP.Provisioning;
using Atomia.Web.Plugin.HCP.Provisioning.ActionFilterAttributes;
using Atomia.Web.Plugin.HCP.Provisioning.Controllers;
using System.Web.Mvc;

namespace Atomia.Web.Plugin.Example.Controllers
{
    [HCPHandleError(Order = 1)]
    [AtomiaServiceChannel(Order = 2)]
    [HCPInitialization(Order = 3)]
    [Internationalization(Order = 4)]
    public class ExampleController : MainController
    {
        [AcceptVerbs(System.Net.WebRequestMethods.Http.Get, System.Net.WebRequestMethods.Http.Post)]
        public ActionResult Index()
        {
            var name = ExampleHelper.CallOperation(RouteData, "GetFirstName", "");
            ViewData["FirstName"] = name ?? "No name";

            return View();
        }

        [AtomiaProvisioningAuthorize(ModuleName = "Provisioning", ObjectTypes = "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", Operation = AuthorizationConstants.ServicesWrite)]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PerformIncrement()
        {
            var dataToReturn = ExampleHelper.CallOperation(RouteData, "Increment", "");

            return Json(dataToReturn);
        }

        [AtomiaProvisioningAuthorize(ModuleName = "Provisioning", ObjectTypes = "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", Operation = AuthorizationConstants.ServicesWrite)]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PerformDecrement()
        {
            var dataToReturn = ExampleHelper.CallOperation(RouteData, "Decrement", "");

            return Json(dataToReturn);
        }
    }
}
