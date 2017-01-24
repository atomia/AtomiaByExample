<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Atomia.Web.Plugin.Example.Models.ExampleModel>" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= Html.Resource("Title") %>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1><%= Html.Resource("AddExample") %></h1>
    <p><%= Html.Resource("Intro") %></p>

    <% Html.BeginForm("Add", "Example", null, FormMethod.Post, new Dictionary<string, object> { {"data-bind", "submitValid"} }); %>
    <%= Html.AntiForgeryToken() %>

        <div class="Formrow Formrow--required">
            <label class="Formrow-label"><%= Html.Resource("Name") %></label>

            <div class="Formrow-offset">
                <%= Html.TextBox("Name", "", new Dictionary<string, object> {
                        { "data-bind", "value: addExample.name, validateRequired" },
                        { "class", "Formrow-input" }
                    }) %>

                <p class="Formrow-message"><%= Html.ValidationMessage("Name") %></p>
            </div>
        </div>

        <p class="js-actions">
            <button type="submit" class="Btn Btn--primary"><%= Html.Resource("Add") %></button>
            <a class="Btn Btn--neutral" href="<%= Url.Action("Index", new { controller = "Example" }) %>"><%= Html.Resource("Cancel") %></a>
        </p>

    <% Html.EndForm(); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
    
    <% Html.RenderPartial("_AddScripts", Model); %>
    
</asp:Content>
