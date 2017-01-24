using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Atomia.Web.Base.Validation;
using Atomia.Web.Plugin.Example.Models;
using Atomia.Web.Plugin.HCP.Provisioning;
using Atomia.Web.Plugin.HCP.Provisioning.Helpers;
using Atomia.Web.Plugin.HCP.Provisioning.Helpers.ActionTrail;
using Atomia.Web.Plugin.HCP.Provisioning.Helpers.AuditLog;
using Atomia.Web.Plugin.ServiceReferences;
using Atomia.Web.Plugin.ServiceReferences.CoreAPI;

namespace Atomia.Web.Plugin.Example.Managers
{
    public class ExampleManager
    {
        private Controller controller;
        private AtomiaServices exampleServiceData;
        private AtomiaServices csBaseServiceData;
        private ICoreApi coreApi;
        private string accountId;
        private string serviceId;

        public ExampleManager(Controller controller)
        {
            var routeData = controller.RouteData;
            var area = routeData.DataTokens["area"].ToString();
            var controllerName = routeData.DataTokens["controller"].ToString();
            var action = routeData.DataTokens["action"].ToString();

            this.controller = controller;
            coreApi = AtomiaServiceChannelManager.GetCoreService();
            accountId = routeData.Values["accountID"].ToString();
            serviceId = routeData.Values["serviceID"].ToString();
            
            var provisioningDescriptionId = AtomiaServicesManager.FetchProvisioningDescriptionID(accountId);
            exampleServiceData = AtomiaServicesManager.FetchServiceData("Example Complex Service", provisioningDescriptionId, area, controllerName, action);
            csBaseServiceData = AtomiaServicesManager.FetchServiceData("Hosting Complex Service", provisioningDescriptionId, area, controllerName, action);
        }
        
        public void DeleteExample(ExampleModel example)
        {
            try
            {
                var service = coreApi.GetServiceById(example.LogicalID, accountId);

                if (service == null)
                {
                    throw new Exception(controller.LocalResource("Index", "ExampleDoesNotExist"));
                }
                
                coreApi.DeleteService(service, accountId);

                LogDeleted(service, example);
            }
            catch (Exception ex)
            {
                throw new AtomiaServerSideValidationException("DeleteExample", ex.Message);
            }
        }

        public void AddExample(ExampleModel example)
        {
            Validate(example);
            
            try
            {
                var possiblePackageServices = coreApi.FindServicesByPath(new ServiceSearchCriteria[] {
                    new ServiceSearchCriteria {
                        ParentService = null,
                        ServicePath = csBaseServiceData.Path,
                        ServiceName = csBaseServiceData.ServiceName
                    }
                }, null, accountId, "", true);
                var packageServices = PackageSelectHelper.FilterServicesToSelectedPackage(controller.RouteData, possiblePackageServices);

                var newService = coreApi.CreateService(exampleServiceData.ServiceName, packageServices.First(), accountId);
                SetServicePropertyValue(newService, "Name", example.Name);

                var addedService = coreApi.AddService(newService, packageServices.First(), accountId, null);
                example.LogicalID = addedService.logicalId;

                LogAdded(addedService, example);
            }
            catch (AtomiaServerSideValidationException e)
            {
                var error = e.Errors.First();
                throw new AtomiaServerSideValidationException(error.PropertyName, error.ErrorMessage);
            }
            catch (Exception e)
            {
                var errorMessage = String.Format(controller.LocalResource("Add", "CantAddExample"), example.Name, e.Message);
                throw new AtomiaServerSideValidationException("Name", errorMessage);
            }
        }

        public void EditExample(ExampleModel example)
        {
            Validate(example);

            try
            {
                var serviceId = example.LogicalID;
                var exampleBefore = FetchExample(serviceId);

                var possibleServices = coreApi.FindServicesByPath(new ServiceSearchCriteria[] { new ServiceSearchCriteria {
                        ParentService = null,
                        ServicePath = exampleServiceData.Path,
                        ServiceName = exampleServiceData.ServiceName
                    }
                }, null, accountId, "", true);
                var services = PackageSelectHelper.FilterServicesToSelectedPackage(controller.RouteData, possibleServices);
                var existingService = services.First(svc => svc.logicalId == serviceId);
                if (!services.Any(svc => svc.logicalId == serviceId))
                {
                    existingService = coreApi.GetServiceById(serviceId, accountId);
                }
                SetServicePropertyValue(existingService, "Name", example.Name);
                var editedService = coreApi.ModifyService(existingService, accountId);
                
                LogUpdated(exampleBefore, FetchExample(serviceId));
            }
            catch (AtomiaServerSideValidationException e)
            {
                var error = e.Errors.First();
                throw new AtomiaServerSideValidationException(error.PropertyName, error.ErrorMessage);
            }
            catch (Exception e)
            {
                var errorMessage = String.Format(controller.LocalResource("Edit", "CantEditExample"), example.Name, e.Message);
                throw new AtomiaServerSideValidationException("Name", errorMessage);
            }
        }

