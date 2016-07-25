// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PassiveStsEndpoint.aspx.cs" company="Atomia AB">
//   Copyright (C) 2010 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   Defines the PassiveStsEndpoint type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Web.UI;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.IdentityModel.Web;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Atomia.Custom.IntermediateSts
{
    /// <summary>
    /// Defines the PassiveStsEndpoint type.
    /// </summary>
    public partial class PassiveStsEndpoint : Page
    {
        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            IntermediateSecurityTokenServiceConfiguration intermediateSecurityTokenServiceConfiguration = new IntermediateSecurityTokenServiceConfiguration();
            SecurityTokenService securityTokenService = new IntermediateSecurityTokenService(intermediateSecurityTokenServiceConfiguration);
            FederatedPassiveSecurityTokenServiceOperations.ProcessRequest(this.Request, this.User, securityTokenService, this.Response);
            return;
        }
    }
}