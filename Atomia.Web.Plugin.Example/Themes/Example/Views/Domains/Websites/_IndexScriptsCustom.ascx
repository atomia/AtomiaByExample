<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<script>
    (function () {
        // Extend a table cell template with your own content.
        // (The template content is a text node, not an element node.)
		var templateContent = $("#js-actions-cell").text();
		$("#js-actions-cell").text('<a class="Btn Btn--small" href="http://www.atomia.com" target="_blank">ATOMIA</a>' + templateContent);

        // Extend an element on the page that is not a template.
		$(".js-bulk-actions").append('<a class="Btn u-right u-margin-right" href="http://www.atomia.com" target="_blank">ATOMIA</a>');
	})();
</script>
