// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IService.cs" company="Atomia AB">
//   Copyright (C) 2010 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   Defines the IService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;
using System.ServiceModel;

namespace LegacySTS
{
    /// <summary>
    /// Defines the IService type.
    /// </summary>
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string GetData(int value);
    }
}
