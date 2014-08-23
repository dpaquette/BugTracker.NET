<%@ Page language="C#" CodeBehind="register.aspx.cs" Inherits="btnet.register" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->


<html>
<head>
<title id="titl" runat="server">btnet forgot password</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body onload="document.forms[0].username.focus()">
<table border=0><tr>

<%

Response.Write (Application["custom_logo"]);

%>

</table>


<div align="center">
<table border=0><tr><td>
<form class=frm runat="server">
	<table border=0>

	<tr>
	<td class=lbl>Username:</td>
	<td><input runat="server" type=text class=txt id="username" maxlength=20 size=20></td>
	<td runat="server" class=err id="username_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Email:</td>
	<td><input runat="server" type=text class=txt id="email" maxlength=40 size=40></td>
	<td runat="server" class=err id="email_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Password:</td>
	<td><input runat="server" autocomplete=off type=password class=txt id="password" maxlength=20 size=20></td>
	<td runat="server" class=err id="password_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Confirm Password:</td>
	<td><input runat="server" autocomplete=off type=password class=txt id="confirm" maxlength=20 size=20></td>
	<td runat="server" class=err id="confirm_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>First Name:</td>
	<td><input runat="server" type=text class=txt id="firstname" maxlength=20 size=20></td>
	<td runat="server" class=err id="firstname_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Last Name:</td>
	<td><input runat="server" type=text class=txt id="lastname" maxlength=20 size=20></td>
	<td runat="server" class=err id="lastname_err">&nbsp;</td>
	</tr>

	<tr><td colspan=2 align=left>
	<span runat="server" class=err id="msg">&nbsp;</span>
	</td></tr>

	<tr><td colspan=2 align=center>
	<input class=btn type=submit value="Submit Registration" runat="server">
	</td></tr>

	</table>
</form>

<a href="default.aspx">Return to login page</a>

</td></tr></table>

</div>
</body>
</html>
