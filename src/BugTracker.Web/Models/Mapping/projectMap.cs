using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class ProjectMap : EntityTypeConfiguration<Project>
    {
        public ProjectMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(80);

            this.Property(t => t.POP3UserName)
                .HasMaxLength(50);

            this.Property(t => t.POP3Password)
                .HasMaxLength(20);

            this.Property(t => t.POP3SourceEMail)
                .HasMaxLength(120);

            this.Property(t => t.CustomDropDownLabel1)
                .HasMaxLength(80);

            this.Property(t => t.CustomDropDownLabel2)
                .HasMaxLength(80);

            this.Property(t => t.CustomDropDownLabel3)
                .HasMaxLength(80);

            this.Property(t => t.CustomDropDownValue1)
                .HasMaxLength(800);

            this.Property(t => t.CustomDropDownValue2)
                .HasMaxLength(800);

            this.Property(t => t.CustomDropDownValue3)
                .HasMaxLength(800);

            this.Property(t => t.Description)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("projects");
            this.Property(t => t.Id).HasColumnName("pj_id");
            this.Property(t => t.Name).HasColumnName("pj_name");
            this.Property(t => t.Active).HasColumnName("pj_active");
            this.Property(t => t.DefaultUser).HasColumnName("pj_default_user");
            this.Property(t => t.AutoAssignToDefaultUser).HasColumnName("pj_auto_assign_default_user");
            this.Property(t => t.AutoSubscribeDefaultUser).HasColumnName("pj_auto_subscribe_default_user");
            this.Property(t => t.EnablePOP3).HasColumnName("pj_enable_pop3");
            this.Property(t => t.POP3UserName).HasColumnName("pj_pop3_username");
            this.Property(t => t.POP3Password).HasColumnName("pj_pop3_password");
            this.Property(t => t.POP3SourceEMail).HasColumnName("pj_pop3_email_from");
            this.Property(t => t.EnableCustomDropDown1).HasColumnName("pj_enable_custom_dropdown1");
            this.Property(t => t.EnableCustomDropDown2).HasColumnName("pj_enable_custom_dropdown2");
            this.Property(t => t.EnableCustomDropDown3).HasColumnName("pj_enable_custom_dropdown3");
            this.Property(t => t.CustomDropDownLabel1).HasColumnName("pj_custom_dropdown_label1");
            this.Property(t => t.CustomDropDownLabel2).HasColumnName("pj_custom_dropdown_label2");
            this.Property(t => t.CustomDropDownLabel3).HasColumnName("pj_custom_dropdown_label3");
            this.Property(t => t.CustomDropDownValue1).HasColumnName("pj_custom_dropdown_values1");
            this.Property(t => t.CustomDropDownValue2).HasColumnName("pj_custom_dropdown_values2");
            this.Property(t => t.CustomDropDownValue3).HasColumnName("pj_custom_dropdown_values3");
            this.Property(t => t.Default).HasColumnName("pj_default");
            this.Property(t => t.Description).HasColumnName("pj_description");
        }
    }
}
