<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Atomia.Web.Plugin.HCP.Authorization.Helpers" %>

<% 
    
%>

<script type="text/html" id="js-dashboard-example-content">
    <div class="u-margin-top u-flexbox-container">
        <% Html.RenderHcpPartial("Loader", "Example"); %>
    </div>
</script>

<script>
    (function () {
        var content = $('#js-dashboard-example-content').html();
        $("#js-dashboard-domains-websites").after(content);
	})();
</script>
