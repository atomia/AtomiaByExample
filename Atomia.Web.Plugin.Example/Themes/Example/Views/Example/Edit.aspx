<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Atomia.Web.Plugin.Example.Models.ExampleModel>" %>
<%@ Import Namespace="Atomia.Web.Plugin.Files.Models"%>

<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
   <%= Html.Resource("Title") %>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
        
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptContentPlaceHolder" runat="server">

</asp:Content>