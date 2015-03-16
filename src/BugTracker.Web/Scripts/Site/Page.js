var BugTracker;
(function (BugTracker) {
    var Page = (function () {
        function Page() {
            $("[data-action=submit]").on("click", function (event) {
                event.target.closest("form").submit();
            });
        }
        return Page;
    })();
    BugTracker.Page = Page;
})(BugTracker || (BugTracker = {}));

var page = new BugTracker.Page();
