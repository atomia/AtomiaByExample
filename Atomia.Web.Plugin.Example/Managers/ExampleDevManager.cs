using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Atomia.Web.Base.Validation;
using Atomia.Web.Plugin.Example.Models;
using Atomia.Web.Plugin.HCP.Provisioning.Helpers.ActionTrail;

namespace Atomia.Web.Plugin.Example.Managers
{
    public class ExampleDevManager
    {
        private Controller controller;
        private static Dictionary<string, ExampleModel> models = null;

        public ExampleDevManager(Controller controller)
        {
            if (models == null)
            {
                var foo = new ExampleModel
                {
                    LogicalID = Guid.NewGuid().ToString(),
                    Name = "foo",
                    Status = "OK"
                };
                var bar = new ExampleModel
                {
                    LogicalID = Guid.NewGuid().ToString(),
                    Name = "bar",
                    Status = "PROCESSING"
                };
                var fizz = new ExampleModel
                {
                    LogicalID = Guid.NewGuid().ToString(),
                    Name = "fizz",
                    Status = "OK"
                };
                var buzz = new ExampleModel
                {
                    LogicalID = Guid.NewGuid().ToString(),
                    Name = "buzz",
                    Status = "UNKNOWN"
                };

                models = new Dictionary<string, ExampleModel>()
                {
                    { foo.LogicalID, foo },
                    { bar.LogicalID, bar },
                    { fizz.LogicalID, fizz },
                    { buzz.LogicalID, buzz },
                };
            }
            
            this.controller = controller;
        }
        
        public void DeleteExample(ExampleModel example)
        {
            try
            {
                models.Remove(example.LogicalID);
            }
            catch (Exception ex)
            {
                throw new AtomiaServerSideValidationException("DeleteExample", ex.Message);
            }
        }

        public void AddExample(ExampleModel example)
        {
            Validate(example);
            
            try
            {
                example.LogicalID = Guid.NewGuid().ToString();
                example.Status = "PROCESSING";
                models.Add(example.LogicalID, example);
            }
            catch (AtomiaServerSideValidationException e)
            {
                var error = e.Errors.First();
                throw new AtomiaServerSideValidationException(error.PropertyName, error.ErrorMessage);
            }
            catch (Exception e)
            {
                var errorMessage = String.Format(controller.LocalResource("Add", "CantAddExample"), example.Name, e.Message);
                throw new AtomiaServerSideValidationException("Name", errorMessage);
            }
        }

        public void EditExample(ExampleModel example)
        {
            Validate(example);

            try
            {
                models[example.LogicalID] = example;
            }
            catch (AtomiaServerSideValidationException e)
            {
                var error = e.Errors.First();
                throw new AtomiaServerSideValidationException(error.PropertyName, error.ErrorMessage);
            }
            catch (Exception e)
            {
                var errorMessage = String.Format(controller.LocalResource("Edit", "CantEditExample"), example.Name, e.Message);
                throw new AtomiaServerSideValidationException("Name", errorMessage);
            }
        }

        public List<ExampleModel> FetchObjectsWithPaging(string search, string iDisplayStart, string iDisplayLength, int sortColumn, string sortDirection, out long total)
        {
            var examples = new List<ExampleModel>();
            total = 0;
            
            try
            {
                foreach (var example in models.Values)
                {
                    examples.Add(example);
                }

                total = models.Count;
            }
            catch (Exception ex)
            {
                HostingControlPanelLogger.LogHostingControlPanelException(ex);
                total = 0;
            }

            return examples;
        }

        public ExampleModel FetchExample(string serviceId)
        {
            try
            {
                return models[serviceId];
            }
            catch (Exception ex)
            {
                HostingControlPanelLogger.LogHostingControlPanelException(ex);
            }

            return new ExampleModel();
        }

        public bool CanAddExampleServices()
        {
            return true;
        }

        private void Validate(ExampleModel example)
        {
            var errors = DataAnnotationsValidationRunner.GetErrors(example);

            if (errors.Any())
            {
                throw new AtomiaServerSideValidationException(errors);
            }
        }
    }
}
