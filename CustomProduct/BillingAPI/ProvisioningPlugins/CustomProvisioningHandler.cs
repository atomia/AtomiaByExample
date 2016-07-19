// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomProvisioningHandler.cs" company="Atomia AB">
//   Copyright (C) 2010 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   Defines the CustomProvisioningHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Atomia.Billing.Core.Sdk.AtomiaProvisioningApi;
using Atomia.Billing.Core.Sdk.BusinessObjects;
using Atomia.Billing.Core.Sdk.Exceptions;
using Atomia.Billing.Core.Sdk.ServiceProxies;
using Atomia.Billing.Plugins.ProvisioningPlugins.AtomiaProvisioningPlugin;
using BoAccount = Atomia.Account.Lib.BusinessObjects.Account;

namespace Atomia.Billing.Plugins.ProvisioningPlugins.Custom
{
    /// <summary>
    /// Defines the CustomProvisioningHandler type.
    /// </summary>
    public class CustomProvisioningHandler : IProvisioningHandler
    {

        /// <summary>
        /// The core service.
        /// </summary>
        private ICoreApi coreService = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomProvisioningHandler"/> class.
        /// </summary>
        public CustomProvisioningHandler()
        {
            this.coreService = new CoreApiProxy();
        }

        /// <summary>
        /// Adds the product.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <param name="mainSubscription">The main subscription.</param>
        /// <param name="account">The account.</param>
        /// <param name="productDescription">The product description.</param>
        /// <param name="additionalData">The additional data.</param>
        /// <returns>Added service.</returns>
        public ProvisioningService AddProduct(Subscription subscription, Subscription mainSubscription, BoAccount account, CustomProduct productDescription, Dictionary<string, object> additionalData)
        {
            if (productDescription.CustomProductProperties == null)
            {
                throw new AtomiaProvisioningException(string.Format("Cannot add {0} service. Custom product {1} has no properties defined in AtomiaProvisioning section.", productDescription.ProvisioningService, productDescription.Name));
            }

            if (subscription.CustomAttributes == null || !subscription.CustomAttributes.ContainsKey("CustomAttributeParam"))
            {
                throw new AtomiaProvisioningException(string.Format("Cannot add {0} service. Subscription does not have custom attribute CustomAttributeParam.", productDescription.ProvisioningService));
            }

            string customAttributeParam = subscription.CustomAttributes["CustomAttributeParam"].ToLowerInvariant();
            string customServiceName = GetProductProperty(productDescription,  "linuxInstance"); // linuxInstance or windowsInstance

            ProvisioningService parentService = this.FindService(productDescription, "parentService", "rootService", account);

            ProvisioningService customService = this.coreService.CreateService(customServiceName, parentService, account.Name);

            FillServiceProperties(customService, subscription.CustomAttributes);

            customService.properties.First(p => p.Name == "Name").propStringValue = subscription.CustomAttributes.ContainsKey("CustomAttribute1")
                         ? subscription.CustomAttributes["CustomAttribute1"]
                         : this.GetServiceName(account.Name, parentService, customServiceName, customAttributeParam);

            ProvisioningService service = null;
            string requestId = string.Empty;
            try
            {
                // Add services.

                
                customService.properties.First(p => p.Name == "CustomParam").propStringValue = customAttributeParam;

                service = this.coreService.AddServiceAsync(customService, parentService, account.Name, null, ref requestId);
                
                this.coreService.EndProvisioningRequestAsync(account.Name, requestId);

                subscription.CustomAttributes["ProvisioningRequestId"] = requestId;
                subscription.ProvisioningStatus = Subscription.ProvisioningStatusProvisioningInitiated;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(requestId))
                {
                    string reason;
                    this.coreService.CancelProvisioningRequest(out reason, account.Name, requestId);
                }

                throw new AtomiaProvisioningException(ex.Message, ex);
            }

            return service;
        }

        /// <summary>
        /// Removes the product.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <param name="mainSubscription">The main subscription.</param>
        /// <param name="account">The account.</param>
        /// <param name="productDescription">The product description.</param>
        /// <param name="additionalData">The additional data.</param>
        /// <returns>Service that was removed.</returns>
        public ProvisioningService RemoveProduct(Subscription subscription, Subscription mainSubscription, BoAccount account, CustomProduct productDescription, Dictionary<string, object> additionalData)
        {
            ProvisioningService service = this.GetService(subscription, productDescription, account);

            try
            {
                this.coreService.DeleteService(service, account.Name);
            }
            catch (Exception ex)
            {
                throw new AtomiaProvisioningException(ex.Message, ex);
            }

            return service;
        }

