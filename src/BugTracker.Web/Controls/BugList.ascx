<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BugList.ascx.cs" Inherits="btnet.Controls.BugList" %>
<div id="popup" class="buglist_popup"></div>
<table id="bug-table" class="table table-striped table-bordered">
    <thead>
        <!--Main header-->
        <tr>
            <%foreach (string columnName in GetVisibleColumns())
              {%>
            <th><%= GetColumnDisplayName(columnName) %></th>

            <%}%>
        </tr>
        <!--Filter row-->
        <tr class="filter-row">
            <% foreach (string columnName in GetVisibleColumns())
               {%>
            <th><% if (IsFilterableColumn(columnName))
                   {%>
                <select class="table-filter" data-column-name="<%=columnName %>">
                    <% foreach (var option in GetFilterValues(columnName))
                       {%>
                    <option value="<%=option.Value %>"><%=option.Text %></option>
                    <%}%>
                </select>

                <%} %></th>

            <%}%>
        </tr>
    </thead>
</table>
<script type="text/javascript">
    
    $(function() {
        var getFlagColorClass = function(flagId) {
            var color = "wht";
            if (flagId == 1) color = "red";
            else if (flagId == 2) color = "grn";
            return color;
        }

        // NOTE: This could be simplified by returning 2 separate columns
        // Vote count is provided as 10,000 * vote count, and
        // last number indicates yes/no for the current user
        // eg) 30,001 means 3 votes, 1 from this user.
        //     120,000 means 12 votes, 0 from this user.
        // The purpose of this is so that we can sort the column by votes,
        // but still color it by THIS user's vote.
        var getVoteCellClass = function(voteValue) {
            var thisUsersVote = 0;
            var magicNumber = 10000;
            thisUsersVote = voteValue % magicNumber;

            var voteClass = "novote";
            if (thisUsersVote == 1)
            {
                voteClass = "yesvote";
            }
            return voteClass;
        }

        var getVoteCount = function(voteValue) {            
            var magicNumber = 10000;                
            var voteCount = Math.floor(voteValue / magicNumber); 
            return voteCount;
        }

        var renderDate = function(data, type, row) {
            var dateValue = new Date(data);
            return moment(dateValue).format("YYYY-MM-DD h:mm A");
        };

        var renderId = function(data, type, row) {
            return '<span style="display: block; background-color:' + row['$COLOR'] + '">' + data + '</span>';
        };

        var renderDescription = function(data, type, row) {
            return '<a onmouseover="on_mouse_over(this)" onmouseout="on_mouse_out()" ' +
                'href="edit_bug.aspx?id=' + row['id'] + '">' + data + '</a>';
        };

        var renderFlag = function(data, type, row) {
            return ' <span title="click to flag/unflag this for yourself" class="' + getFlagColorClass(data) + '" ' +
                'onclick="flag(this, \'' + row['id'] + '\')">&nbsp;</span>';
        };

        var renderSeen = function(data, type, row) {
            var className = data == 0 ? 'new' : 'old';
            return '<span title="click to toggle new/old" class="' + className + '" ' +
                ' onclick="seen(this, \'' + row['id'] + '\')">&nbsp;</span>';
        };

        var renderVote = function(data, type, row) {
            var className = getVoteCellClass(data);
            var voteCount = getVoteCount(data);

            return '<span title="click to toggle your vote" class="' + className + '" ' +
                'onclick="vote(this, \'' + row['id'] + '\')">' + voteCount + '</span>';
        };

        var getSpecialRenderFunction = function(columnName) {
            //Some columns are rendered in a special way...
            var result = null;
            switch (columnName) {
            case "id":
                result = renderId;
                break;
            case "desc":
                result = renderDescription;
                break;
            case "$FLAG":
                result = renderFlag;
                break;
            case "$SEEN":
                result = renderSeen;
                break;
            case "$VOTE":
                result = renderVote;
            default:
                if (columnName.toLowerCase().match(" on$") == " on") {
                    result = renderDate;
                }
            }
            return result;
        }

        var columns = [];
        var columnNames = [<%= string.Join(", ", GetVisibleColumns().Select(c => string.Format("'{0}'", c)).ToArray()) %>];
        var queryId = <%=SelectedQuery != null ? SelectedQuery.Id : 0%>;
        for (var i = 0; i < columnNames.length; i++) {
            var columnName = columnNames[i];
            var column = { data:  columnName};
            var renderFunction = getSpecialRenderFunction(columnName);
            if (renderFunction != null) {
                column.render = renderFunction;
            }
            columns.push(column);
        }
                   
        var getCurrentFilters = function() {
            var filters = [];
            $("select.table-filter").each(function() {
                var currentFilter = $(this);
                var selectedValue = currentFilter.val();
                if (selectedValue) {
                    var selectedColumnName = currentFilter.attr('data-column-name');
                    filters.push({Column: selectedColumnName, Value: selectedValue});
                }
            });
            return filters;
        };

        var bugsTable = $("#bug-table").dataTable({
            serverSide: true,
            processing: true,
            paging: true,
            "columns": columns,            
            orderCellsTop : true,
            orderMulti: false,
            searching: false,
            ajax: function(data, callback) {
                var sortColumnName = data.columns[data.order[0].column].data;
                var urlParameters = {
                    queryId : queryId,
                    sortBy : sortColumnName,
                    sortOrder : data.order[0].dir,
                    start: data.start,
                    length: data.length,
                    filters: getCurrentFilters()
                }
                BugList.setQueryParams(urlParameters);
                var queryUrl = "api/BugQuery?" + $.param(urlParameters);
                
                $.get(queryUrl).done(function(d) {
                    callback(d);
                }).fail(function() {
                    callback();

                });
            }
        }).api();
        $("select.table-filter").on("change", function() {
            bugsTable.draw();
        });
    });
</script>