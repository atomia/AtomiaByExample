<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    var Atomia = Atomia || {};
    Atomia.VM = Atomia.VM || {};

    // Keep all custom JS within a closure to not risk namespace collisions.
    (function () {

        function EditExampleViewModel(name) {
            var self = this;

            self.name = ko.observable(name);

            self.name.subscribe(function (nval) {
                self.name(self.name().toLowerCase());
            });
        }

        // Instantiate and attach view model to Atomia.VM root view model so knockout bindings get applied.
        $.extend(Atomia.VM, {
            editExample: new EditExampleViewModel('<%= Model.Name %>')
        });
    })();
    
</script>
