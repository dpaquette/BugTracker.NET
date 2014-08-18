/*
Copyright 2002-2009 Corey Trager
Distributed under the terms of the GNU General Public License
*/

function on_body_unload()
{
	// don't leave stray child windows
	if (popup_window != null)
	{
		popup_window.close();
	}
}

function set_relationship_cnt(bugid, cnt)
{
	if (bugid == this_bugid) 
	{
		el = get_el("relationship_cnt");
		set_text(el,cnt)
	}
}

function set_task_cnt(bugid, cnt)
{
	if (bugid == this_bugid) 
	{
		el = get_el("task_cnt");
		set_text(el,cnt)
	}
}


var popup_window = null
function open_popup_window(url, title, bugid, width, height)
{
	var url_and_vars = url + '?id=' + bugid
	
	popup_window = window.open(
		url_and_vars,
		'bug',
		"menubar=0,scrollbars=1,toolbar=0,resizable=1,width=" + width + ",height=" + height)
		
	popup_window.focus()
}

var dirty = false;
function mark_dirty()
{
	dirty = true
}

function my_confirm()
{
	return confirm('You have unsaved changes.  Do you want to leave this page and lose your changes?.')
}

function warn_if_dirty(event)
{
	if (dirty)
	{
		result = my_confirm()
		if (!result)
		{
			event.preventDefault()
		}
	}
}

function send_email(id)
{
	if (dirty)
	{
		var result = my_confirm()
		if (result)
		{
			window.document.location = "send_email.aspx?bg_id=" + id;
		}
	}
	else
	{
		window.document.location = "send_email.aspx?bg_id=" + id;
	}
}

function handle_rewrite_posts(data, status)
{
	$("#posts").html(data)
	$(".warn").click(warn_if_dirty)
	$.get("get_db_datetime.aspx","",handle_get_bug_date)
	start_animation()
}

function handle_get_bug_date(data, status)
{
	var el = document.getElementById("snapshot_timestamp")
	el.value = data
}

function rewrite_posts(bugid)
{
	var images_inline = get_cookie("images_inline")
	var history_inline = get_cookie("history_inline")

	var url = "write_posts.aspx?images_inline=" + images_inline
		+ "&history_inline=" + history_inline
		+ "&id=" + bugid
	
	$.get(url, "", handle_rewrite_posts)
}

function toggle_notifications(bugid)
{
	var el = get_el("get_stop_notifications");
	var text = get_text(el)
	
	var url = "subscribe.aspx?ses="
		+ get_cookie("se_id")
		+ "&id=" 
		+ bugid
		+ "&actn="
		
	if (text == "get notifications")
		url += "1"
	else
		url += "0"

	$.get(url)

	// modify text in web page	
	if (text == "get notifications")
	{
		set_text(el,"stop notifications")
	}
	else
	{
		set_text(el, "get notifications")
	}
}


function toggle_images2(bugid)
{
	var images_inline = get_cookie("images_inline")
	if (images_inline == "1")
	{
		images_inline = "0"
		set_text(get_el("hideshow_images"), "show inline images")
	}
	else
	{
		images_inline = "1"
		set_text(get_el("hideshow_images"),"hide inline images")
	}

	set_cookie("images_inline",images_inline)

	rewrite_posts(bugid)
}

function toggle_history2(bugid)
{
	var history_inline = get_cookie("history_inline")
	if (history_inline == "1")
	{
		history_inline = "0"
		set_text(get_el("hideshow_history"), "show change history")
	}
	else
	{
		history_inline = "1"
		set_text(get_el("hideshow_history"), "hide change history")
	}

	set_cookie("history_inline",history_inline)

	rewrite_posts(bugid)
}



function resize_iframe(elid, delta)
{
	var el = get_el(elid);

	if (parseInt($(el).height()) + parseInt(delta) < 100)
	{
		el.style.height = "100";
	}
	else
	{
		el.style.height = (parseInt($(el).height()) + parseInt(delta)) + "px";
	}

}


