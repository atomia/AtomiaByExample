// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestIntermediateClaimProvider.cs" company="Atomia AB">
//   Copyright (C) 2010 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   Defines the TestIntermediateClaimProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System;

using Microsoft.IdentityModel.Claims;

namespace Atomia.Custom.TestIntermediateClaimProvider
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
    /// Defines the TestIntermediateClaimProvider type.
    /// </summary>
    public class TestIntermediateClaimProvider : IClaimProvider
    {
        /// <summary>
        /// The list of claim types this provider can handle
        /// </summary>
        private readonly List<string> canHandleTypes;

        /// <summary>
        /// Determinate if object was disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestIntermediateClaimProvider"/> class.
        /// </summary>
        public TestIntermediateClaimProvider()
        {
            this.canHandleTypes = new List<string>
                                  {
                                      System.IdentityModel.Claims.ClaimTypes.Name
                                  };
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TestIntermediateClaimProvider"/> class.
        /// </summary>
        ~TestIntermediateClaimProvider()
        {
            this.CleanUp(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Determines whether this instance can handle the specified claim types.
        /// </summary>
        /// <param name="claimTypes">The claim types.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>List of claims</returns>
        public string[] CanHandle(string[] claimTypes, IClaimsIdentity identity)
        {
            return claimTypes.Where(claimType => this.canHandleTypes.Exists((s) => s == claimType)).ToArray();            
        }

        /// <summary>
        /// Updates the identity.
        /// </summary>
        /// <param name="claimTypes">The claim types.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>Updated identity</returns>
        public IClaimsIdentity UpdateIdentity(string[] claimTypes, IClaimsIdentity identity)
        {
            Account account = GetAccount(identity.Name);
            if (account != null)
            {
                identity = new ClaimsIdentity();
                identity.Claims.Add(new Claim(System.IdentityModel.Claims.ClaimTypes.Name, account.Username));
            }

            return identity;
        }

        /// <summary>
        /// Gets the account.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>Account of customer with provided username</returns>
        private static Account GetAccount(string username)
        {
            Account account = new Account();
            switch (username)
            {
                case "LegacyAdmin":
                    account.Username = "someuser@example.com";
                    break;
                case "LegacyAdmin2":
                    account.Username = "someuser2@example.com";
                    break;
            }

            return account;
        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> [disposing].</param>
        private void CleanUp(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                }
            }

            this.disposed = true;
        }
    }
}
