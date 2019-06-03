<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Atomia.Web.Plugin.ServiceReferences.AccountAPI" %>

<script type="text/javascript">
    $(document).ready(function () {
        var $incrementBtn = $('#incrementBtn');
        var $decrementBtn = $('#decrementBtn');
        var $valuePlaceholder = $('#valuePlaceholder');

        $incrementBtn.click(function () {
            $.post(Atomia.URLS.performIncrement, null,function(res) {
                $valuePlaceholder.text(res);
            });
        });

         $decrementBtn.click(function () {
            $.post(Atomia.URLS.performDecrement, null,function(res) {
                $valuePlaceholder.text(res);
            });
        });
    });
</script>