        /// <summary>
        /// Modifies the product.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <param name="mainSubscription">The main subscription.</param>
        /// <param name="account">The account.</param>
        /// <param name="productDescription">The product description.</param>
        /// <param name="additionalData">The additional data.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>Modified service.</returns>
        public ProvisioningService ModifyProduct(Subscription subscription, Subscription mainSubscription, BoAccount account, CustomProduct productDescription, Dictionary<string, object> additionalData)
        {
            ProvisioningService service = null;

            if (additionalData == null || !additionalData.ContainsKey("ModifyOperation"))
            {
                throw new AtomiaProvisioningException(string.Format("Cannot modify {0} service. Additional data are null or does not contain ModifyOperation key.", productDescription.ProvisioningService));
            }

            string operation = additionalData["ModifyOperation"].ToString();
            ProvisioningService parentService = this.GetService(subscription, productDescription, account);
            string serviceName = GetProductProperty(productDescription, "instanceService");
            ServiceSearchCriteria ssc = new ServiceSearchCriteria
            {
                ParentService = parentService,
                ServiceName = serviceName
            };

            ProvisioningService[] foundServices = this.coreService.FindServicesByPath(new[] { ssc }, null, account.Name, null, true);
            if (foundServices == null || foundServices.Length == 0)
            {
                throw new NullReferenceException(string.Format("Cannot modify {0} service. Service {1} cannot be found on account {2}.", productDescription.ProvisioningService, serviceName, account.Name));
            }

            service = foundServices[0];
            switch (operation)
            {
                case "Suspend":
                    parentService.properties.First(property => property.Name == "State").propStringValue = "Suspended";
                    this.coreService.CallOperation("Stop", string.Empty, service, account.Name);
                    break;

                case "Unsuspend":
                    parentService.properties.First(property => property.Name == "State").propStringValue = "Ok";
                    this.coreService.CallOperation("Start", string.Empty, service, account.Name);
                    break;

                default:
                    throw new ArgumentException(string.Format("Cannot modify {0} service. Unknown operation: {1}.", productDescription.ProvisioningService, operation));
            }

            try
            {
                return this.coreService.ModifyService(parentService, account.Name);
            }
            catch (Exception ex)
            {
                throw new AtomiaProvisioningException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Renews the product.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <param name="mainSubscription">The main subscription.</param>
        /// <param name="account">The account.</param>
        /// <param name="productDescription">The product description.</param>
        /// <param name="additionalData">The additional data.</param>
        /// <returns>Renewed service.</returns>
        public ProvisioningService RenewProduct(Subscription subscription, Subscription mainSubscription, BoAccount account, CustomProduct productDescription, Dictionary<string, object> additionalData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the provisioning service.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <param name="mainSubscription">The main subscription.</param>
        /// <param name="account">The account.</param>
        /// <param name="productDescription">The product description.</param>
        /// <param name="additionalData">The additional data.</param>
        /// <returns>THe service.</returns>
        public ProvisioningService GetProvisioningService(Subscription subscription, Subscription mainSubscription, BoAccount account, CustomProduct productDescription, Dictionary<string, object> additionalData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fills the property.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="customAttributes">The custom attributes.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        private static void FillProperty(ProvisioningService service, string propertyName, Dictionary<string, string> customAttributes, string attributeName)
        {
            if (customAttributes.ContainsKey(attributeName) && service.properties.Any(p => p.Name == propertyName))
            {
                service.properties.First(p => p.Name == propertyName).propStringValue = customAttributes[attributeName];
            }
        }

        /// <summary>
        /// Fills the service properties.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="customAttributes">The custom attributes.</param>
        private static void FillServiceProperties(ProvisioningService service, Dictionary<string, string> customAttributes)
        {
            FillProperty(service, "CustomParam1", customAttributes, "CustomAttribute1");
            FillProperty(service, "CustomParam2", customAttributes, "CustomAttribute2");
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="serviceName">The name of service.</param>
        /// <returns>Name for service.</returns>
        private static string GetName(string serviceName)
        {
            return string.Format("{0}_{1}", serviceName, DateTime.Now.ToString("yyMMddHHmmss"));
        }

        /// <summary>
        /// Gets the product property.
        /// </summary>
        /// <param name="productDescription">The product description.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>Property of the product.</returns>
        private static string GetProductProperty(CustomProduct productDescription, string propertyName)
        {
            string propertyValue = string.Empty;
            if (productDescription.CustomProductProperties.Cast<Property>().Any(customProductProperty => customProductProperty.Name == propertyName))
            {
                propertyValue = productDescription.CustomProductProperties[propertyName].Value;
            }

            if (string.IsNullOrEmpty(propertyValue))
            {
                throw new AtomiaProvisioningException(string.Format("Cannot add {0} service. Property {1} is not defined in AtomiaProvisioning section.", productDescription.ProvisioningService, propertyName));
            }

            return propertyValue;
        }

        /// <summary>
        /// Creates the name of the service.
        /// </summary>
        /// <param name="accountId">The account unique identifier.</param>
        /// <param name="parentService">The parent service.</param>
        /// <param name="serviceName">Name of the _service.</param>
        /// <param name="name">The name.</param>
        /// <returns>Name for service</returns>
        private string GetServiceName(string accountId, ProvisioningService parentService, string serviceName, string name)
        {
            string propertyValue = GetName(name);
            ServiceSearchCriteria ssc = new ServiceSearchCriteria
            {
                ParentService = parentService,
                ServiceName = serviceName
            };

            ProvisioningService[] foundServices = this.coreService.FindServicesByPath(new[] { ssc }, null, accountId, null, true);
            if (foundServices.Length > 0)
            {
                while (foundServices.Any(s => s.name == propertyValue))
                {
                    propertyValue = GetName(name);
                }
            }

            return propertyValue;
        }

        /// <summary>
        /// Adds the service.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <param name="productDescription">The product description.</param>
        /// <param name="parentServiceKey">The parent service key.</param>
        /// <param name="serviceKey">The service key.</param>
        /// <param name="name">The name.</param>
        /// <param name="requestId">The request unique identifier.</param>
        /// <returns>Added service</returns>
        private ProvisioningService CreateService(BoAccount account, CustomProduct productDescription, string parentServiceKey, string serviceKey, string name, ref string requestId)
        {
            ProvisioningService parentService = this.FindService(productDescription, parentServiceKey, "rootService",  account);
            ProvisioningService service = this.PrepareService(productDescription, account, serviceKey, parentService);

            string nameValue = this.GetServiceName(account.Name, parentService, serviceKey, name);
            service.properties.First(p => p.Name == "CustomParam1").propStringValue = nameValue;
            service.properties.First(p => p.Name == "CustomParam2").propStringValue = nameValue;

            if (!string.IsNullOrEmpty(requestId))
            {
                return this.coreService.AddServiceAsync(service, parentService, account.Name, null, ref requestId);
            }

            return this.coreService.AddService(service, parentService, account.Name, null);
        }

        /// <summary>
        /// Prepares the service.
        /// </summary>
        /// <param name="productDescription">The product description.</param>
        /// <param name="account">The account.</param>
        /// <param name="serviceKey">The service key.</param>
        /// <param name="parentService">The parent service.</param>
        /// <returns>Prepared service.</returns>
        private ProvisioningService PrepareService(CustomProduct productDescription, BoAccount account, string serviceKey, ProvisioningService parentService)
        {
            string serviceName = string.Empty;

            if (productDescription.CustomProductProperties.Cast<Property>().Any(customProductProperty => customProductProperty.Name == serviceKey))
            {
                serviceName = productDescription.CustomProductProperties[serviceKey].Value;
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                throw new AtomiaProvisioningException(string.Format("Cannot add {0} service. Property {1} is not defined in AtomiaProvisioning section.", productDescription.ProvisioningService, serviceKey));
            }

            return this.coreService.CreateService(serviceName, parentService, account.Name);
        }

        /// <summary>
        /// Finds the service.
        /// </summary>
        /// <param name="productDescription">The product description.</param>
        /// <param name="parentServiceKey">The parent service key.</param>
        /// <param name="rootServiceKey">The root service key.</param>
        /// <param name="account">The account.</param>
        /// <returns>Service that was found using parent services.</returns>
        private ProvisioningService FindService(CustomProduct productDescription, string parentServiceKey, string rootServiceKey, BoAccount account)
        {
            string parentServiceName = GetProductProperty(productDescription, parentServiceKey);
            string servicePath = productDescription.CustomProductProperties[rootServiceKey].Value;
            ServiceSearchCriteria ssc = new ServiceSearchCriteria
                                        {
                                            ParentService = null,
                                            ServiceName = parentServiceName,
                                            ServicePath = servicePath
                                        };

            ProvisioningService[] parentService = this.coreService.FindServicesByPath(new[] { ssc }, null, account.Name, null, true);
            if (parentService == null || parentService.Length < 1)
            {
                throw new AtomiaProvisioningException(string.Format("Cannot add {0} service. Account {1} doesn't have {2} service provisioned.", productDescription.ProvisioningService, account.Name, parentServiceName));
            }

            return parentService[0];
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="subscription">The subscription.</param>
        /// <param name="productDescription">The product description.</param>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        private ProvisioningService GetService(Subscription subscription, CustomProduct productDescription, BoAccount account)
        {
            if (subscription.CustomAttributes == null || !subscription.CustomAttributes.ContainsKey("ServiceId"))
            {
                throw new AtomiaProvisioningException(string.Format("Cannot get service {0}. Subscription has no custom attribute ServiceId.", productDescription.ProvisioningService));
            }

            string serviceId = subscription.CustomAttributes["ServiceId"];
            ProvisioningService service = this.coreService.GetServiceById(serviceId, account.Name);

            if (service == null)
            {
                throw new AtomiaProvisioningException(string.Format("Cannot find service with id {0} for account {1}.", serviceId, account.Name));
            }

            return service;
        }
    }
}