        public List<ExampleModel> FetchObjectsWithPaging(string search, string iDisplayStart, string iDisplayLength, int sortColumn, string sortDirection, out long total)
        {
            var examples = new List<ExampleModel>();
            total = 0;

            try
            {
                var serviceSearchCriteria = new ServiceSearchCriteria[] {
                    new ServiceSearchCriteria {
                        ParentService = null,
                        ServicePath = exampleServiceData.Path,
                        ServiceName = exampleServiceData.ServiceName
                    }
                };
                var searchDictionary = search.Length > 0
                    ? new Dictionary<string, string> { { exampleServiceData.ServiceProperties["Name"], "%*" + search + "%*" } }
                    : null;
                var exampleServices = coreApi.FindServicesByPathWithPaging(
                    out total,
                    serviceSearchCriteria, 
                    searchDictionary, 
                    accountId, 
                    exampleServiceData.ServiceProperties["Name"],
                    sortDirection.ToLower() == "asc",
                    Convert.ToInt32(iDisplayStart) / Convert.ToInt32(iDisplayLength),
                    Convert.ToInt32(iDisplayLength));

                exampleServices = PackageSelectHelper.FilterServicesToSelectedPackage(controller.RouteData, exampleServices);

                foreach (var exampleService in exampleServices)
                {
                    examples.Add(new ExampleModel {
                        LogicalID = exampleService.logicalId,
                        Name = GetServicePropertyValue(exampleService, "Name"),
                        Status = GetServicePropertyValue(exampleService, "Status")
                    });
                }
            }
            catch (Exception ex)
            {
                HostingControlPanelLogger.LogHostingControlPanelException(ex);
                total = 0;
            }

            return examples;
        }

        public ExampleModel FetchExample(string serviceId)
        {
            try
            {
                var exampleService = coreApi.GetServiceById(serviceId, accountId);

                return new ExampleModel
                {
                    LogicalID = exampleService.logicalId,
                    Name = GetServicePropertyValue(exampleService, "Name"),
                    Status = GetServicePropertyValue(exampleService, "Status"),
                };
            }
            catch (Exception ex)
            {
                HostingControlPanelLogger.LogHostingControlPanelException(ex);
            }

            return new ExampleModel();
        }

        public static string FetchPluginParameterFromConfig(string pluginParameterName)
        {
            return PluginSettingsFetcher.FetchSetting(pluginParameterName, Assembly.GetExecutingAssembly().CodeBase);
        }

        public bool CanAddExampleServices()
        {
            return PackageLimiter.CheckGlobalAddingPossibilities("Example", controller.RouteData.Values).isPossible;
        }

        private void Validate(ExampleModel example)
        {
            var errors = DataAnnotationsValidationRunner.GetErrors(example);

            if (errors.Any())
            {
                throw new AtomiaServerSideValidationException(errors);
            }
        }

        private string GetServicePropertyValue(ProvisioningService service, string propertyName)
        {
            return service.properties.FirstOrDefault(p => p.Name == exampleServiceData.ServiceProperties[propertyName]).propStringValue;
        }

        private void SetServicePropertyValue(ProvisioningService service, string propertyName, string value)
        {
            service.properties.Single(p => p.Name == exampleServiceData.ServiceProperties[propertyName]).propStringValue = value;
        }

        private void LogDeleted(ProvisioningService service, ExampleModel example)
        {
            HostingControlPanelAuditLogger.Log("EXAMPLE_DELETED", $"Deleted example with name {GetServicePropertyValue(service, "Name")}.",
                accountId, service.logicalId, new Dictionary<string, object> { { "Example", example } });
        }

        private void LogAdded(ProvisioningService service, ExampleModel example)
        {
            HostingControlPanelAuditLogger.Log("EXAMPLE_ADDED", $"Created example with name {example.Name}.",
                accountId, service.logicalId, new Dictionary<string, object> { { "Exampe", example } });
        }

        private void LogUpdated(ExampleModel before, ExampleModel after)
        {
            HostingControlPanelAuditLogger.Log("EXAMPLE_UPDATED", $"Updated example with name {after.Name}.",
                accountId, serviceId, new Dictionary<string, object> { { "Before", before }, { "After", after } });
        }
    }
}
