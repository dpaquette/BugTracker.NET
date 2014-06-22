<%@ Page language="C#" validateRequest="false"%>

<!--

Copyright 2006 Jochen Jonckheere
Distributed under the terms of the GNU General Public License

-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

    String sql;
    
    Security security;


    ///////////////////////////////////////////////////////////////////////
    void Page_Load(Object sender, EventArgs e)
    {
    	Util.do_not_cache(Response);
		
		security = new Security();
		security.check_security( HttpContext.Current, Security.ANY_USER_OK);

		titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
			+ "translate";

		string string_bp_id = Request["postid"];
		string string_bg_id = Request["bugid"];

		if (!IsPostBack)
		{

			if (string_bp_id != null && string_bp_id != "")
			{

				string_bp_id = Util.sanitize_integer(string_bp_id);

				sql = @"select bp_bug, bp_comment
						from bug_posts
						where bp_id = $id";

				sql = sql.Replace("$id", string_bp_id);

				DataRow dr = btnet.DbUtil.get_datarow(sql);

				string_bg_id = Convert.ToString((int) dr["bp_bug"]);
				object obj = dr["bp_comment"];
				if (dr["bp_comment"] != System.DBNull.Value)
					source.InnerText = obj.ToString();

			}
			else if (string_bg_id != null && string_bg_id != "")
			{

				string_bg_id = Util.sanitize_integer(string_bg_id);

				sql = @"select bg_short_desc
						from bugs
						where bg_id = $id";

				sql = sql.Replace("$id", string_bg_id);

				object obj = btnet.DbUtil.execute_scalar(sql);

				if (obj != System.DBNull.Value)
					source.InnerText = obj.ToString();
			}

            // added check for permission level - corey
            int permission_level = Bug.get_bug_permission_level(Convert.ToInt32(string_bg_id), security);
            if (permission_level == Security.PERMISSION_NONE) {
                Response.Write("You are not allowed to view this item");
                Response.End();
            }
			
			back_href.HRef = "edit_bug.aspx?id=" + string_bg_id;

			bugid.Value = string_bg_id;

			fill_translationmodes();
		}
		else
		{
			on_translate();
		}
	}


	///////////////////////////////////////////////////////////////////////
	void fill_translationmodes()
	{
		TranslationService ts = new TranslationService();

		foreach (TranslationMode tm in ts.GetAllTranslationModes())
		{
			mode.Items.Add(new ListItem(tm.VisualNameEN, tm.ObjectID));
		}

		mode.SelectedValue = "fr_nl";

		ts = null;
	}

    ///////////////////////////////////////////////////////////////////////
    void on_translate()
    {
		TranslationService ts = new TranslationService();
		TranslationMode tm = ts.GetTranslationModeByObjectID(mode.SelectedValue);

		string result = ts.Translate(tm, source.InnerText);

		result = result.Replace("\n", "<br>");

		dest.Text = result;

		pnl.Visible = true;

		tm = null;
		ts = null;
	}

</script>

<html>
<head>
    <title id="titl" runat="server">btnet send email</title>
    <link href="btnet.css" type="text/css" rel="StyleSheet" />
</head>
<body>
    <% security.write_menu(Response, Util.get_setting("PluralBugLabel","bugs")); %>
    <div class="align">
        <table border="0">
            <tbody>
                <tr>
                    <td>
                        <a id="back_href" href="" runat="server">back to <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %></a>
                        <form enctype="multipart/form-data" runat="server">
							<table border="0" class="frm">
                                <tbody>
                                    <tr>
                                        <td class="lbl">
                                            Translation mode: <asp:DropDownList id="mode" runat="server" /></td>
                                    </tr>
                                        <td class="lbl">
                                            Source text:</td>
									</tr>
									<tr>
                                        <td>
                                            <textarea class="txt" id="source" rows="15" cols="72" runat="server"></textarea>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="middle">
                                            <input class="btn" id="sub" type="submit" value="Translate" runat="server" />
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
							<br>
							<asp:Panel id="pnl" runat="server" visible="false">
								<table class="cmt">
									<tbody>
										<tr>
											<td>
												<span class=pst>translated from <%= mode.SelectedItem %> on <%= DateTime.Now %></span>
												<br>
												<br>
												<asp:Label id="dest" runat="server" cssclass="cmt_text" />										</td>
										</tr>
									</tbody>
								</table>
							</asp:Panel>
                             <input id="bugid" type="hidden" runat="server" />
                        </form></td>
                </tr>
            </tbody>
        </table>

    </div>
</body>
</html>