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
