<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%
    var showExamples = (bool)ViewData["ShowExamples"];
%>

<% if (showExamples) { %>
    <div class="Bar">
	    <h4 class="Bar-title Bar-item"><%= Html.Resource("ExamplesTitle") %></h4>
    </div>

    <div class="Iconbox">
        <span class="Iconbox-icon Icon Icon--folder"></span>
        <p class="Iconbox-text">
            <a class="Iconbox-link" href="<%= Url.Action("Index", new { controller = "Example" }) %>"><%= Html.Resource("Examples") %></a>
            <%= Html.Resource("ExamplesText")%>
        </p>
    </div>
<% } %>


<%--
    To render this partial as its own section on the dashboard, include something like this in Dashboard\Index.aspx
    
    <div class="u-margin-top u-flexbox-container">
		<% Html.RenderHcpPartial("Loader", "Example"); %>
	</div>

    You can of course also include it in one of the existing sections of the dashboard.
--%>
