btnet = {
	ext: true,
	div: null,
	prefs: null,
	prevX: -1,
	prevY: -1,
	canvasX: 14,
	canvasY: 34,
	canvas: null,
	prev_image: null,
	original_image: null,
	drawing: false,
	adjustedW: -1,
	adjustedH: -1,
	drawing_mode: 'arrow',
	doc: document,

	html: " \
<table><tr> \
<td valign=top nowrap> \
	<div id=btnet_div_header> \
<form id='btnet_donate_form' action='https://www.paypal.com/cgi-bin/webscr' method='post'> \
<input id='btnet_donate' type=submit value='Is BugTracker.NET helping you or your company? Please Donate!'> \
<input type='hidden' name='cmd' value='_s-xclick'> \
<input type='hidden' name='encrypted' value='-----BEGIN PKCS7-----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-----END PKCS7-----\
'></form>\
	<span id=btnet_span_tool>Draw:&nbsp; \
		<select id=btnet_select_tool> \
		<option selected value='arrow'>Red arrow</option> \
		<option value='line'>Red line</option> \
		</select> \
	</span> \
	&nbsp;&nbsp;<input type=submit id=btnet_clear value='Clear'> \
	</div> \
	<div id=btnet_div_middle></div> \
<td valign=top nowrap> \
	<div id=btnet_div_side> \
	<span>Description:<br></span> \
	<textarea id=btnet_desc name=btnet_desc rows=3 maxlength=200></textarea> \
	<br><br> \
	<input type=submit id=btnet_send value='Send'> \
	<br><br> \
	<input type=radio id=btnet_radio_new name=btnet_new_or_existing value=new checked>Create new bug</input>\
	<br> \
	<input type=radio id=btnet_radio_existing name=btnet_new_or_existing vaue=existing>Update existing bug </input>\
	<br> \
	<span>Bug ID#&nbsp;</span><input type=text id=btnet_bugid name=btnet_bugid size=8 value='0' disabled> \
	</div> \
	<input type=submit id=btnet_configure value='Configure'> \
	<br> \
	<input type=submit id=btnet_close value='Close'> \
	<div id=btnet_version1><a style='font-size:8pt;' id=btnet_version2 href=http://ifdefined.com/bugtrackernet.html>BugTracker.NET Screen Capture</a></div> \
	<div id=btnet_version3>Version 1.1 2010-11-28</div> \
</table>",


	clear: function() {
		var ctx = btnet.canvas.getContext('2d')
		ctx.drawImage(btnet.original_image, 0,0)
		btnet.prev_image.src = btnet.original_image.src
	},
	
	send: function() {
		var url = btnet.prefs.getCharPref("url")
		if (url == "")
		{
			alert('You need to enter your configuration before you can post screenshots')
			btnet.configure()
		}

		var url = btnet.prefs.getCharPref("url")
		if (url == "")
		{
			return
		}

		btnet.post()
	},

	close: function() {
		var body = btnet.doc.getElementsByTagName('body')[0];
		var div = btnet.doc.getElementById("btnet_div")
		body.removeChild(div)
	},
	
	new_or_existing_onclick: function() {
		var radio_new = btnet.doc.getElementById("btnet_radio_new")
		var bugid = btnet.doc.getElementById("btnet_bugid")
		bugid.disabled = radio_new.checked	
	},

	wire_up_events: function() {

		btnet.doc.getElementById("btnet_send")
			.addEventListener("click", btnet.send, false)

		btnet.doc.getElementById("btnet_configure").
			addEventListener("click", btnet.configure, false)

		btnet.doc.getElementById("btnet_close")
			.addEventListener("click", btnet.close, false)

		btnet.doc.getElementById("btnet_radio_new")
			.addEventListener("click", btnet.new_or_existing_onclick, false)

		btnet.doc.getElementById("btnet_radio_existing")
			.addEventListener("click", btnet.new_or_existing_onclick, false)

		btnet.doc.getElementById("btnet_clear")
			.addEventListener("click", btnet.clear, false)


		btnet.canvas.addEventListener("mousedown", btnet.mousedown, false);	
		btnet.canvas.addEventListener("mouseup", btnet.mouseup, false);	
		btnet.canvas.addEventListener("mousemove", btnet.mousemove, false);	

	},
	
	mousedown: function(event) {
	
		btnet.drawing = true
		btnet.prevX = event.pageX - btnet.canvasX
		btnet.prevY = event.pageY - btnet.canvasY
		btnet.tool = btnet.doc.getElementById('btnet_select_tool').value
	},
	
	mousemove: function(event) {
	
		if (btnet.drawing == true)
		{
			if (btnet.tool == 'arrow')
			{
				var ctx = btnet.canvas.getContext('2d')
				ctx.drawImage(btnet.prev_image,0,0)
				btnet.draw_arrow(event)
			}
			else
			{
				btnet.draw_line(event)
			}
		}
	
	},

	mouseup: function(event) {

		btnet.drawing = false
		if (btnet.tool == 'arrow')
		{
			var ctx = btnet.canvas.getContext('2d')
			ctx.drawImage(btnet.prev_image,0,0)
			btnet.draw_arrow(event)
		}
		else
		{
			btnet.draw_line(event)
		}
		btnet.prev_image.src = btnet.canvas.toDataURL("image/png")
	},

	draw_line: function(event) {

		var x = event.pageX - btnet.canvasX
		var y = event.pageY - btnet.canvasY

		var ctx= btnet.canvas.getContext('2d');
  
		ctx.strokeStyle = "red"
		ctx.lineWidth = 2
		ctx.beginPath()
		ctx.moveTo(btnet.prevX,btnet.prevY)
		ctx.lineTo(x,y)
		ctx.stroke()
		btnet.prevX = x
		btnet.prevY = y
	},

	draw_arrow: function(event) {
	
		var x = event.pageX - btnet.canvasX
		var y = event.pageY - btnet.canvasY

		var ctx= btnet.canvas.getContext('2d');
  
		// pythagoras
		var lenX = x - btnet.prevX
		var lenY = y - btnet.prevY
		var len = Math.sqrt((lenX*lenX) + (lenY*lenY))

		var angle = Math.atan2(lenY, lenX)

		ctx.save();
		ctx.strokeStyle = "red"
		ctx.lineWidth = 2
		ctx.translate(btnet.prevX, btnet.prevY);
		ctx.rotate(angle) 
		ctx.beginPath()

		//draw line with arrow		
		var arrow_size = 4
		ctx.moveTo(0,0)
		ctx.lineTo(len,0)
		ctx.lineTo(len - arrow_size, arrow_size)
		ctx.moveTo(len,0)
		ctx.lineTo(len - arrow_size, -arrow_size)
		ctx.stroke()
		ctx.restore();		
	
	},
	

	inject: function() {
	
		if (btnet.ext)
		{
			cssRef = content.document.createElement('LINK');
			cssRef.rel = 'stylesheet';
			cssRef.href = 'chrome://btnet/content/btnet.css';
			cssRef.type = 'text/css';
			content.document.getElementsByTagName('head')[0].appendChild(cssRef);		
		
			btnet.doc = window.content.document
		}
		else
		{
			btnet.doc = document
		}
		
		doc = btnet.doc


		if (doc.getElementById("btnet_div") != null)
		{
			return
		}
	


		// create a div
		btnet.div = doc.createElement("div");
		btnet.div.id = "btnet_div" 
		btnet.div.style.display = "none"


		// create a canvas
		btnet.canvas = doc.createElement("canvas");
		var canvas = btnet.canvas
		var ctx = canvas.getContext("2d")
		canvas.id = "btnet_canvas"


		// put everything in this div
		btnet.div.innerHTML = btnet.html

		var captureW = 0
		var captureH = 0
		
		if(window.content.document.compatMode == "CSS1Compat") {
			captureW = window.content.document.documentElement.clientWidth;
			captureH = window.content.document.documentElement.clientHeight;
		}
		else {
			captureW = window.content.document.body.clientWidth;
			captureH = window.content.document.body.clientHeight;
		}

		var imageSize = btnet.prefs.getIntPref("imageSize")

		btnet.adjustedW = imageSize
		btnet.adjustedH = imageSize

		var scale = 1.0
	
		if (captureW >= captureH)
		{
			if (captureW > imageSize)
			{
				scale = imageSize/captureW
				btnet.adjustedH = Math.floor(captureH * scale)
			}
			else
			{
				btnet.adjustedW = captureW
				btnet.adjustedH = captureH
			}
		}
		else
		{
			if (captureH > imageSize)
			{
				scale = imageSize/captureH
				btnet.adjustedW = Math.floor(captureW * scale)
			}
			else
			{
				btnet.adjustedW = captureW
				btnet.adjustedH = captureH
			}
		}


		// resize canvas to match capture
		canvas.width = btnet.adjustedW
		canvas.height = btnet.adjustedH
		canvas.style.width = btnet.adjustedW
		canvas.style.height = btnet.adjustedH

	
		ctx = canvas.getContext("2d"); 
		ctx.save()			
		ctx.scale(scale, scale)

		ctx.drawWindow(
			window.content,
			window.content.scrollX,
			window.content.scrollY, 
			captureW, captureH,
			"rgb(255,255,255)")

		ctx.restore()


		// inject
		var body = doc.getElementsByTagName('body')[0];
		body.appendChild(btnet.div);
		btnet.div.style.display = "block" 	

		btnet.div_middle = btnet.doc.getElementById("btnet_div_middle")
		btnet.div_middle.appendChild(btnet.canvas)

        btnet.prev_image = new Image()
        btnet.original_image = new Image()
        btnet.prev_image.src = canvas.toDataURL("image/png")
        btnet.original_image.src = btnet.prev_image.src

		btnet.wire_up_events()

	},
	
	on_load: function() {

		// init prefs
		btnet.prefs = Components.classes["@mozilla.org/preferences-service;1"]
			.getService(Components.interfaces.nsIPrefBranch)
			.getBranch("extensions.btnet.")

	},

	post: function() {

		var loginInfo = btnet.get_login_info_object()
		btnet.get_password(loginInfo)
		var username = loginInfo.username
		var password = loginInfo.password
		var projectid = btnet.prefs.getIntPref("projectNumber")

		var desc = btnet.doc.getElementById("btnet_desc").value
		var radio_existing = btnet.doc.getElementById("btnet_radio_existing")
		var	bugid = 0
		
		if (radio_existing.checked)
		{
			bugid =  btnet.doc.getElementById("btnet_bugid").value
		}

        base64_image  = btnet.canvas.toDataURL("image/jpeg", "")
		pos = base64_image.indexOf(",")
		base64_image = base64_image.substr(pos+1)

		var params = "username=" + username
			+ "&password=" + encodeURIComponent(password) 
			+ "&short_desc=" + encodeURIComponent(desc)
			+ "&bugid=" + encodeURIComponent(bugid)
			+ "&projectid=" + projectid
			+ "&attachment_filename=webpage.jpg"
			+ "&attachment_content_type=image/jpeg"
			+ "&attachment_desc=mydesc"
			+ "&attachment=" + encodeURIComponent(base64_image)

		//alert(btnet.base64_image.length)
		var url = btnet.prefs.getCharPref("url")


		var req = new XMLHttpRequest();

		req.open("POST", url + "/insert_bug.aspx", true)

		req.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
		req.setRequestHeader("Content-length", params.length);
		req.setRequestHeader("Connection", "close");


		req.onreadystatechange = function() {btnet.onreadystatechange(req)}
		req.send(params)
	},


	onreadystatechange: function(req) {

		if(req.readyState == 4)
		{
			if (req.responseText.substr(0,3) == 'OK:')
			{
				bugid = req.responseText.substr(3)
				var answer = confirm("Posted screenshot to Bug ID# " + bugid + "\n\nGo to the BugTracker.NET website?")
				
				if (answer == true)
				{
					var url = btnet.prefs.getCharPref("url") + '/edit_bug.aspx?id=' + bugid
					window.content.location = url
				}
			}
			else
			{
				alert('ERROR\n\nhttp status: ' + req.status + '\n' + req.responseText)				
			}
		}
	},

	configure: function() { 
		window.openDialog("chrome://btnet/content/prefs2.xul","BugTracker.NET Configuration","centerscreen,chrome,dialog,modal", 
			btnet);
	},


	get_prefs: function(dlg) {
		var loginInfo = btnet.get_login_info_object()
		btnet.get_password(loginInfo)
		
		dlg.getElementById('username').value = loginInfo.username
		dlg.getElementById('password').value = loginInfo.password
		dlg.getElementById('url').value =  btnet.prefs.getCharPref("url")

		dlg.getElementById('projectNumber').value = btnet.prefs.getIntPref("projectNumber")
		dlg.getElementById('imageSize').value = btnet.prefs.getIntPref("imageSize")
	},

	save_prefs: function(dlg) {

		btnet.prefs.setCharPref(
			"url",
			dlg.getElementById('url').value)

		btnet.prefs.setIntPref(
			"projectNumber",
			dlg.getElementById('projectNumber').value)
			
		btnet.prefs.setIntPref(
			"imageSize",
			dlg.getElementById('imageSize').value)

		btnet.put_password(
			dlg.getElementById('username').value,
			dlg.getElementById('password').value)

	},
		
	get_password: function(loginInfo) {
	
		var passwordManager = Components.classes["@mozilla.org/login-manager;1"].
			getService(Components.interfaces.nsILoginManager)  
	
		var logins = passwordManager.findLogins({}, loginInfo.hostname, null, loginInfo.httprealm);   
		
		for (var i = 0; i < logins.length; i++) {   
			loginInfo.username = logins[i].username
			loginInfo.password = logins[i].password
			return true
		}
		
		return false
	
	},

	get_login_info_object: function() {
		
		var nsLoginInfo = new Components.Constructor("@mozilla.org/login-manager/loginInfo;1",
			Components.interfaces.nsILoginInfo,
			"init");   
   
		var loginInfo = new nsLoginInfo(
				'chrome://btnet',
				null, 
				'btnet password',
				"", "", "", "")

		return loginInfo	
	},
	
	
	put_password: function(username, password) {
		
		var passwordManager = Components.classes["@mozilla.org/login-manager;1"].
			getService(Components.interfaces.nsILoginManager)  
     
		var loginInfo = btnet.get_login_info_object()
		
		btnet.get_password(loginInfo)
		
		if (loginInfo.username != "")
		{
			passwordManager.removeLogin(loginInfo)			
		}
		
		loginInfo.username = username
		loginInfo.password = password
		
		passwordManager.addLogin(loginInfo)
		
	 },
} 


window.addEventListener("load", btnet.on_load, false);


