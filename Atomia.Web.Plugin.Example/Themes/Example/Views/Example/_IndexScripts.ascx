<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    var Atomia = Atomia || {};
    Atomia.VM = Atomia.VM || {};

    // Keep all custom JS within a closure to not risk namespace collisions.
    (function () {

        // Define the URLS that are used by the view models below.
        var URLS = {
            exampleSearch: '<%= Url.Action("Search", new { controller="Example" }) %>',
            exampleEdit: '<%= Url.Action("Edit", new { controller = "Example", serviceID = "_serviceID_"  }) %>',
            exampleDelete: '<%= Url.Action("Delete", new { controller = "Example", serviceID = "_serviceID_" }) %>'
        };


        // View model for the data table
        function ExampleListViewModel() {
            var self = this;
            $.extend(self, new Atomia.Shared.DatatableMixin());

            // see Search action in ExampleController for data
            self.columns = [
                {},
                { // Status
                    orderData: [0],
                    searchable: false,
                    render: self.renderRow(0)
                },
                { // Name
                    orderData: [1],
                    render: self.renderRow(1)
                },
                { // Actions
                    searchable: false,
                    sortable: false,
                    render: function (data, type, row) {
                        var actionsData = JSON.parse(row[2]);
                        var editUrl = URLS.exampleEdit.replace('_serviceID_', actionsData.logicalId);

                        return renderKoTemplate('js-actions-cell', {
                            canDelete: actionsData.canDelete,
                            canEdit: actionsData.canEdit,
                            editUrl: editUrl
                        });
                    }
                }
            ];

            self.initDelete = function (e) {
                var $row = $(e.currentTarget).parents('tr');
                var rowData = self.getRowData($row);
                var actionsData = JSON.parse(rowData[2]);

                ko.postbox.publish('uiInitDeleteExample', {
                    name: rowData[1],
                    logicalId: actionsData.logicalId    
                });
            };

            self.loadTableData = function (data, callback, settings) {
                $.post(URLS.exampleSearch, data, function (json) {
                    callback(json);
                }, 'json');
            };

            ko.postbox.subscribe('dataDeletedExample', function () {
                var datatable = self.datatable();

                if (datatable) {
                    datatable.draw();
                }
            });

            $.extend(self.datatableOptions, {
                ajax: self.loadTableData,
                columns: self.columns
            });
        }


        // View model for confirming delete of example service.
        function DeleteExampleDialog() {
            $.extend(self, new Atomia.Shared.DialogMixin());

            self.logicalId = ko.observable();
            self.name = ko.observable();

            ko.postbox.subscribe('uiInitCertAction', function (data) {
                requireProperties(data, 'logicalId', 'name');

                self.logicalId(data.logicalId);
                self.name(data.name);
            });

            self.doDelete = function () {
                var action = URLS.exampleDelete.replace('_serviceID_', self.logicalId());

                $.post(action, function (data) {
                    if (data && data.success === 'TRUE') {
                        ko.postbox.publish('dataDeletedExample');
                    } else if (data && data.error) {
                        notify.error(data.error);
                    }

                    self.dialogIsOpen(false);
                }, 'json');
            };
        }


        // Instantiate and attach view models to Atomia.VM root view model so knockout bindings get applied.
        $.extend(Atomia.VM, {
            examples: new ExampleListViewModel(),
            delExample: new DeleteExampleDialog()
        });
    })();
    
</script>
