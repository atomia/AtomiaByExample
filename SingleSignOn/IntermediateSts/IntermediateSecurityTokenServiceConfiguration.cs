// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntermediateSecurityTokenServiceConfiguration.cs" company="Atomia AB">
//   Copyright (C) 2010 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   Defines the IntermediateSecurityTokenServiceConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.SecurityTokenService;

namespace Atomia.Custom.IntermediateSts
{
    /// <summary>
    /// Defines the IntermediateSecurityTokenServiceConfiguration type.
    /// </summary>
    public class IntermediateSecurityTokenServiceConfiguration : SecurityTokenServiceConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntermediateSecurityTokenServiceConfiguration"/> class.
        /// </summary>
        public IntermediateSecurityTokenServiceConfiguration() : base(
            WebConfigurationManager.AppSettings[Common.IssuerName],
            new X509SigningCredentials(
                CertificateUtil.GetCertificate(StoreName.My, StoreLocation.LocalMachine, WebConfigurationManager.AppSettings[Common.SigningCertificateName])))
        {
            this.SecurityTokenService = typeof(IntermediateSecurityTokenService);
        }
    }
}