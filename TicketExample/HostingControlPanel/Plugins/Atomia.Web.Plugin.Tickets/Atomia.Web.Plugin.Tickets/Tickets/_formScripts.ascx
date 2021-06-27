<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Atomia.Web.Plugin.ServiceReferences.AccountAPI" %>

<script type="text/javascript">

forms = function ($, undefined) {
    var notificationErrorLevel = 'error';
    var notificationSuccessLevel = 'success'

    function init() {
        submitForm();
        formButtons();
    }

    function submitForm() {
        $(document).on('submit', 'form[rel="kyk"]', function (e) {
            e.preventDefault();
			
            var form = $(this);
			
			var action = $(this).attr("action");
			var toSend=null;
			
			if (form.attr("refresh") == "tickets") {
				Subject = form["0"].elements[1].value;  
				Message = form["0"].elements[2].value;  
				Emergency = form["0"].elements[3].checked;
				toSend = {subject: Subject, message: Message, emergency: Emergency};
			}
			else{
				Reply = form["0"].elements[1].value;
				toSend = {reply: Reply};
			}

		        $.ajax({
                url: $(this).attr("action"),
                type: $(this).attr("method"),
                data: toSend,
                success: function (data) {
                    clearErrors(form);
					
					if (typeof data.Errors !== "undefined") {
					
                        showNotif(notificationErrorLevel);
                    }

                    if (typeof data.Success !== "undefined" && data.Success == true) {
                        showNotif(notificationSuccessLevel);

                        clearInputs(form);
                        $(form.attr("container")).hide();
                        $(form.attr("addbtn")).show();

                        if (form.attr("refresh") == "tickets") {
                            tickets.redrawTickets(data.TicketId);
                        }
                        if (form.attr("refresh") == "replies") {
                            tickets.redrawReplies();
                        }
                    }
                },
                error: function () {
                    showNotif(notificationErrorLevel);
                },
                complete: function() {
                }
            });
			});
    }

    function showNotif(sufix)
    {
        var notificationTitle = '';
        var notificationMessage;

        if (sufix == notificationErrorLevel) {
            notificationMessage = $('div.notification[rel="kyk-error"] p').text();
        } else if (sufix == notificationSuccessLevel) {
            notificationMessage = $('div.notification[rel="kyk-success"] p').text();
        }

        notify(notificationTitle, notificationMessage, sufix);
    }
    
    function notify(notificationTitle, notificationMessage, notificationLevel) {
        ko.postbox.publish('uiSetNotification', {
            title: notificationTitle,
            message: notificationMessage,
            level: notificationLevel
        });
    }

    function formButtons() {
        $(document).on('click', 'a[rel="kyk"][data-target][data-base]', function (e) {
            $($(this).data("target")).show();
            $($(this).data("base")).hide();
            $("#EmergencyExplanation").hide();

            var clearform = $(this).data("clearform");
            if (typeof clearform !== undefined && clearform == "base") {
                clearInputs($($(this).data("base")));
                clearErrors($($(this).data("base")));
            }
            if (typeof clearform !== undefined && clearform == "target") {
                clearInputs($($(this).data("target")));
                clearErrors($($(this).data("target")));
            }
        });

        $(document).on('click', 'div.notification[rel^="kyk-"] > a.close-btn', function (e) {
            $(this).parent().hide('drop', { direction: "up" }, 500);
        });
    }

    function clearInputs(form) {
        form.find('input[type=text], input[type=file], textarea').val('');
        form.find('input[type=checkbox]').prop('checked', false);
    }

    function clearErrors(form)
    {
        form.find('p.errorinfo').remove();
        form.find('.errorinfo').removeClass('errorinfo');
    }
    return {
        init: init
    }
}(jQuery);

jQuery(document).ready(function ($) {
    forms.init();

});

</script>
