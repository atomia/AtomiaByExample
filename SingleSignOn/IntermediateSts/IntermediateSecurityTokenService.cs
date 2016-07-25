// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntermediateSecurityTokenService.cs" company="Atomia AB">
//   Copyright (C) 2010 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   Defines the IntermediateSecurityTokenService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using Atomia.Custom.IntermediateSts.Configuration;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Protocols.WSIdentity;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Atomia.Custom.IntermediateSts
{
    public interface IClaimProvider : IDisposable
    {
        /// <summary>
        /// Determines whether this instance can handle the specified claim types.
        /// </summary>
        /// <param name="claimTypes">The claim types.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>List of claim types that this instance can handle.</returns>
        string[] CanHandle(string[] claimTypes, IClaimsIdentity identity);

        /// <summary>
        /// Updates the identity with acquired claim types.
        /// </summary>
        /// <param name="claimTypes">The claim types.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>Updated identity</returns>
        IClaimsIdentity UpdateIdentity(string[] claimTypes, IClaimsIdentity identity);
    }
    /// <summary>
    /// Defines the IntermediateSecurityTokenService type.
    /// </summary>
    public class IntermediateSecurityTokenService : SecurityTokenService
    {
        /// <summary>
        /// Set enableAppliesToValidation to true to enable only the RP URLs specified in the ActiveClaimsAwareApps array to get a token from this STS
        /// </summary>
        private const bool EnableAppliesToValidation = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntermediateSecurityTokenService"/> class.
        /// </summary>
        /// <param name="securityTokenServiceConfiguration">The security token service configuration.</param>
        public IntermediateSecurityTokenService(SecurityTokenServiceConfiguration securityTokenServiceConfiguration) : base(securityTokenServiceConfiguration)
        {
        }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <param name="request">The request.</param>
        /// <returns>Returns scope.</returns>
        protected override Scope GetScope(IClaimsPrincipal principal, RequestSecurityToken request)
        {
            ValidateAppliesTo(request.AppliesTo);

            Scope scope = new Scope(request.AppliesTo.Uri.OriginalString, SecurityTokenServiceConfiguration.SigningCredentials)
                          {
                              AppliesToAddress = request.AppliesTo.Uri.AbsoluteUri
                          };

            string encryptingCertificateName = GetEncryptingCertificateName(request.AppliesTo.Uri.ToString());
            if (!string.IsNullOrEmpty(encryptingCertificateName))
            {
                scope.EncryptingCredentials = new X509EncryptingCredentials(CertificateUtil.GetCertificate(StoreName.My, StoreLocation.LocalMachine, encryptingCertificateName));
            }
            else
            {
                scope.TokenEncryptionRequired = false;
            }

            if (Uri.IsWellFormedUriString(request.ReplyTo, UriKind.Absolute))
            {
                scope.ReplyToAddress = request.AppliesTo.Uri.Host != new Uri(request.ReplyTo).Host ? request.AppliesTo.Uri.AbsoluteUri : request.ReplyTo;
            }
            else
            {
                Uri resultUri = null;
                scope.ReplyToAddress = Uri.TryCreate(request.AppliesTo.Uri, request.ReplyTo, out resultUri) ? resultUri.AbsoluteUri : request.AppliesTo.Uri.ToString();
            }

            return scope;
        }

        /// <summary>
        /// Gets the output claims identity.
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <param name="request">The request.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>Returns output claims identity.</returns>
        protected override IClaimsIdentity GetOutputClaimsIdentity(IClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            if (null == principal)
            {
                throw new ArgumentNullException("principal");
            }

            const string ContainerName = "defaultContainer";
            UnityConfigurationSection section = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;
            if (!section.Containers.Any(container => container.Name == ContainerName))
            {
                throw new ArgumentException("No defaultContainer in unity configuration section.");
            }

            UnityContainer unityContainer = new UnityContainer();
            section.Configure(unityContainer, ContainerName);

            IClaimProvider claimProvider = unityContainer.Resolve<IClaimProvider>();

            ClaimsIdentity outputIdentity = new ClaimsIdentity(principal.Identity);
            string[] canHandleTypes = claimProvider.CanHandle(new[] { System.IdentityModel.Claims.ClaimTypes.Name }, outputIdentity);
            outputIdentity = claimProvider.UpdateIdentity(canHandleTypes, outputIdentity) as ClaimsIdentity;
            Logger.Write("Created claims identity: " + outputIdentity.Name);

            return outputIdentity;
        }

        /// <summary>
        /// Gets the name of the encrypting certificate.
        /// </summary>
        /// <param name="relayingParty">The relaying party.</param>
        /// <returns>Name of the encrypting certificate</returns>
        private static string GetEncryptingCertificateName(string relayingParty)
        {
            try
            {
                RelayingParty rp = StsConfigurationSection.Instance.RelayingParties.GetItemByKey(relayingParty);
                return rp.certificate;
            }
            catch (Exception)
            {
                throw new UnauthorizedRequestException(string.Format("Relaying party: {0} is uknown.", relayingParty));
            }
        }

        /// <summary>
        /// Validates appliesTo and throws an exception if the appliesTo is null or contains an unexpected address.
        /// </summary>
        /// <param name="appliesTo">The AppliesTo value that came in the RST.</param>
        /// <exception cref="ArgumentNullException">If 'appliesTo' parameter is null.</exception>
        /// <exception cref="InvalidRequestException">If 'appliesTo' is not valid.</exception>
        private static void ValidateAppliesTo(EndpointAddress appliesTo)
        {
            if (appliesTo == null)
            {
                throw new ArgumentNullException("appliesTo");
            }

            if (EnableAppliesToValidation)
            {
                bool validAppliesTo = StsConfigurationSection.Instance.RelayingParties.Cast<RelayingParty>().Any(rp => appliesTo.Uri.Equals(new Uri(rp.name)));

                if (!validAppliesTo)
                {
                    throw new InvalidRequestException(string.Format("The 'appliesTo' address '{0}' is not valid.", appliesTo.Uri.OriginalString));
                }
            }
        }
    }
}