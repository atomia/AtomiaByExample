using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Atomia.Web.Base.ActionFilters;
using Atomia.Web.Base.Validation;
using Atomia.Web.Plugin.Example.Models;
using Atomia.Web.Plugin.HCP.Authorization;
using Atomia.Web.Plugin.HCP.Authorization.ActionFilterAttributes;
using Atomia.Web.Plugin.HCP.Provisioning;
using Atomia.Web.Plugin.HCP.Provisioning.ActionFilterAttributes;
using Atomia.Web.Plugin.HCP.Provisioning.Controllers;
using Atomia.Web.Plugin.HCP.Provisioning.Helpers.ActionTrail;
using Atomia.Web.Plugin.Example.Managers;

namespace Atomia.Web.Plugin.Example.Controllers
{
    [HCPHandleError(Order = 1)]
    [AtomiaServiceChannel(Order = 2)]
    [HCPInitialization(Order = 3)]
    [Internationalization(Order = 4)]
    [AccountValidation(Order = 5, Roles = "Administrators")]
    public class ExampleController : MainController
    {
        [AtomiaProvisioningAuthorize(Roles = "Administrators", ModuleName = "Provisioning", ObjectTypes = "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", Operation = AuthorizationConstants.ListServices)]
        public ActionResult Index()
        {
            if (!PackageLimiter.CheckGlobalAddingPossibilities("Example", RouteData.Values).isPossible)
            {
                throw new HttpException(401, String.Empty);
            }

            ViewData["canAdd"] = CheckCanAdd();
            ViewData["canEdit"] = CheckCanEdit();
            ViewData["canDelete"] = CheckCanDelete();
            
            return View();
        }

        [AtomiaProvisioningAuthorize(Roles = "Administrators", ModuleName = "Provisioning", ObjectTypes = "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", Operation = AuthorizationConstants.ListServices)]
        public ActionResult Search(string sSearch, string iDisplayStart, string iDisplayLength, string sEcho, string iSortCol_0, string sSortDir_0)
        {
            long total;
            var exampleManager = new ExampleManager(this);
            var examples = exampleManager.FetchObjectsWithPaging(sSearch, iDisplayStart, iDisplayLength, Convert.ToInt32(iSortCol_0), sSortDir_0, out total);
            
            ViewData["canAdd"] = CheckCanAdd();
            ViewData["canEdit"] = CheckCanEdit();
            ViewData["canDelete"] = CheckCanDelete();

            var counter = 0;
            var jsSerailizer = new JavaScriptSerializer();
            var result = new
            {
                sEcho,
                iTotalRecords = examples.Count(),
                iTotalDisplayRecords = total,
                aaData = new string[examples.Count][]
            };
            
            foreach (var exampleData in examples)
            {
                result.aaData[counter++] = new[] 
                {
                    exampleData.Status,
                    exampleData.Name,
                    jsSerailizer.Serialize(new
                    {
                        canEdit = (bool)ViewData["canEdit"],
                        canDelete = (bool)ViewData["canDelete"],
                        logicalID = exampleData.LogicalID,
                    }),
                };
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AtomiaProvisioningAuthorize(Roles = "Administrators", ModuleName = "Provisioning", ObjectTypes = "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", Operation = AuthorizationConstants.AddServices)]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Add()
        {
            try
            {
                return View(new ExampleModel());   
            }
            catch (Exception e)
            {
                HostingControlPanelLogger.LogHostingControlPanelException(e);
            }

            return RedirectToAction("Index", new { controller = "Example" });
        }

        [AtomiaProvisioningAuthorize(Roles = "Administrators", ModuleName = "Provisioning", ObjectTypes = "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", Operation = AuthorizationConstants.AddServices)]
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public ActionResult Add(ExampleModel example)
        {
            try
            {
                var exampleManager = new ExampleManager(this);
                exampleManager.AddExample(example);
            }
            catch (AtomiaServerSideValidationException ex)
            {
                ex.AddModelStateErrors(ModelState, string.Empty);
            }
            
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(example);
                }
            }
            catch (Exception e)
            {
                HostingControlPanelLogger.LogHostingControlPanelException(e);
            }
                        
