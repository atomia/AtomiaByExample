<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script type="text/javascript">
    var Atomia = Atomia || {};
    Atomia.URLS = Atomia.URLS || {};
    Atomia.RESX = Atomia.RESX || {};

    $.extend(Atomia.URLS, {
        performIncrement: '<%= Url.Action("PerformIncrement", new { area = "Example", controller = "Example" }) %>',
        performDecrement: '<%= Url.Action("PerformDecrement", new { area = "Example", controller = "Example" }) %>'
    });
</script>
