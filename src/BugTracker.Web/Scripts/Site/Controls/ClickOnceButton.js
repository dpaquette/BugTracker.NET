var BugTracker;
(function (BugTracker) {
    (function (Controls) {
        var ClickOnceButton = (function () {
            function ClickOnceButton(selector) {
                var _this = this;
                $(selector).on("click", function (event) {
                    return _this.disableButton(event);
                });
            }
            ClickOnceButton.prototype.disableButton = function (event) {
                $(event.target).attr("disabled", true);
                $(event.target).val("Updating...");
            };
            return ClickOnceButton;
        })();
        Controls.ClickOnceButton = ClickOnceButton;
    })(BugTracker.Controls || (BugTracker.Controls = {}));
    var Controls = BugTracker.Controls;
})(BugTracker || (BugTracker = {}));
//# sourceMappingURL=ClickOnceButton.js.map
