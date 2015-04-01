/*
Copyright 2002-2009 Corey Trager
Distributed under the terms of the GNU General Public License
*/





$(document).ready(function () {
    var page = new BugTracker.EditBug();
    var date_format = $("[data-date-format]").attr("data-date-format");
    $(".date").datepicker({ dateFormat: date_format, duration: 'fast' });
    $(".date").change(page.mark_dirty);
    $(".warn").click(page.warn_if_dirty);
    if ($("[data-use-fck-editor]").length > 0) {
        CKEDITOR.replace('comment');
    }

    page.on_body_load();
    $(document).on("unload", "body", function () { on_body_unload(); });
});