using Atomia.Web.Plugin.Validation.ValidationAttributes;

namespace Atomia.Web.Plugin.Example.Models
{
    public class ExampleModel
    {
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        public string Name { get; set; }

        public string LogicalID { get; set; }

        public string Status { get; set; }

    }
}
