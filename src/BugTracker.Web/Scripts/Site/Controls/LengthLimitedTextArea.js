var BugTracker;
(function (BugTracker) {
    (function (Controls) {
        var LengthLimitedTextArea = (function () {
            function LengthLimitedTextArea(textAreaId) {
                var _this = this;
                this.textAreaId = textAreaId;
                $("#" + textAreaId).on("keyup", function () {
                    return _this.count_chars(textAreaId, 200);
                });
            }
            LengthLimitedTextArea.prototype.count_chars = function (textarea_id, max) {
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
                } else {
                    count_span.text((max - len) + " more characters allowed");
                }

                return true;
            };
            return LengthLimitedTextArea;
        })();
        Controls.LengthLimitedTextArea = LengthLimitedTextArea;
    })(BugTracker.Controls || (BugTracker.Controls = {}));
    var Controls = BugTracker.Controls;
})(BugTracker || (BugTracker = {}));
//# sourceMappingURL=LengthLimitedTextArea.js.map
