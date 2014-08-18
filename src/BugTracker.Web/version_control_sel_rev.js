var sel_rev_0 = ""
var sel_row_0 = ""
var sel_rev_1 = 0
var sel_row_1 = 0
var sel_path_0 = ""
var sel_path_1 = ""

function desel_for_diff(rev) {
    var el = document.getElementById(rev)
    el.firstChild.nodeValue = "select for diff"
    el.style.fontWeight = ""
    el.style.background = ""
}


function sel_for_diff(row, rev, path) {
    var el = document.getElementById(rev)

    if (el.firstChild.nodeValue == "[SELECTED]") {
        desel_for_diff(rev)

        // go from 1 or 2 selection to 1 or 0
        if (rev = sel_rev_0) {
            sel_rev_0 = sel_rev_1
            sel_row_0 = sel_row_1
            sel_path_0 = sel_path_1
        }

        sel_rev_1 = ""
        sel_row_1 = 0
        sel_path_1 = ""

    }
    else {
        if (sel_rev_1 != "") {
            desel_for_diff(sel_rev_1)
        }

        sel_rev_1 = sel_rev_0
        sel_row_1 = sel_row_0
        sel_path_1 = sel_path_0

        sel_rev_0 = rev
        sel_row_0 = row
        sel_path_0 = path

        el.firstChild.nodeValue = "[SELECTED]"
        el.style.fontWeight = "bold"
        el.style.background = "yellow"

    }

    // enable, disable link
    if (sel_rev_0 != "" && sel_rev_1 != "" && sel_rev_0 != sel_rev_1) {
        document.getElementById("do_diff_enabled").style.display = "block"
        document.getElementById("do_diff_disabled").style.display = "none"
    }
    else {
        document.getElementById("do_diff_enabled").style.display = "none"
        document.getElementById("do_diff_disabled").style.display = "block"
    }

}

function on_do_diff() {

    if (sel_rev_0 != "" && sel_rev_1 != "" && sel_rev_0 != sel_rev_1) {
        if (sel_row_0 < sel_row_1)  // if 1 is newer, put one on right of diff
        {
            frm.rev_0.value = sel_rev_1
            frm.rev_1.value = sel_rev_0
            try {
                frm.path_0.value = sel_path_1
                frm.path_1.value = sel_path_0
            } catch (e) {// Do nothing
            }
        }
        else {
            frm.rev_0.value = sel_rev_0
            frm.rev_1.value = sel_rev_1
            try {
                frm.path_0.value = sel_path_0
                frm.path_1.value = sel_path_1
            } catch (e) {// Do nothing
            }
        }

        frm.submit()
    }
    else {
        alert("First select two commits to diff (compare side-by-side)")
    }
}
