<%@ Import Namespace="Atomia.Web.Plugin.Example.Models" %>
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Atomia.Web.Plugin.Example.Models.TicketModel>" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Web.Routing" %>

<asp:content id="indexTitle" contentplaceholderid="TitleContent" runat="server">
   <%=Html.Resource("Title")%>
</asp:content>

<asp:content id="indexContent" contentplaceholderid="MainContent" runat="server">
    <% TicketModel ticket = (TicketModel)ViewData["ticket"]; %>
	
	<div class="settingsbox">
        <h3><%= Html.Resource("HeaderText") %> - <%= ticket.Ticketid %></h3>
		
		<div class="notification" rel="kyk-error" style="display:none; z-index: 1100;">
            <p><%= Html.Resource("AddReplyErrorText") %></p>
        </div>

        <div class="notification" rel="kyk-success" style="display:none; z-index: 1100;">
            <p><%= Html.Resource("AddReplySuccessText") %></p>
        </div>
		
		<div class="settingsboxinner">

            <div class="Formrow">
                <label class="Formrow-label"><%= Html.Resource("TicketSubject") %></label>
                <div class="Formrow-offset">
                    <span><%= ticket.Subject %></span>
                </div>
            </div>
			
			<div class="Formrow">
                <label class="Formrow-label"><%= Html.Resource("TicketMessage") %></label>
                <div class="Formrow-offset">
                    <span><%= ticket.Message %></span>
                </div>
            </div>
			
			<div class="Formrow">
                <label class="Formrow-label"><%= Html.Resource("TicketDate") %></label>
                <div class="Formrow-offset">
                    <span><%= ticket.Date %></span>
                </div>
            </div>
			
			<div class="Formrow">
                <label class="Formrow-label"><%= Html.Resource("TicketStatus") %></label>
                <div class="Formrow-offset">
                    <span><%= ticket.Status %></span>
                </div>
            </div>
			
			<h3 id="replyTableConfig" 
                data-config=
                '{ "iTicketId": "<%=ticket.Ticketid%>", "sAjaxSource": "<%=Url.Action("SearchReplies", new {area="Tickets", controller = "Tickets"})%>", "sProcessing": "<%= Html.Resource("Datatables, Processing") %>", "sZeroRecords": "<%= Html.Resource("Datatables, ZeroRecords") %>", "sInfo": "<%= Html.Resource("Datatables, Info") %>", "sInfoEmpty": "<%= Html.Resource("Datatables, InfoEmpty") %>", "sInfoFiltered": "<%= Html.Resource("Datatables, InfoFilter") %>", "sSearch": "<%= Html.Resource("Datatables, Search") %>" }'
                    >Replies</h3>
					
			<div id="kykAddFormContainer" class="form_dialog" style="display:none">
                    <h4><%= Html.Resource("AddNewReply") %></h4>
					
					<div class="form_dialog_content">
                        <%= Html.Resource("DescriptionNewReply") %>
						
						<% Html.BeginForm("AddReply", "Tickets", new { area="Tickets", ticketId = ticket.Ticketid }, FormMethod.Post, new { enctype = "multipart/form-data", @id = "new_reply_form", @class = "autocomplete_off", @rel = "kyk", @refresh = "replies", @container = "#kykAddFormContainer", @addbtn = "#kykAddBtn", @novalidate = "novalidate" }); %>
                           <%= Html.AntiForgeryToken() %>
						   
						<div class="Formrow Formrow--required">
							<label class="Formrow-label" for="Message"><%= Html.Resource("Message") %></label>

							<div class="Formrow-offset">
								<div class="Formrow-inputGroup">
									<textarea id="Message" style="height:120px;"></textarea>
								</div>
							</div>
						</div>
						
						<div class="Formrow">
                            <label class="Formrow-label"><%= Html.Resource("Attachments") %></label>
                            <div class="Formrow-offset">
                                <input type="file" value="" multiple="multiple" />
                                    <br /><span class="quiet"></span>
                            </div>
                        </div>	   
						
						<p class="actions">
                            <button type="submit" class="Btn Btn--large Btn--primary"><%= Html.Resource("Save") %></button>
                            <a rel="kyk" data-target="#kykAddBtn" data-base="#kykAddFormContainer" data-clearform="base" class="Btn Btn--neutral" href="javascript:void(0)"><%= Html.Resource("Cancel") %></a>
                        </p>
							
						<% Html.EndForm(); %>

					</div>
			</div>
					
		     <div class="dt-Actions dt-Actions--filter">
               <input class="dt-Search" placeholder="<%= Html.Resource("Datatables, Search") %>" aria-controls="filteringResellerDomainTable" type="search" id="searchbox1" />
                <a id="kykAddBtn" href="javascript:void(0);" rel="kyk" data-target="#kykAddFormContainer" data-base="#kykAddBtn" class="Btn Btn--primary Icon Icon--plus u-right"><%= Html.Resource("AddNewReply") %></a>
            </div>
			
			<table class="list display" id="replyTable">
                <thead>
                    <tr>
                        <th>
                            <%= Html.Resource("ReplyFrom")%>
                        </th>
                         <th>
                            <%= Html.Resource("ReplyDate")%>
                        </th>
                        <th>
                            <%= Html.Resource("ReplyMessage")%>
                        </th>
                        <th>
                            <%= Html.Resource("ReplyAttachments")%>
                        </th>
                    </tr>
                </thead>
                <tbody>
                      <tr><td></td><td></td><td></td><td></td></tr>
                </tbody>
            </table>
                <br />
			
		</div>
		
	</div>

</asp:content>

<asp:Content ID="jsFiles" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">
   
     <% Html.RenderPartial("_formScripts"); %>
	 <% Html.RenderPartial("_ticketScripts"); %>
   
   
</asp:Content>