using Atomia.Provisioning.Modules.Common;
using Atomia.Provisioning.Modules.Example.Commands;
using System;
using System.Collections.Generic;
namespace Atomia.Provisioning.Modules.Example
{
    public class Example : CommandPatternModuleBase
    {
        public override string GetServiceDescriptionAssemblyPath()
        {
            return "Atomia.Provisioning.Modules.Example.ServiceDescription.xml";
        }

        public override Dictionary<string, Type> GetServiceNameToCommandClassTypeMapping()
        {
            return new Dictionary<string, Type>
            {
                { "Example", typeof(ExampleCommand) }
            };
        }
    }
}
