using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class projectMap : EntityTypeConfiguration<project>
    {
        public projectMap()
        {
            // Primary Key
            this.HasKey(t => t.pj_id);

            // Properties
            this.Property(t => t.pj_name)
                .IsRequired()
                .HasMaxLength(80);

            this.Property(t => t.pj_pop3_username)
                .HasMaxLength(50);

            this.Property(t => t.pj_pop3_password)
                .HasMaxLength(20);

            this.Property(t => t.pj_pop3_email_from)
                .HasMaxLength(120);

            this.Property(t => t.pj_custom_dropdown_label1)
                .HasMaxLength(80);

            this.Property(t => t.pj_custom_dropdown_label2)
                .HasMaxLength(80);

            this.Property(t => t.pj_custom_dropdown_label3)
                .HasMaxLength(80);

            this.Property(t => t.pj_custom_dropdown_values1)
                .HasMaxLength(800);

            this.Property(t => t.pj_custom_dropdown_values2)
                .HasMaxLength(800);

            this.Property(t => t.pj_custom_dropdown_values3)
                .HasMaxLength(800);

            this.Property(t => t.pj_description)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("projects");
            this.Property(t => t.pj_id).HasColumnName("pj_id");
            this.Property(t => t.pj_name).HasColumnName("pj_name");
            this.Property(t => t.pj_active).HasColumnName("pj_active");
            this.Property(t => t.pj_default_user).HasColumnName("pj_default_user");
            this.Property(t => t.pj_auto_assign_default_user).HasColumnName("pj_auto_assign_default_user");
            this.Property(t => t.pj_auto_subscribe_default_user).HasColumnName("pj_auto_subscribe_default_user");
            this.Property(t => t.pj_enable_pop3).HasColumnName("pj_enable_pop3");
            this.Property(t => t.pj_pop3_username).HasColumnName("pj_pop3_username");
            this.Property(t => t.pj_pop3_password).HasColumnName("pj_pop3_password");
            this.Property(t => t.pj_pop3_email_from).HasColumnName("pj_pop3_email_from");
            this.Property(t => t.pj_enable_custom_dropdown1).HasColumnName("pj_enable_custom_dropdown1");
            this.Property(t => t.pj_enable_custom_dropdown2).HasColumnName("pj_enable_custom_dropdown2");
            this.Property(t => t.pj_enable_custom_dropdown3).HasColumnName("pj_enable_custom_dropdown3");
            this.Property(t => t.pj_custom_dropdown_label1).HasColumnName("pj_custom_dropdown_label1");
            this.Property(t => t.pj_custom_dropdown_label2).HasColumnName("pj_custom_dropdown_label2");
            this.Property(t => t.pj_custom_dropdown_label3).HasColumnName("pj_custom_dropdown_label3");
            this.Property(t => t.pj_custom_dropdown_values1).HasColumnName("pj_custom_dropdown_values1");
            this.Property(t => t.pj_custom_dropdown_values2).HasColumnName("pj_custom_dropdown_values2");
            this.Property(t => t.pj_custom_dropdown_values3).HasColumnName("pj_custom_dropdown_values3");
            this.Property(t => t.pj_default).HasColumnName("pj_default");
            this.Property(t => t.pj_description).HasColumnName("pj_description");
        }
    }
}
