var BugTracker;
(function (BugTracker) {
    var EditBug = (function () {
        function EditBug() {
            var _this = this;
            this.popup_window = null;
            this.cls = null;
            this.ie = null;
            this.color = 128;
            this.timer = null;
            this.new_posts = null;
            this.hex_chars = "0123456789ABCDEF";
            this.bugId = parseInt($("[data-bug-id]").attr("data-bug-id"));
            this.isSubscribed = $("[data-is-subscribed]").attr("data-is_subscribed") === "True";
            $("[data-action=send_email] a").on("click", function () {
                return _this.send_email(_this.bugId);
            });
            $("[data-action=send_email] a").on("click", function () {
                return _this.clone();
            });
            $("[data-action=add_attachment] a").on("click", function () {
                return _this.add_attachment();
            });
            $("[data-action=notifications] a").on("click", function () {
                return _this.toggle_notifications(_this.bugId);
            });

            new BugTracker.Controls.LengthLimitedTextArea("short_desc");

            this.dirtyFlag = new BugTracker.Controls.DirtyFlag($("form[name=aspnetForm]"));
            $(".warn").click(function (event) {
                return _this.warnIfDirty(event);
            });

            new BugTracker.Controls.ClickOnceButton("input[type=submit]");
            $("input[type=submit]").on("click", function () {
                return $("form[name=aspnetForm]").submit();
            });
        }
        EditBug.prototype.add_attachment = function () {
            this.open_popup_window('add_attachment.aspx', 'add attachment ', this.bugId, 600, 300);
        };

        EditBug.prototype.on_body_unload = function () {
            // don't leave stray child windows
            if (this.popup_window != null) {
                this.popup_window.close();
            }
        };

        EditBug.prototype.set_relationship_cnt = function (bugid, cnt) {
            if (bugid == this.bugId) {
                var el = this.get_el("relationship_cnt");
                this.set_text(el, cnt);
            }
        };

        EditBug.prototype.open_popup_window = function (url, title, bugid, width, height) {
            var url_and_vars = url + '?id=' + bugid;

            this.popup_window = window.open(url_and_vars, 'bug', "menubar=0,scrollbars=1,toolbar=0,resizable=1,width=" + width + ",height=" + height);

            this.popup_window.focus();
        };

        EditBug.prototype.confirmPageLeave = function () {
            return confirm('You have unsaved changes.  Do you want to leave this page and lose your changes?.');
        };

        EditBug.prototype.warnIfDirty = function (event) {
            if (this.dirtyFlag.isDirty) {
                var result = this.confirmPageLeave();
                if (!result) {
                    event.preventDefault();
                }
            }
        };

        EditBug.prototype.send_email = function (id) {
            console.log("sending email");
            if (this.dirtyFlag.isDirty) {
                var result = this.confirmPageLeave();
                if (result) {
                    window.document.location.href = window.document.location.protocol + window.document.location.host + "/send_email.aspx?bg_id=" + id;
                }
            } else {
                window.document.location.href = window.document.location.protocol + window.document.location.host + "/send_email.aspx?bg_id=" + id;
            }
        };

        EditBug.prototype.handle_rewrite_posts = function (data, status) {
            $("#posts").html(data);

            $.get("get_db_datetime.aspx", "", this.handle_get_bug_date);
            this.start_animation();
        };

        EditBug.prototype.handle_get_bug_date = function (data, status) {
            var el = $("#snapshot_timestamp");
            el.val(data);
        };

        EditBug.prototype.rewrite_posts = function (bugid) {
            var images_inline = BugTracker.Util.Cookie.Get("images_inline");
            var history_inline = BugTracker.Util.Cookie.Get("history_inline");

            var url = "write_posts.aspx?images_inline=" + images_inline + "&history_inline=" + history_inline + "&id=" + bugid;

            $.get(url, "", this.handle_rewrite_posts);
        };

        EditBug.prototype.toggle_notifications = function (bugid) {
            var url = "subscribe.aspx?ses=" + BugTracker.Util.Cookie.Get("se_id") + "&id=" + bugid + "&actn=";

            if (this.isSubscribed)
                url += "0";
            else
                url += "1";

            $.get(url);
            this.set_notification_label();

            // modify text in web page
            this.isSubscribed = !this.isSubscribed;
        };

        EditBug.prototype.set_notification_label = function () {
            if (this.isSubscribed) {
                $("[data-id=notifications-label").text("Stop notifications");
            } else {
                $("[data-id=notifications-label").text("Get notifications");
            }
        };

        EditBug.prototype.toggle_images2 = function (bugid) {
            var images_inline = BugTracker.Util.Cookie.Get("images_inline");
            if (images_inline == "1") {
                images_inline = "0";
                this.set_text(this.get_el("hideshow_images"), "show inline images");
            } else {
                images_inline = "1";
                this.set_text(this.get_el("hideshow_images"), "hide inline images");
            }

            BugTracker.Util.Cookie.Set("images_inline", images_inline);

            this.rewrite_posts(bugid);
        };

        EditBug.prototype.toggle_history2 = function (bugid) {
            var history_inline = BugTracker.Util.Cookie.Get("history_inline");
            if (history_inline == "1") {
                history_inline = "0";
                this.set_text(this.get_el("hideshow_history"), "show change history");
            } else {
                history_inline = "1";
                this.set_text(this.get_el("hideshow_history"), "hide change history");
            }

            BugTracker.Util.Cookie.Set("history_inline", history_inline);

            this.rewrite_posts(bugid);
        };

        EditBug.prototype.resize_iframe = function (elid, delta) {
            var el = this.get_el(elid);

            if (parseInt($(el).height()) + parseInt(delta) < 100) {
                el.style.height = "100";
            } else {
                el.style.height = (parseInt($(el).height()) + parseInt(delta)) + "px";
            }
        };

        EditBug.prototype.resize_image = function (elid, delta) {
            var el = $("#" + elid);
            if (parseFloat(el.height) * parseFloat(delta) < 5 || parseFloat(el.width) * parseFloat(delta) < 5) {
                // do nothing
            } else {
                var h = parseInt((parseFloat(el.height) * parseFloat(delta)).toString());
                var w = parseInt((parseFloat(el.width) * parseFloat(delta)).toString());
                el.height = h;
                el.width = w;
            }
        };

        // prevent user from hitting "Submit" twice
        EditBug.prototype.on_user_hit_submit = function () {
            $("#user_hit_submit").val("1");
        };

        EditBug.prototype.save_var = function (name) {
            var el = this.get_el(name);
            if (el != null) {
                var val = el.options[el.selectedIndex].text;
                BugTracker.Util.Cookie.Set(name, val);
            }
        };

        EditBug.prototype.get_preset = function (name) {
            var el = this.get_el(name);
            if (el != null) {
                var val = BugTracker.Util.Cookie.Get(name);

                if (val != null) {
                    for (var i = 0; i < el.options.length; i++) {
                        if (el.options[i].text == val) {
                            el.options[i].selected = true;
                            break;
                        }
                    }
                }
            }
        };

        EditBug.prototype.get_presets = function () {
            this.get_preset("category");
            this.get_preset("priority");
            this.get_preset("status");
            this.get_preset("udf");
            this.get_preset("assigned_to");

            //	get_preset("pcd1")
            this.on_body_load(); // to change the select styles
        };

        EditBug.prototype.set_presets = function () {
            this.save_var("category");
            this.save_var("priority");
            this.save_var("status");
            this.save_var("udf");
            this.save_var("assigned_to");
            //strange side effect with these.  The browser remembers the saved presets even if user doesn't click "use"
            //	save_var("pcd1")
        };

        EditBug.prototype.clone = function () {
            var el = this.get_el("bugid");
            this.set_text(el, "");

            el = this.get_el("bugid_label");
            this.set_text(el, "");

            el = this.get_el("submit_button");
            el.value = "Create";

            try  {
                el = this.get_el("submit_button2");
                el.value = "Create";
            } catch (e) {
            }

            el = this.get_el("posts");
            el.innerHTML = "";

            el = this.get_el("clone_ignore_bugid");
            el.value = "1";

            el = this.get_el("edit_bug_menu");
            el.style.display = "none";
        };

        EditBug.prototype.get_el = function (id) {
            return document.getElementById(id);
        };

        EditBug.prototype.get_text = function (el) {
            return el.firstChild.nodeValue;
        };

        EditBug.prototype.set_text = function (el, text) {
            return el.firstChild.nodeValue = text;
        };

        EditBug.prototype.on_body_load = function () {
            this.ie = (navigator.userAgent.indexOf("MSIE") > 0);
            this.cls = (navigator.userAgent.indexOf("MSIE") > 0) ? "className" : "class";
            var sels = document.getElementsByTagName("select");

            // resize the options, making them all as wide as the widest
            var max_width = 0;

            for (var i = 0; i < sels.length; i++) {
                if (sels[i].offsetWidth > max_width) {
                    max_width = sels[i].offsetWidth;
                }
            }

            max_width += 10; // a little fudge factor, because of the bold text

            for (var i = 0; i < sels.length; i++) {
                sels[i].style.width = max_width.toString();
            }

            this.change_dropdown_style();

            var spans = document.getElementsByTagName("span");

            for (i = 0; i < spans.length; i++) {
                if (spans[i].getAttribute(this.cls) == "static") {
                    if (this.get_text(spans[i]).indexOf("[no") > -1) {
                        spans[i].setAttribute(this.cls, 'edit_bug_static_none');
                    } else {
                        spans[i].setAttribute(this.cls, 'edit_bug_static');
                    }
                }
            }

            var short_desc = document.getElementById("short_desc");

            if (short_desc != null)
                short_desc.title = short_desc.value;

            this.start_animation();
            this.set_notification_label();
        };

        EditBug.prototype.change_dropdown_style = function () {
            var _this = this;
            var sels = document.getElementsByTagName("select");

            for (var i = 0; i < sels.length; i++) {
                if (sels[i].id != "project")
                    ;
                 {
                    sels[i].onchange = function () {
                        return _this.change_dropdown_style();
                    };
                }
            }
        };

        EditBug.prototype.show_tags = function () {
            this.popup_window = window.open('tags.aspx', 'tags', "menubar=0,scrollbars=1,toolbar=0,resizable=1,width=500,height=400");

            this.popup_window.focus();
        };

        EditBug.prototype.append_tag = function (s) {
            var el = this.get_el("tags");

            var tags = el.value.split(",");

            for (var i = 0; i < tags.length; i++) {
                var s2 = tags[i].replace(/^\s+|\s+$/g, "");
                if (s == s2) {
                    return;
                }
            }

            if (el.value != "") {
                el.value += ",";
            }

            el.value += s;
        };

        EditBug.prototype.done_selecting_tags = function () {
        };

        EditBug.prototype.decimal_to_hex = function (dec) {
            var result = this.hex_chars.charAt(Math.floor(dec / 16)) + this.hex_chars.charAt(dec % 16);
            return result;
        };

        EditBug.prototype.RGB2HTML = function (red, green, blue) {
            var rgb = "#";
            rgb += String(this.decimal_to_hex(red));
            rgb += String(this.decimal_to_hex(green));
            rgb += String(this.decimal_to_hex(blue));
            return rgb;
        };

        EditBug.prototype.start_animation = function () {
            this.color = 100;

            if (navigator.userAgent.indexOf("MSIE") > 0)
                this.new_posts = document.querySelector('td[name=new_post]');
            else
                this.new_posts = document.getElementsByName("new_post");

            var new_posts_length = this.new_posts.length;

            if (new_posts_length > 0) {
                this.timer = setInterval(this.timer_callback, 5);
            }
        };

        EditBug.prototype.timer_callback = function () {
            this.color++;

            for (var i = 0; i < this.new_posts_length; i++) {
                this.new_posts[i].style.background = this.RGB2HTML(this.color, 255, this.color);
            }

            if (this.color == 255) {
                clearInterval(this.timer);
            }
        };

        EditBug.prototype.show_calendar = function (el) {
            $("#" + el).datepicker("show");
        };
        return EditBug;
    })();
    BugTracker.EditBug = EditBug;
})(BugTracker || (BugTracker = {}));
//# sourceMappingURL=EditBug.js.map
