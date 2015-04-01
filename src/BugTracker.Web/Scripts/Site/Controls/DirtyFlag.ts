///<reference path="../../typings/jquery/jquery.d.ts"/>
module BugTracker.Controls {
    export class DirtyFlag {
        isDirty = false;
        constructor(public container: JQuery) {
            container.on("change", "input", () => this.markDirty());
            container.on("change", "select", () => this.markDirty());
            container.on("change", "textarea", () => this.markDirty());
        }

        markDirty() {
            this.isDirty = true;
        }
    }
}