// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignOut.aspx.cs" company="Atomia AB">
//   Copyright (C) 2010 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   Defines the SignOut type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Web.UI;

namespace Atomia.Custom.IntermediateSts
{
    /// <summary>
    /// Defines the SignOut type.
    /// </summary>
    public partial class SignOut : Page
    {
        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = ConfigurationManager.AppSettings["SignOutAction"];
            Response.Redirect(action);
        }
    }
}