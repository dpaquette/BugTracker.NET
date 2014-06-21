<%@ Page language="C#"%>

<script runat="server">

void Page_Load(Object sender, EventArgs e)
{

	Response.Write("Hello<br>");

	Response.Write(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
	
	Response.Write("<br>");

	System.Globalization.CultureInfo ci = 
	new System.Globalization.CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
	
	Response.Write(ci.NumberFormat.NumberDecimalSeparator);
	
	ci = 
	
	new System.Globalization.CultureInfo("de-DE");
	
	Response.Write(ci.NumberFormat.NumberDecimalSeparator);
		
	

}


</script>

