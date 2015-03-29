var BugTracker;
(function (BugTracker) {
    ///<reference path="../../typings/jquery/jquery.d.ts"/>
    (function (Controls) {
        var DirtyFlag = (function () {
            function DirtyFlag(container) {
                var _this = this;
                this.container = container;
                this.isDirty = false;
                container.on("change", "input", function () {
                    return _this.markDirty();
                });
                container.on("change", "select", function () {
                    return _this.markDirty();
                });
                container.on("change", "textarea", function () {
                    return _this.markDirty();
                });
            }
            DirtyFlag.prototype.markDirty = function () {
                this.isDirty = true;
            };
            return DirtyFlag;
        })();
        Controls.DirtyFlag = DirtyFlag;
    })(BugTracker.Controls || (BugTracker.Controls = {}));
    var Controls = BugTracker.Controls;
})(BugTracker || (BugTracker = {}));
//# sourceMappingURL=DirtyFlag.js.map
