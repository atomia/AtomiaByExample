// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecurityTokenHandler.cs" company="Atomia AB">
//   Copyright (C) 2010 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   Defines the SecurityTokenHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.IdentityModel.Tokens;
using System.Web.Security;
using Microsoft.IdentityModel.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Atomia.Custom.SecurityTokenHandlers
{
    /// <summary>
    /// Defines the SecurityTokenHandler type.
    /// </summary>
    public class SecurityTokenHandler : UserNameSecurityTokenHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityTokenHandler"/> class.
        /// </summary>
        public SecurityTokenHandler()
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance can validate token.
        /// </summary>
        /// <value><c>true</c> if this instance can validate token; otherwise, <c>false</c>.</value>
        public override bool CanValidateToken
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Collection of identity claims</returns>
        public override ClaimsIdentityCollection ValidateToken(System.IdentityModel.Tokens.SecurityToken token)
        {
            UserNameSecurityToken userNameSecurityToken = token as UserNameSecurityToken;

            MembershipProvider provider = Membership.Provider;
            bool isValidated = provider.ValidateUser(userNameSecurityToken.UserName, userNameSecurityToken.Password);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new Claim[] { new Claim(System.IdentityModel.Claims.ClaimTypes.Name, userNameSecurityToken.UserName) });

            ClaimsIdentityCollection claimIdentityCollection = new ClaimsIdentityCollection(new IClaimsIdentity[] { claimsIdentity });

            return claimIdentityCollection;
        }
    }
}
