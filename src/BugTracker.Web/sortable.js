/*********************************************************************/
var prevcol
var asc
var col
var type
var qt = "\""

var gecko = navigator.userAgent.toLowerCase().indexOf("gecko") > -1;

/*********************************************************************/
function create_xls() {

var cnt=0;

	table = document.getElementById("mytable");
	var s = ""
	for (var i = 0; i < table.rows.length; i++) {
		var row = table.rows(i);
		var line = "";
		for (var j = 0; j < row.cells.length; j++) {
			if (line != "") line += "\t";
			line += row.cells(j).innerText.replace(/\r\n/ig,'/');
		}
		line += "\n";
		s += line;
	}

	document.getElementById("xls").value = s;
	document.getElementById("xlsholder").style.display = "block";
	document.getElementById("xlsanchor").scrollIntoView();

}

/*********************************************************************/
function onload() {
	prevcol = -1
	asc = true
}




/*********************************************************************/
function sort_by_col (c, t) {
	document.getElementById("wait").firstChild.nodeValue = "Sorting.  Please wait...";
	col = c
	type = t
	setTimeout("sort_by_col_impl()",1)
}

/*********************************************************************/
function get_number(val) {
	
	var f = parseFloat(val);	
	
	if (isNaN(f)) {
		return 0
	}
	else {
		return f
	}
}


/*********************************************************************/
function get_date(val) {
	var x = Date.parse(val)

	if (isNaN(x))
		return val
	else
		return x
}

/*********************************************************************/
function sort_by_col_impl () {

	table = document.getElementById("mytable");
	holder = document.getElementById("myholder");

	var myarray = new Array()

	// load an array with the cells
	var i, j
	var len = table.rows.length
	for (i = 1; i < len; i++) {
		j = i - 1
		myarray[j] = new Object
		myarray[j].cells = table.rows[i].cells
		myarray[j].val = getInnerTextHelper(table.rows[i].cells[col])
	}


	// clicking on a column twice toggles the sort order
	if (col == prevcol) {
		if (asc)
			asc = false
		else
			asc = true
	}
	else {
		asc = true
	}


	document.getElementById("sortedby").firstChild.nodeValue = "Sorted by "
	+ getInnerTextHelper(table.rows[0].cells[col])
	+ (asc ? " asc" : " desc")

	if (type == "num") {
		myarray.sort(function compare_number(a, b) {
				return flip(get_number(a.val) - get_number(b.val))
			}
		)
	}
	else if (type == "date") {
		myarray.sort(function compare_date(a, b) {
				if (get_date(b.val) < get_date(a.val)) {
					return flip(1)
				}
				else {
					return flip(-1)
				}

			}
		)
	}
	else {
		myarray.sort(function compare_string(a, b) {
				if (b.val.toUpperCase() < a.val.toUpperCase())
					return flip(1)
				else
					return flip(-1)
			}
		)
	}


// for speed, use string array and join rather
// than concatenation

	var k = 0
	
	var string_array = new Array()

	
	string_array[k++] = "<table id=" + table.id
		+ " border=1" 
		+ " class=datat" 
		+ ">"
		+ (gecko ? GET_OUTER_HTML(table.rows[0]) : table.rows[0].outerHTML)

	// use and array and join for speed

	// append the sorted rows to the table
	for (i = 1; i < len; i++) {

		var cells = myarray[i - 1].cells

		string_array[k++] = "<tr>"

		for (var j = 0; j < cells.length; j++) {
			if (gecko)
			{
				string_array[k++] = GET_OUTER_HTML(cells[j]);
			}
			else
			{
				string_array[k++] = cells[j].outerHTML
			}
		}

	}

	string_array[k++] = "</table>"


	var s = string_array.join( "" );

	holder.innerHTML = s

	prevcol = col

	document.getElementById("wait").firstChild.nodeValue = "";
	document.getElementById("wait").innerHTML = "&nbsp;";

}

function flip (num) {

	// toggle ascending and descending order
	if (!asc) {
		return -1 * num
	}
	else {
		return num
	}
}

function getInnerTextHelper(el) {
	if (el.innerText) return el.innerText;	//Not needed but it is faster
	if (typeof el == "string") return el;
	if (typeof el == "undefined") { return el };
	var str = "";
	
	var cs = el.childNodes;
	var l = cs.length;
	for (var i = 0; i < l; i++) {
		switch (cs[i].nodeType) {
			case 1: //ELEMENT_NODE
				str += getInnerTextHelper(cs[i]);
				break;
			case 3:	//TEXT_NODE
				str += cs[i].nodeValue;
				break;
		}
	}
	return str;
}


var emptyElements = {
  HR: true, BR: true, IMG: true, INPUT: true
};

var specialElements = {
  TEXTAREA: true
};

function GET_OUTER_HTML (node) {

  var html = '';
  switch (node.nodeType) {
	case Node.ELEMENT_NODE:
	  html += '<';
	  html += node.nodeName;
	  if (!specialElements[node.nodeName]) {
		for (var a = 0; a < node.attributes.length; a++)
		  html += ' ' + node.attributes[a].nodeName.toUpperCase() +
				  '="' + node.attributes[a].nodeValue + '"';
		html += '>'; 
		if (!emptyElements[node.nodeName]) {
		  html += node.innerHTML;
		  html += '<\/' + node.nodeName + '>';
		}
	  }
	  else switch (node.nodeName) {
		case 'TEXTAREA':
		  for (var a = 0; a < node.attributes.length; a++)
			if (node.attributes[a].nodeName.toLowerCase() != 'value')
			  html += ' ' + node.attributes[a].nodeName.toUpperCase() +
					  '="' + node.attributes[a].nodeValue + '"';
			else 
			  var content = node.attributes[a].nodeValue;
		  html += '>'; 
		  html += content;
		  html += '<\/' + node.nodeName + '>';
		  break; 
	  }
	  break;
	case Node.TEXT_NODE:
	  html += node.nodeValue;
	  break;
	case Node.COMMENT_NODE:
	  html += '<!' + '--' + node.nodeValue + '--' + '>';
	  break;
  }
  return html;
}

