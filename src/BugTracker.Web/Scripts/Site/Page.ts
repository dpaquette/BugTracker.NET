declare var $: any;
module BugTracker {
    export class Page {
        constructor() {
            $("[data-action=submit]").on("click", (event) => {
                event.target.closest("form").submit();
            });
        }
    }
}

var page = new BugTracker.Page();