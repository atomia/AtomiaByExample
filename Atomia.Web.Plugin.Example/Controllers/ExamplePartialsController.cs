using System.Web.Mvc;
using Atomia.Web.Base.ActionFilters;
using Atomia.Web.Plugin.Example.Managers;
using Atomia.Web.Plugin.HCP.Authorization;
using Atomia.Web.Plugin.HCP.Authorization.ActionFilterAttributes;
using Atomia.Web.Plugin.HCP.Provisioning.ActionFilterAttributes;
using Atomia.Web.Plugin.HCP.Provisioning.Controllers;


namespace Atomia.Web.Plugin.Example.Controllers
{
    [HCPHandleError(Order = 1)]
    [Internationalization(Order = 2)]
    public class ExamplePartialsController : MainController
    {
        private ExampleDevManager manager;
        //private ExampleManager manager;

        public ExamplePartialsController()
        {
            manager = new ExampleDevManager(this);
            //manager = new ExampleManager(this);
        }

        [AtomiaProvisioningAuthorize(Roles = "Administrators", ModuleName = "Provisioning", ObjectTypes = "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", Operation = AuthorizationConstants.ListServices)]
        public ActionResult Loader()
        {
            ViewData["ShowExamples"] = manager.CanAddExampleServices();

            return PartialView();
        }
    }
}
