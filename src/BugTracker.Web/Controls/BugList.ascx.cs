using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using btnet.Models;

namespace btnet.Controls
{
    public partial class BugList : System.Web.UI.UserControl
    {
        public Query SelectedQuery { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private readonly IDictionary<string, string> _columnNameOverrides = new Dictionary<string, string>
        {
            {"$FLAG", "flag"},
            {"$SEEN", "new"},
            {"$VOTE", "votes"},
            {"search_desc", "desc"},
            {"search_text", "context"},
            {"search_source", "text source"}            
        };

        protected string GetColumnDisplayName(string columnName)
        {
            return _columnNameOverrides.ContainsKey(columnName) ? _columnNameOverrides[columnName] : columnName;
        }

        protected IEnumerable<string> GetVisibleColumns()
        {
            return SelectedQuery == null ? new string[] {} : SelectedQuery.VisibleColumnNames;
        } 

        private readonly string[] _filterableColumns = {"$FLAG", "$SEEN", "project", "organization", "category", "priority", "status", "reported by", "assigned to" };

        protected bool IsFilterableColumn(string columnName)
        {
            return _filterableColumns.Contains(columnName);
        }

        public IEnumerable<SelectListItem> GetFilterValues(string columnName)
        {
            var result = new List<SelectListItem>();            
            result.Add(new SelectListItem{Value = "", Text = "[no filter]"});
            //TODO: Room for improvement here. This if statement is a bad code smell. 
            //      Consider creating a lookup provider that we can ask for specific lookup types from
            if (columnName == "$FLAG")
            {
                result.Add(new SelectListItem{Value = "0", Text = "None"});
                result.Add(new SelectListItem{Value = "1", Text = "Red"});
                result.Add(new SelectListItem{Value = "2", Text = "Green"});                
            }
            else if (columnName == "$SEEN")
            {
                result.Add(new SelectListItem{Value = "0", Text = "Yes"});
                result.Add(new SelectListItem{Value = "1", Text = "No"});
            }
            else if (columnName == "project")
            {
                using (Context context = new Context())
                {
                    var projectItems = context.Projects.OrderBy(p => p.Name)
                        .Select(p => new SelectListItem
                        {
                            Value = p.Name,
                            Text = p.Name
                        });
                    result.AddRange(projectItems);
                }
            }
            else if (columnName == "organization")
            {
                using (Context context = new Context())
                {
                    var organizationItems = context.Organizations.OrderBy(p => p.Name)
                        .Select(p => new SelectListItem
                        {
                            Value = p.Name,
                            Text = p.Name
                        });
                    result.AddRange(organizationItems);
                }
            }
            else if (columnName == "category")
            {
                using (Context context = new Context())
                {
                    var categoryItems = context.Categories.OrderBy(p => p.Name)
                        .Select(p => new SelectListItem
                        {
                            Value = p.Name,
                            Text = p.Name
                        });
                    result.AddRange(categoryItems);
                }
            }
            else if (columnName == "priority")
            {
                using (Context context = new Context())
                {
                    var priorityItems = context.Priorities.OrderBy(p => p.Name)
                        .Select(p => new SelectListItem
                        {
                            Value = p.Name,
                            Text = p.Name
                        });
                    result.AddRange(priorityItems);
                }
            }
            else if (columnName == "status")
            {
                using (Context context = new Context())
                {
                    var statusItems = context.Statuses.OrderBy(p => p.Name)
                        .Select(p => new SelectListItem
                        {
                            Value = p.Name,
                            Text = p.Name
                        });
                    result.AddRange(statusItems);
                }
            }
            else if (columnName == "assigned to" || columnName == "reported by")
            {
                using (Context context = new Context())
                {
                    var userItems = context.Users.OrderBy(p => p.UserName)
                        .Select(p => new SelectListItem
                        {
                            Value = p.UserName,
                            Text = p.UserName
                        });
                    result.AddRange(userItems);
                }
            }
            return result;
        }
    }
}