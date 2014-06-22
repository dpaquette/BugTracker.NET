<link rel="shortcut icon" href="favicon.ico">
<script language="C#" runat="server">

string sql;
System.Data.DataView dv;
System.Data.DataSet ds_custom_cols = null;

///////////////////////////////////////////////////////////////////////
void display_bugs(bool show_checkboxes)
{
	btnet.BugList.display_bugs (
		show_checkboxes,
		dv,
		Response,
		security,
		new_page.Value,
		IsPostBack,
		ds_custom_cols,
		filter.Value);
}

void call_sort_and_filter_buglist_dataview()
{
	string filter_val = filter.Value;
	string sort_val = sort.Value;
	string prev_sort_val = prev_sort.Value;
	string prev_dir_val = prev_dir.Value;


	btnet.BugList.sort_and_filter_buglist_dataview(dv, IsPostBack,
		actn.Value,
		ref filter_val,
		ref sort_val,
		ref prev_sort_val,
		ref prev_dir_val);

	filter.Value = filter_val;
	sort.Value = sort_val;
	prev_sort.Value = prev_sort_val;
	prev_dir.Value = prev_dir_val;

}

</script>