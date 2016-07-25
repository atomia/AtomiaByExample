// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CertificateUtil.cs" company="Atomia AB">
//   Copyright (C) 2010 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   Defines the CertificateUtil type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Security.Cryptography.X509Certificates;

namespace Atomia.Custom.IntermediateSts
{
    /// <summary>
    /// Defines the CertificateUtil type.
    /// </summary>
    public class CertificateUtil
    {
        /// <summary>
        /// Gets the certificate.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="location">The location.</param>
        /// <param name="subjectName">Name of the subject.</param>
        /// <returns>Certificate by given name.</returns>
        public static X509Certificate2 GetCertificate(StoreName name, StoreLocation location, string subjectName)
        {
            X509Store store = new X509Store(name, location);
            X509Certificate2Collection certificates = null;
            store.Open(OpenFlags.ReadOnly);

            try
            {
                X509Certificate2 result = null;

                // Every time we call store.Certificates property, a new collection will be returned.
                certificates = store.Certificates;

                foreach (X509Certificate2 cert in certificates)
                {
                    if (cert.SubjectName.Name.ToLower() == subjectName.ToLower())
                    {
                        if (result != null)
                        {
                            throw new ApplicationException(string.Format("There are multiple certificate for subject Name {0}", subjectName));
                        }

                        result = new X509Certificate2(cert);
                    }
                }

                if (result == null)
                {
                    throw new ApplicationException(string.Format("No certificate was found for subject Name {0}", subjectName));
                }

                return result;
            }
            finally
            {
                if (certificates != null)
                {
                    foreach (X509Certificate2 cert in certificates)
                    {
                        cert.Reset();
                    }
                }

                store.Close();
            }
        }
    }
}