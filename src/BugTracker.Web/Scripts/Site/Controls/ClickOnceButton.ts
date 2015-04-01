module BugTracker.Controls {
    export class ClickOnceButton {
        constructor(selector:string) {
            $(selector).on("click", (event) => this.disableButton(event));
        }

        disableButton(event) {
            $(event.target).attr("disabled", true);
            $(event.target).val("Updating...");
        }
    }
} 