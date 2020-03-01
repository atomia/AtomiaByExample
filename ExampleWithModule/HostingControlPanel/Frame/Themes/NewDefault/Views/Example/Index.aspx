<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Web.Mvc" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    <%= Html.Resource("Title") %>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <%
        var canAdd = ViewData["CanAdd"] == "1";
        var exists = ViewData["Exists"] == "1";
    %>
    <h1><%= Html.Resource("Title") %></h1>

    <% if (!canAdd) { %>

        <p><%= Html.Resource("NotAllowed") %></p>

    <% } else { %>

        <p><%= Html.Resource("IntroText") %></p>

        <% if (exists) { %>
            <%
                var firstName = ViewData["FirstName"];
                var lastName  = ViewData["LastName"];
                var number    = ViewData["Number"];
            %>
            <% Html.BeginForm("Delete", "Example", new { area = "Example" }, FormMethod.Post, new { @autocomplete = "off" }); %>
            <%= Html.AntiForgeryToken() %>
            <section class="Paper">
                <h4><%= Html.Resource("ServiceData") %></h4>

                <div class="Formrow">
                    <label class="Formrow-label"><%= Html.Resource("FirstName") %></label>

                    <div class="Formrow-offset">
                        <span class="Formrow-input Formrow-input--readonly"><%: firstName %></span>
                    </div>
                </div>

                <div class="Formrow">
                    <label class="Formrow-label"><%= Html.Resource("LastName") %></label>

                    <div class="Formrow-offset">
                        <span class="Formrow-input Formrow-input--readonly"><%: lastName %></span>
                    </div>
                </div>

                <div class="Formrow">
                    <label class="Formrow-label"><%= Html.Resource("Number") %></label>

                    <div class="Formrow-offset">
                        <p>
                            <span class="Formrow-input Formrow-input--readonly"><strong id="valuePlaceholder"><%: number %></strong></span>
                        </p>
                        <p>
                            <button id='incrementBtn' class="Btn Btn--large Btn--info" type="button"><%= Html.Resource("Increment") %></button>
                            <button id='decrementBtn' class="Btn Btn--large Btn--secondary" type="button"><%= Html.Resource("Decrement") %></button>
                        </p>
                    </div>
                </div>
            </section>
            <div class="Bar Bar--light js-actions">
                <button class="Btn Btn--large Btn--caution" type="submit"><%= Html.Resource("DeleteService") %></button>
            </div>
            <% Html.EndForm(); %>
        <% } else { %>
            <% Html.BeginForm("Create", "Example", new { area = "Example" }, FormMethod.Post, new { @autocomplete = "off" }); %>
            <%= Html.AntiForgeryToken() %>
            <div class="Bar Bar--light js-actions">
                <button class="Btn Btn--large Btn--primary" type="submit"><%= Html.Resource("CreateService") %></button>
            </div>
            <% Html.EndForm(); %>
        <% } %>

    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
    <% Html.RenderPartial("_Resources"); %>
    <% Html.RenderPartial("_NewScripts"); %>
</asp:content>
