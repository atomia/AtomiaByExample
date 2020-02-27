<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Web.Mvc" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= Html.Resource("Title") %>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <%
        var firstName = ViewData["FirstName"];
        var Number = ViewData["Number"];
    %>
    <h3>Hello <%= firstName %></h3>
    <div>
        <button id='incrementBtn' class="Btn Btn--primary">Increment</button>
        <button id='decrementBtn' class="Btn Btn--secundary">Decrement</button>
        <div class="Message Message--info Message--block">
            <h2>Current vaulue is: <span id="valuePlaceholder"><%= Number %></span></h2>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
        <% Html.RenderPartial("_Resources"); %>
        <% Html.RenderPartial("_NewScripts"); %>
</asp:content>
