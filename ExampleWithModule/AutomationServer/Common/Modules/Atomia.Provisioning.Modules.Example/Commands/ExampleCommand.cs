using System;
using Atomia.Provisioning.Base;
using Atomia.Provisioning.Base.Module;
using Atomia.Provisioning.Modules.Common;
using System.Threading.Tasks;

namespace Atomia.Provisioning.Modules.Example.Commands
{
    public class ExampleCommand : ExampleCommandBase
    {
        public ExampleCommand(
            ModuleService service,
            ResourceDescription resource,
            ModuleService newServiceSettings,
            ModuleCommandType commandType,
            int listDepth) : base(service, resource, newServiceSettings, commandType, listDepth)
        {
        }

        protected override void ExecuteAdd(ModuleService moduleService)
        {
        }

        protected override void ExecuteRemove(ModuleService moduleService)
        {
        }

        protected override void ValidateService(ModuleService moduleService)
        {
        }

        public override string CallOperation(string operationName, string operationArgument)
        {
            switch (operationName)
            {
                case "GetFirstName":
                    return FirstName;
                case "GetFullName":
                    return $"{FirstName} {LastName}";
                case "Increment":
                    return (++Value).ToString();
                case "Decrement":
                    return (--Value).ToString();
                default:
                    throw new Exception($"Invalid operation name {operationName}");
            }
        }
    }
}
