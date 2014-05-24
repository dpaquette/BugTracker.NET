<%@ Page language="C#"%>
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	Util.do_not_cache(Response);
    

	DateTime dt = (DateTime) btnet.DbUtil.execute_scalar("select getdate()");

	Response.Write(dt.ToString("yyyyMMdd HH\\:mm\\:ss\\:fff"));

}

</script>