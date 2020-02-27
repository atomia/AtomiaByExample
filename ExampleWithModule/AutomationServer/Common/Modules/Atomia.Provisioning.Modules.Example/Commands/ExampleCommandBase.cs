using System.Linq;
using Atomia.Provisioning.Base;
using Atomia.Provisioning.Base.Module;
using Atomia.Provisioning.Modules.Common;

namespace Atomia.Provisioning.Modules.Example.Commands
{
    public abstract class ExampleCommandBase : ModuleCommandSimpleBase
    {
        public string AccountId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public ExampleCommandBase(
            ModuleService service,
            ResourceDescription resource,
            ModuleService newServiceSettings,
            ModuleCommandType commandType,
            int listDepth) : base(service, resource, newServiceSettings, commandType, listDepth)
        {

            if (service.Properties.SingleOrDefault(p => p.Name == "AccountId") != null)
            {
                AccountId = service["AccountId"];
                FirstName = service["FirstName"];
                LastName = service["LastName"];
            }
        }
    }
}
