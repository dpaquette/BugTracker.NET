function handle_suggestion(data, status)
{
	if (data != "")
	{
		var mypopup = document.getElementById("suggest_popup");
		mypopup.innerHTML = data
		var sel = document.getElementById("suggest_select")


		// position and show the popup
		var mylike = document.getElementById("like")
		sel.style.width = mylike.offsetWidth
		mypopup.style.left = $(mylike).offset().left
		mypopup.style.top = $(mylike).offset().top + (mylike.offsetHeight-3)
		mypopup.style.display = "block";
	}
	else
	{
		hide_suggest();
	}
}

var prev_desc = ""

function get_suggestion()
{
	mylike = document.getElementById("like")
	s = mylike.value

	if (s.length >= search_suggest_min_chars)
	{
		if (s != prev_desc)
		{
			prev_desc = s
			var url = "ajax2.aspx?q=" + s
			$.get(url, "", handle_suggestion)
		}
	}
	else
	{
		hide_suggest()
	}
}

var suppress_suggest = false

function select_suggestion(el)
{
	sel = document.getElementById("suggest_select")
	mylike = document.getElementById("like")

	mylike.value = sel.options[sel.selectedIndex].text
	on_change()

	hide_suggest()

	// workaround for strange behavior
	suppress_suggest = true
	mylike.focus()
}


function hide_suggest()
{
	var mypopup = document.getElementById("suggest_popup");
	mypopup.style.display = "none";
}

// look at the key entered when the suggest dropdown has focus
function suggest_sel_onkeydown(el, ev)
{
	keynum = 0

	if (window.event)
	{
		keynum = window.event.keyCode
	}
	else if (ev.which)
	{
		keynum = ev.which
	}

	// enter - done with suggest list
	if (keynum == 13)
	{
		select_suggestion(el)
		return false
	}
	// escape
	else if (keynum == 27)
	{
		hide_suggest()
		return false
	}

	return true
}


// look at the key when the "description contains" has focus
function search_criteria_onkeydown(el, ev)
{

	keynum = 0

	if (window.event)
	{
		keynum = window.event.keyCode
	}
	else if (ev.which)
	{
		keynum = ev.which
	}

	// down arrow - jump into suggest list
	if (keynum == 40)
	{
		sel = document.getElementById("suggest_select")
		if (sel)
		{
			sel.focus()
		}
	}
	// tab
	else if (keynum == 9)
	{
		hide_suggest()
	}
}

function search_criteria_onkeyup(el, ev)
{
	on_change()

	if (!suppress_suggest)
	{
		get_suggestion()
	}
	// User hit enter in the dropdown
	// and we shifted focus to the text input.
	// Unless we suppress the enter, the form submits, which we don't want.
	else
	{
		suppress_suggest = false
	}
}