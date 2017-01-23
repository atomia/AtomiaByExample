<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
   <%= Html.Resource("Title") %>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <%
        var exampleAdd = Url.Action("Add", new { controller = "Example" });

    %>
    <!-- Introductory content. What is this page? -->
    <h1><%= Html.Resource("Examples") %></h1>
    <p><%= Html.Resource("Info") %></p>


    <!-- Data table actions bar -->
    <div class="dt-Actions dt-Actions--filter js-bulk-actions" data-bind="moveFilter">
        <a class="Btn Btn--primary Icon Icon--plus u-right" href="<%= exampleAdd %>" data-bind="visible"><%= Html.Resource("Add") %></a>
    </div>


    <!-- The data table -->
    <table class="Table js-datatable" data-bind="datatable: examples, delegatedClick: examples.initDelete">
        <thead>
            <tr>
                <th></th>
                <th data-priority="2"><%= Html.Resource("Status") %></th>
                <th data-priority="1"><%= Html.Resource("Name") %></th>
                <th data-priority="3" data-class-name="u-nowrap u-right-text"><%= Html.Resource("Actions") %></th>
            </tr>
        </thead>
        <tbody></tbody>
    </table>


    <!-- Data table cell templates -->
    <script type="text/html" id="js-status-cell">
        <!-- ko if: $data === 'OK' -->
            <span class="Icon Icon--checked">
                <span class="Icon-label"><%= Html.Resource("OK") %></span>
            </span>
        <!-- /ko -->
        <!-- ko if: $data === 'PROCESSING' -->
            <span class="Icon Icon--loading">
                <span class="Icon-label"><%= Html.Resource("Processing") %></span>
            </span>
        <!-- /ko -->
        <!-- ko if: $data === 'UNKNOWN' -->
            <span class="Icon Icon--warning">
                <span class="Icon-label"><%= Html.Resource("Unknown") %></span>
            </span>
        <!-- /ko -->
    </script>

    <script type="text/html" id="js-actions-cell">
        <a class="Btn Btn--small" data-bind="visible: canEdit, attr: {href: editUrl}"><%= Html.ResourceNotEncoded("Edit") %></a>
        <button class="Btn Btn--small Icon Icon--x" type="button" data-click="examples.initDelete" title="<%= Html.ResourceNotEncoded("Delete") %>" data-bind="visible: canDelete"></button>
    </script>


    <!-- Dialogs for confirming actions or submitting forms -->
    <div class="Dialog Dialog--modal" id="js-example-delete" data-bind="dialog, isOpen: delExample.dialogIsOpen">
        <section class="Dialog-header js-dialog-header">
            <span><%= Html.Resource("Confirm") %></span>
            <button class="Dialog-close Icon Icon--x Icon--standalone Icon--border" data-bind="click: delExample.closeDialog"></button>
        </section>

        <section class="Dialog-content js-dialog-content">
            <p>
                <%= Html.Resource("DeleteExampleQuestion") %>
                <strong data-bind="text: delExample.name"></strong>?
            </p>

            <p class="js-actions">
                <button class="Btn Btn--caution" data-bind="click: delExample.doDelete"><%= Html.Resource("Delete") %></button>
                <button class="Btn Btn--neutral" data-bind="click: delExample.closeDialog"><%= Html.Resource("Cancel") %></button>
            </p>
        </section>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
    <% Html.RenderPartial("_IndexScripts"); %>
</asp:Content>