using Atomia.Web.Plugin.HCP.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Atomia.Web.Plugin.ServiceReferences;
using Atomia.Web.Plugin.ServiceReferences.CoreAPI;
using Atomia.Web.Plugin.HCP.Provisioning.Helpers;

namespace Atomia.Web.Plugin.Example.Helpers
{
    public static class ExampleHelper
    {
        public static string CallOperation(RouteData routeData, string operationName, string operationArgument, ProvisioningService provisioningService = null)
        {
            var accountId = routeData.Values["accountID"].ToString();
            var coreService = AtomiaServiceChannelManager.GetCoreService(false);

            if (provisioningService == null)
            {
                provisioningService = GetService(routeData);
            }

            return coreService.CallOperation(operationName, operationArgument, provisioningService, accountId);
        }

        public static ProvisioningService GetService(RouteData routeData, string serviceName = "Example Simple Service")
        {
            var coreService = AtomiaServiceChannelManager.GetCoreService(false);
            var accountId = routeData.Values["accountId"].ToString();
            var fetchedProvisioningDescriptionID = AtomiaServicesManager.FetchProvisioningDescriptionID(accountId);
            var serviceData = AtomiaServicesManager.FetchServiceData(
                                serviceName,
                                fetchedProvisioningDescriptionID,
                                routeData.DataTokens["area"].ToString(),
                                routeData.Values["controller"].ToString(),
                                routeData.Values["action"].ToString());
            var baseService = GetRootService(routeData);
            ProvisioningService simpleService = null;

            if (baseService != null)
            {
                var servicesList = SearchServices(coreService, baseService, serviceData.ServiceName, accountId, serviceData.Path);

                if (servicesList != null && servicesList.Length > 0)
                {
                    simpleService = servicesList[0];
                }
            }

            return simpleService;
        }

        private static ProvisioningService[] SearchServices(
            ICoreApi coreApi,
            ProvisioningService parentService,
            string serviceName,
            string accountName,
            string servicePath = null,
            Dictionary<string, string> properties = null)
        {
            if (parentService != null)
            {
                servicePath = servicePath.Substring(servicePath.LastIndexOf(parentService.name) + parentService.name.Length);
            }

            var criteria = new ServiceSearchCriteria
            {
                ParentService = parentService,
                ServiceName = serviceName,
                ServicePath = servicePath
            };

            return coreApi.FindServicesByPath(new[] { criteria }, properties, accountName, null, true);
        }

        private static AtomiaServices GetServiceData(
            RouteData routeData,
            string fetchedProvisioningDescriptionID,
            string serviceName = "Example Simple Service")
        {
            return AtomiaServicesManager.FetchServiceData(
                     serviceName,
                     fetchedProvisioningDescriptionID,
                     routeData.DataTokens["area"].ToString(),
                     routeData.Values["controller"].ToString(),
                     routeData.Values["action"].ToString());
        }

        private static ProvisioningService GetRootService(RouteData routeData)
        {
            var coreService = AtomiaServiceChannelManager.GetCoreService(false);
            var fetchedProvisioningDescriptionID = AtomiaServicesManager.FetchProvisioningDescriptionID(routeData.Values["accountID"].ToString());
            var fetchedRootFolderServiceData = AtomiaServicesManager.FetchServiceData(
                    "Hosting Complex Service",
                    fetchedProvisioningDescriptionID,
                    routeData.DataTokens["area"].ToString(),
                    routeData.Values["controller"].ToString(),
                    routeData.Values["action"].ToString());
            var serviceSearchCriteria = new[]
            {
                new ServiceSearchCriteria
                {
                    ParentService = null,
                    ServicePath = fetchedRootFolderServiceData.Path,
                    ServiceName = fetchedRootFolderServiceData.ServiceName
                }
            };
            var fetchedServices = coreService.FindServicesByPath(serviceSearchCriteria, null, routeData.Values["accountID"].ToString(), "", true);

            return PackageSelectHelper.FilterServicesToSelectedPackage(routeData, fetchedServices).FirstOrDefault();
        }
    }
}
