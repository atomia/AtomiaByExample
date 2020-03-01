using Atomia.Web.Base.ActionFilters;
using Atomia.Web.Plugin.Example.Helpers;
using Atomia.Web.Plugin.HCP.Authorization;
using Atomia.Web.Plugin.HCP.Authorization.ActionFilterAttributes;
using Atomia.Web.Plugin.HCP.Provisioning.ActionFilterAttributes;
using Atomia.Web.Plugin.HCP.Provisioning.Controllers;
using System.Web.Mvc;
using Atomia.Web.Plugin.ServiceReferences;

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
            var service = ExampleHelper.GetServiceModel(RouteData);

            ViewData["CanAdd"] = ExampleHelper.CanAdd(RouteData) ? "1" : "0";
            ViewData["Exists"] = null == service ? "0" : "1";

            if (null != service)
            {
                ViewData["FirstName"] = service.FirstName ?? "No name";
                ViewData["LastName"] = service.LastName ?? "No name";
                ViewData["Number"] = service.Number ?? "0";
            }

            return View();
        }

        [AtomiaProvisioningAuthorize(ModuleName = "Provisioning", ObjectTypes = "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", Operation = AuthorizationConstants.ServicesWrite)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create()
        {
            var atomiaAccountApiClient = AtomiaServiceChannelManager.GetAccountService();
            var account = atomiaAccountApiClient.GetAccountByName(RouteData.Values["accountId"].ToString());

            ExampleHelper.Create(RouteData, account.FirstName, account.LastName);

            return RedirectToAction("Index", new { area = "Example", controller = "Example" });
        }

        [AtomiaProvisioningAuthorize(ModuleName = "Provisioning", ObjectTypes = "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", Operation = AuthorizationConstants.ServicesWrite)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete()
        {
            ExampleHelper.Delete(RouteData);

            return RedirectToAction("Index", new { area = "Example", controller = "Example" });
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
