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
            // Here is the place where you can call third party API to add (provision) some servis
            // Usually the result of provisioning is some ID that can be saved in the Atomia service property
            // in this case we will just randomly create some integer and save it in the "Number" property 
            moduleService["Number"] = (new Random()).Next(0, 10).ToString();
        }

        protected override void ExecuteRemove(ModuleService moduleService)
        {
            // Here is the place where you can call third party API to remove (deprovision) servis
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
                    int x = 0;
                    Int32.TryParse(service["Number"], out x);
                    service["Number"] = (++x).ToString();
                    return service["Number"];
                case "Decrement":
                    int y = 0;
                    Int32.TryParse(service["Number"], out y);
                    service["Number"] = (--y).ToString();
                    return service["Number"];
                default:
                    throw new Exception($"Invalid operation name {operationName}");
            }
        }
    }
}
