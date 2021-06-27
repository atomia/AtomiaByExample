<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Atomia.Web.Plugin.ServiceReferences.AccountAPI" %>


<script type="text/javascript">

tickets = function ($, undefined) {
    var debug = true,
        ticketsFetched = false;

    var oTicketsTable;
    var oRepliesTable;

    var newTicketId = -1;

    function init() {
        setupTicketsTable();
        setupRepliesTable();
        initSearch();
    }

    function setupTicketsTable() {
        if (!$("#ticketsTableConfig").length) {
            return;
        }

        var config = $("#ticketsTableConfig").data("config");

        oTicketsTable = $('#ticketsTable').dataTable({
            "bProcessing": true,
            "bServerSide": true,
            "sDom": 'rt<"Grid Grid--no-gutter"<"Grid-column-1"l><"Grid-column-4"p><"Grid-column-1"i>>', // TODO: Add 'f' when pages are updated to use knockout datatables.
            "sPaginationType": "full_numbers",
            "sAjaxSource": config.sAjaxSource, 
            "aaSorting": [[0, 'desc']],
            "fnServerData": function (sSource, aoData, fnCallback) {

                var filter = {};
                filter.StatusFilter = $('#ticketStatus').val();
                filter.DepartmentFilter = $('#ticketDepartment').val();

                aoData.push({ "name": "iFilter", "value": JSON.stringify(filter) });

                $.getJSON(sSource, aoData, function (json) {
                    fnCallback(json)
                });
            },
            "oLanguage": {
                "sProcessing": config.sProcessing,
                "sZeroRecords": config.sZeroRecords,
                "sInfo": config.sInfo,
                "sInfoEmpty": config.sInfoEmpty,
                "sInfoFiltered": config.InfoFilter,
                "sSearch": config.sSearch
            },
            "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                if (aData[0] == newTicketId) {
                    $(nRow).attr("class", "attention " + $(nRow).attr("class"));
                }
                
                return nRow;
            },
            "aoColumns": [
                {
                    "sWidth": "17%",
                    "bSearchable": false,
                    "bSortable": true
                },
                {
                    "sWidth": "17%",
                    "bSearchable": false,
                    "bSortable": false
                },
                {
                    "sWidth": "17%",
                    "bSearchable": false,
                    "bSortable": false
                },
                {
                    "sWidth": "15%",
                    "bSearchable": false,
                    "bSortable": false
                },
                {
                    "sWidth": "17%",
                    "bSearchable": false,
                    "bSortable": false
                },
                {
                    "sWidth": "5%",
                    "bSearchable": false,
                    "bSortable": false,
					"mRender": function (oObj, type, rowData) {
                        return '<a class="Btn Btn--small Btn--secondary js_edit_note" href="' + oObj + '" id="" name="view">' + config.sView + '</a>';
                    }
                }
            ]
        });
    }

    function setupRepliesTable() {
        if (!$("#replyTableConfig").length) {
            return;
        }

        var config = $("#replyTableConfig").data("config");

        oRepliesTable = $('#replyTable').dataTable({
            "bProcessing": true,
            "bServerSide": true,
            "sDom": 'rt<"Grid Grid--no-gutter"<"Grid-column-1"l><"Grid-column-4"p><"Grid-column-1"i>>',
            "sPaginationType": "full_numbers",
            "sAjaxSource": config.sAjaxSource, 
            "aaSorting": [[0, 'desc']],
            "fnServerData": function (sSource, aoData, fnCallback) { 
                aoData.push({ "name": "iTicketId", "value": config.iTicketId });

                $.getJSON(sSource, aoData, function (json) {
                    fnCallback(json)
                });
            },
            "oLanguage": {
                "sProcessing": config.sProcessing,
                "sZeroRecords": config.sZeroRecords,
                "sInfo": config.sInfo,
                "sInfoEmpty": config.sInfoEmpty,
                "sInfoFiltered": config.InfoFilter,
                "sSearch": config.sSearch,
            },
            "aoColumns": [
                {
                    "sWidth": "15%",
                    "bSearchable": false,
                    "bSortable": true
                },
                {
                    "sWidth": "18%",
                    "bSearchable": false,
                    "bSortable": false
                },
                {
                    "sWidth": "40%",
                    "bSearchable": false,
                    "bSortable": false
                },
                {
                    "sWidth": "20%",
                    "bSearchable": false,
                    "bSortable": false
                }
            ]
        });
    }

    function log(message) {
        if (!debug) {
            return;
        }

        console.log(message);
    }

    function redrawTickets(newTId) {
        newTicketId = newTId; 

        $('[id*="Table_filter"] input[type="text"]').val('');
        oTicketsTable.fnFilter('');
        oTicketsTable.fnSort([[1, 'desc']]);
    }

    function redrawReplies() {
		oRepliesTable.fnFilter('');
        oRepliesTable.fnSort([[1, 'desc']]);
    }

    function initSearch() {
        $("#searchbox").keyup(function () {
            oTicketsTable.fnFilter(this.value);
        });
		$("#searchbox1").keyup(function () {
			oRepliesTable.fnFilter(this.value);
        });
    }

    return {
        init: init,
        redrawTickets: redrawTickets,
        redrawReplies: redrawReplies
    }
}(jQuery);

jQuery(document).ready(function ($) {
    tickets.init();
});


</script>
