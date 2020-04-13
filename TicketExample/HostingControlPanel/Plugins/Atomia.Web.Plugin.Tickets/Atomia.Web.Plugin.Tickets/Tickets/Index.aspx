<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="Atomia.Web.Plugin.Example.Models" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Web.Routing" %>

<asp:content id="indexTitle" contentplaceholderid="TitleContent" runat="server">
   <%=Html.Resource("Title")%>
</asp:content>

<asp:content id="indexContent" contentplaceholderid="MainContent" runat="server">
    <div class="settingsbox">
        <h3><%= Html.Resource("HeaderText") %></h3>
		
		<div class="notification" rel="kyk-error" style="display:none; z-index: 1100;">
            <p><%= Html.Resource("AddTicketErrorText") %></p>
        </div>
		
		<div class="notification" rel="kyk-success" style="display:none; z-index: 1100;">
            <p><%= Html.Resource("AddTicketSuccessText") %></p>
        </div>
		
		<div class="settingsboxinner">
            <p id="ticketsTableConfig" 
                data-config=
                '{ "sAjaxSource": "<%= Url.Action("Search", new {area="Tickets", controller = "Tickets"})%>", "sProcessing": "<%= Html.Resource("Datatables, Processing") %>", "sZeroRecords": "<%= Html.Resource("Datatables, ZeroRecords") %>", "sInfo": "<%= Html.Resource("Datatables, Info") %>", "sInfoEmpty": "<%= Html.Resource("Datatables, InfoEmpty") %>", "sInfoFiltered": "<%= Html.Resource("Datatables, InfoFilter") %>", "sSearch": "<%= Html.Resource("Datatables, Search") %>", "sView": "View" }'
            ><%= Html.Resource("DescriptionTickets") %></p>
			
			<div id="kykAddFormContainer" class="form_dialog" style="display:none">
                <h4><%= Html.Resource("AddNewTicket") %></h4>
                <div class="form_dialog_content">
                    <%= Html.Resource("DescriptionNewTicket") %>

                    <% Html.BeginForm("AddTicket", "Tickets", new { area = "Tickets" }, FormMethod.Post, new { enctype = "multipart/form-data", @id = "new_ticket_form", @class = "autocomplete_off", @rel = "kyk", @refresh = "tickets", @container = "#kykAddFormContainer", @addbtn = "#kykAddBtn", @novalidate = "novalidate" }); %>
                        <%= Html.AntiForgeryToken() %>
					
					    <div class="Formrow Formrow--required">
							<label class="Formrow-label" for="Subject"><%= Html.Resource("Subject") %></label>

							<div class="Formrow-offset">
									<input id="Subject" type="text">							
							</div>
						</div>
						
						<div class="Formrow Formrow--required">
							<label class="Formrow-label" for="Message"><%= Html.Resource("Message") %></label>

							<div class="Formrow-offset">			
								<textarea id="Message" style="height:120px;"></textarea>
							</div>
						</div>
						
						<div class="Formrow">
							<label class="Formrow-label" for="Emergency"><%= Html.Resource("Emergency") %></label>

							<div class="Formrow-offset">
								<div class="">
									<p>
									   <label class="Tick">
											<input class="Tick-input Formrow-input" id="Emergency" type="checkbox">
											<span class="Tick-checkbox"></span>
											<i><%= Html.Resource("EmergencyExplanation") %></i>
										</label>
									</p>
								</div>
							</div>
					   </div>
					   
					   <p class="actions">
                        <button type="submit" class="Btn Btn--primary"><%= Html.Resource("Save") %></button>
                        <a rel="kyk" data-target="#kykAddBtn" data-base="#kykAddFormContainer" data-clearform="base" class="Btn Btn--neutral" href="javascript:void(0)"><%= Html.Resource("Cancel") %></a>
					  </p>
					    <% Html.EndForm(); %>
				</div>
			</div>
			
			<div class="dt-Actions dt-Actions--filter">
                <input class="dt-Search" placeholder="<%= Html.Resource("Datatables, Search") %>" aria-controls="filteringResellerDomainTable" type="search" id="searchbox" />

                <a id="kykAddBtn" href="javascript:void(0);"  rel="kyk"  data-target="#kykAddFormContainer" data-base="#kykAddBtn"  class="Btn Btn--primary Icon Icon--plus u-right u-margin-left"><%= Html.Resource("AddNewTicket") %></a>
							
				
			</div>
			
		      <table class="list display" id="ticketsTable">
                <thead>
                <tr>
                    <th>
                        <%= Html.Resource("TicketID") %>
                    </th>
                     <th>
                        <%= Html.Resource("Date")%>
                    </th>
                    <th>
                        <%= Html.Resource("Subject")%>
                    </th>
                    <th>
                        <%= Html.Resource("Status")%>
                    </th>
                    <th>
                        <%= Html.Resource("LastReply")%>
                    </th>                  
                    <th>
                        <%= Html.Resource("Actions")%>
                    </th>
					
                </tr>
                </thead>
                <tbody>
                    <tr><td></td><td></td><td></td><td></td><td></td><td></td></tr>
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