function resize_image(elid, delta)
{
	var el = get_el(elid);
	if (parseFloat(el.height) * parseFloat(delta) < 5
	|| parseFloat(el.width) * parseFloat(delta) < 5)
	{
		// do nothing
	}
	else
	{
		var h = parseInt((parseFloat(el.height) * parseFloat(delta)));
		var w = parseInt((parseFloat(el.width) * parseFloat(delta)));
		el.height = h;
		el.width = w;
	}
}


// prevent user from hitting "Submit" twice
function on_user_hit_submit()
{
    $("#user_hit_submit").val("1")
    $("#submit_button").attr('disabled', 'disabled')
    $("#submit_button2").attr('disabled', 'disabled')
    $("#submit_button").val("Please wait...")
    $("#submit_button2").val("Please wait...")
    var btn = document.getElementById("submit_button")
    btn.form.submit()
	
}
function set_cookie(name,value)
{
	var date = new Date();

	// expire in 10 years
	date.setTime(date.getTime()+(3650*24*60*60*1000));

	document.cookie = name +"=" + value
		+ ";expires=" + date.toGMTString();
		+ ";path=/";
}


function get_cookie(name)
{
	var nameEQ = name + "=";
	var ca = document.cookie.split(';');
	for(var i=0;i < ca.length;i++) {
		var c = ca[i];
		while (c.charAt(0)==' ') c = c.substring(1,c.length);
		if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
	}
	return null;
}

function save_var(name)
{
	var el = get_el(name)
	if (el != null)
	{
		var val = el.options[el.selectedIndex].text;
		set_cookie(name, val)
	}
}

function get_preset(name)
{
	var el = get_el(name)
	if (el != null)
	{
		val = get_cookie(name)
		
		if (val != null)
		{
			for (i = 0; i < el.options.length; i++)
			{
				if (el.options[i].text == val)
				{
					el.options[i].selected = true
					break
				}
			}
		}
	}
}

function get_presets()
{
	get_preset("category")
	get_preset("priority")
	get_preset("status")
	get_preset("udf")
	get_preset("assigned_to")
//	get_preset("pcd1")

	on_body_load() // to change the select styles
}

function set_presets()
{
	save_var("category")
	save_var("priority")
	save_var("status")
	save_var("udf")
	save_var("assigned_to")
//strange side effect with these.  The browser remembers the saved presets even if user doesn't click "use"
//	save_var("pcd1")
}

function clone()
{
	el = get_el("bugid")
	set_text(el,"")

	el = get_el("bugid_label")
	set_text(el,"")

	el = get_el("submit_button")
	el.value = "Create"
	
	try
	{
		el = get_el("submit_button2")
		el.value = "Create"
	}
	catch (e)
	{
	
	}

	el = get_el("posts")
	el.innerHTML = ""

	el = get_el("clone_ignore_bugid")
	el.value = "1"

	el = get_el("edit_bug_menu")
	el.style.display = "none"

}

var cls = null
var ie = null

function get_el(id) 
{
	return document.getElementById(id)
}

function get_text(el)
{
	return el.firstChild.nodeValue
}


function set_text(el, text)
{
	return el.firstChild.nodeValue = text
}


function on_body_load()
{

	ie = (navigator.userAgent.indexOf("MSIE") > 0)
	cls = (navigator.userAgent.indexOf("MSIE") > 0) ? "className" : "class";
	
	sels = document.getElementsByTagName("select");
	
	// resize the options, making them all as wide as the widest
	max_width = 0

	for (i = 0; i < sels.length; i++)
	{
		if (sels[i].offsetWidth > max_width)
		{
			max_width = sels[i].offsetWidth;
		}
	}

	max_width += 10; // a little fudge factor, because of the bold text

	for (i = 0; i < sels.length; i++)
	{
		sels[i].style.width = max_width; 
	}
	
	change_dropdown_style();
	

	spans = document.getElementsByTagName("span");

	for (i = 0; i < spans.length; i++)
	{
		if (spans[i].getAttribute(cls) == "static")
		{
			if (get_text(spans[i]).indexOf("[no") > -1)
			{
				spans[i].setAttribute(cls,'edit_bug_static_none')
			}
			else
			{
				spans[i].setAttribute(cls,'edit_bug_static')
			}
		}
	}

	var short_desc = document.getElementById("short_desc")

	dirty = false 

	if (short_desc != null)
		short_desc.title = short_desc.value

	start_animation()	
}