            return RedirectToAction("Index", new { controller = "Example" });
        }

        [AtomiaProvisioningAuthorize(Roles = "Administrators", ModuleName = "Provisioning", ObjectTypes = "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", Operation = AuthorizationConstants.ModifyServices + ", " + AuthorizationConstants.DeleteServices + ", " + AuthorizationConstants.AddServices)]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Edit(string serviceID)
        {
            try
            {
                var serviceId = this.RouteData.Values["serviceID"].ToString();
                var exampleManager = new ExampleManager(this);
                var example = exampleManager.FetchExample(serviceId);

                return View(example);   
            }
            catch (Exception e)
            {
                HostingControlPanelLogger.LogHostingControlPanelException(e);
            }

            return RedirectToAction("Index", new { controller = "Example" });
        }

        [AtomiaProvisioningAuthorize(Roles = "Administrators", ModuleName = "Provisioning", ObjectTypes = "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", Operation = AuthorizationConstants.ModifyServices + ", " + AuthorizationConstants.DeleteServices + ", " + AuthorizationConstants.AddServices)]
        [AcceptVerbs(HttpVerbs.Post), ValidateInput(false)]
        public ActionResult Edit(ExampleModel example)
        {
            try
            {
                var exampleManager = new ExampleManager(this);
                exampleManager.EditExample(example);
                return RedirectToAction("Index", new { controller = "Example" });
            }
            catch (AtomiaServerSideValidationException ex)
            {
                ex.AddModelStateErrors(ModelState, string.Empty);
            }
            
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(example);
                }
            }
            catch (Exception e)
            {
                HostingControlPanelLogger.LogHostingControlPanelException(e);
            }
            
            return RedirectToAction("Index", new { controller = "Example" });
        }

        [AtomiaProvisioningAuthorize(Roles = "Administrators", ModuleName = "Provisioning", ObjectTypes = "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", Operation = AuthorizationConstants.DeleteServices)]
        public ActionResult Delete(string serviceID)
        {
            var result = new Dictionary<string, string>();
            var example = new ExampleModel()
            {
                LogicalID = serviceID
            };
            
            try
            {
                var exampleManager = new ExampleManager(this);
                exampleManager.DeleteExample(example);
            }
            catch (AtomiaServerSideValidationException ex)
            {
                result.Add("succeeded", "false");
                result.Add("error", ex.Errors.First().ErrorMessage);
                ex.AddModelStateErrors(ModelState, string.Empty);
            }

            if (ModelState.IsValid)
            {
                result.Add("succeeded", "true");
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private bool CheckCanEdit()
        {
            var editAuthorization = new IdentityAuthorization("Provisioning", "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", AuthorizationConstants.ModifyServices + ", " + AuthorizationConstants.DeleteServices + ", " + AuthorizationConstants.AddServices, RouteData.Values, HttpContext);
            return (!editAuthorization.Check() && !editAuthorization.CheckRoles(new string[] { "Administrators" }, User)) ? false : true;
        }

        private bool CheckCanDelete()
        {
            var deleteAuthorization = new IdentityAuthorization("Provisioning", "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", AuthorizationConstants.DeleteServices, RouteData.Values, HttpContext);
            return (!deleteAuthorization.Check() && !deleteAuthorization.CheckRoles(new string[] { "Administrators" }, User)) ? false : true;
        }

        private bool CheckCanAdd()
        {
            var addAuthorization = new IdentityAuthorization("Provisioning", "http://schemas.atomia.com/atomia/2009/04/provisioning/claims/account/{account_id}", AuthorizationConstants.AddServices, RouteData.Values, HttpContext);
            return addAuthorization.Check();
        }
    }
}
