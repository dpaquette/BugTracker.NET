using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Active { get; set; }
        public Nullable<int> DefaultUser { get; set; }
        public Nullable<int> AutoAssignToDefaultUser { get; set; }
        public Nullable<int> AutoSubscribeDefaultUser { get; set; }
        public Nullable<int> EnablePOP3 { get; set; }
        public string POP3UserName { get; set; }
        public string POP3Password { get; set; }
        public string POP3SourceEMail { get; set; }
        public int EnableCustomDropDown1 { get; set; }
        public int EnableCustomDropDown2 { get; set; }
        public int EnableCustomDropDown3 { get; set; }
        public string CustomDropDownLabel1 { get; set; }
        public string CustomDropDownLabel2 { get; set; }
        public string CustomDropDownLabel3 { get; set; }
        public string CustomDropDownValue1 { get; set; }
        public string CustomDropDownValue2 { get; set; }
        public string CustomDropDownValue3 { get; set; }
        public int Default { get; set; }
        public string Description { get; set; }
    }
}
