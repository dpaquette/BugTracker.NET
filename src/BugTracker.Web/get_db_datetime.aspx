<%@ Page language="C#" CodeBehind="get_db_datetime.aspx.cs" Inherits="btnet.get_db_datetime" AutoEventWireup="True" %>
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	Util.do_not_cache(Response);
    

	DateTime dt = (DateTime) btnet.DbUtil.execute_scalar(new SQLString("select getdate()"));

	Response.Write(dt.ToString("yyyyMMdd HH\\:mm\\:ss\\:fff"));

}

</script>
