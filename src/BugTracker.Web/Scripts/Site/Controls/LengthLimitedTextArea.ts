module BugTracker.Controls {
    export class LengthLimitedTextArea {
        constructor(public textAreaId: string) {
            $("#" + textAreaId).on("keyup", () => this.count_chars(textAreaId, 200));
        }
        count_chars(textarea_id, max) {

            var textarea = $("#" + textarea_id);
            var count_span = $("#" + textarea_id + "_cnt");

            // \n counts as two chars by the time we insert,
            // so double them here for the purpose of counting
            var ren = new RegExp("\\n", "g");
            
            var s = textarea.val().replace(ren, "\n\n");
            var len = s.length;

            if (s.length > max) {
                // truncate
                var s = s.substr(0, max);
                // convert the \n\n back to \n
                var ren2 = new RegExp("\\n\\n", "g");
                textarea.val(s.replace(ren2, "\n"));

                count_span.text("0 more characters allowed");
            }
            else {
                count_span.text((max - len) + " more characters allowed");
            }

            return true;
        }
    }
}