function change_dropdown_style()
{
	sels = document.getElementsByTagName("select");

	// change the select styles depending on whether something has been selected or not
	for (i = 0; i < sels.length; i++)
	{
		if (sels[i].id != "project")
		{
			sels[i].onchange = change_dropdown_style
		}
		si = sels[i].options.selectedIndex;
		if (sels[i].options[si].text.substr(0,3) == "[no")
		{
			sels[i].setAttribute(cls,'edit_bug_option_none')
		}
		else
		{
			sels[i].setAttribute(cls,'edit_bug_option')
		}
	}

	mark_dirty()

}

var ren = new RegExp( "\\n", "g" )
var ren2 = new RegExp( "\\n\\n", "g" )

function count_chars(textarea_id, max)
{
	mark_dirty()
	
	var textarea = get_el(textarea_id)
	var count_span = get_el(textarea_id + "_cnt");

	// \n counts as two chars by the time we insert,
	// so double them here for the purpose of counting
	var s = textarea.value.replace(ren,"\n\n")
	var len = s.length

	if (s.length > max)
	{
		// truncate
		var s = s.substr(0,max)
		// convert the \n\n back to \n
		textarea.value = s.replace(ren2,"\n")

		set_text(count_span,"0 more characters allowed")
	}
	else
	{
		set_text(count_span, (max - len) + " more characters allowed")
	}

	return true
}


function show_tags() // also in bug_list.js
{
	popup_window = window.open(
		'tags.aspx',
		'tags',
		"menubar=0,scrollbars=1,toolbar=0,resizable=1,width=500,height=400")

	popup_window.focus()
}

function append_tag(s) // also in bug_list.js, different element
{
	el = get_el("tags")

	tags = el.value.split(",")

	for (i = 0; i < tags.length; i++)
	{
		s2 = tags[i].replace(/^\s+|\s+$/g,"") // trim
		if (s == s2)
		{
			return; // already entered
		}
	}

	if (el.value != "")
	{
		el.value += ","
	}

	el.value += s;
}


function done_selecting_tags()
{
}

var color = 128
var timer = null
var new_posts = null
var new_posts_length
var hex_chars = "0123456789ABCDEF"

function decimal_to_hex(dec)
{
	var result = 
		hex_chars.charAt(Math.floor(dec / 16))
		+ hex_chars.charAt(dec % 16)
	return result
}

function RGB2HTML(red, green, blue)
{
	var rgb = "#"
	rgb += String(decimal_to_hex(red));
	rgb += String(decimal_to_hex(green));
	rgb += String(decimal_to_hex(blue));
	return rgb
}

function start_animation()
{
	color = 100
	
	if (navigator.userAgent.indexOf("MSIE") > 0)
		new_posts = getElementsByName_for_ie6_and_ie7("td","new_post") 
	else
		new_posts = document.getElementsByName("new_post") 
	
	new_posts_length = new_posts.length

	if (new_posts_length > 0)
	{
		timer = setInterval(timer_callback,5)
	}
}

function timer_callback()
{
	color++
	
	for (i = 0; i < new_posts_length; i++)
	{
		new_posts[i].style.background = RGB2HTML(color,255,color)
	}
	
	if (color == 255) // if the color is now white
	{
		clearInterval(timer)
	}
}

function getElementsByName_for_ie6_and_ie7(tag, name) {

	var elem = document.getElementsByTagName(tag);
	var arr = new Array();
	for(i = 0,iarr = 0; i < elem.length; i++)
	{
		att = elem[i].getAttribute("name");
		if(att == name)
		{
			arr[iarr] = elem[i];
			iarr++;
		}
	}
	return arr;
}	

function show_calendar(el)
{
	$("#" + el).datepicker("show")
}

