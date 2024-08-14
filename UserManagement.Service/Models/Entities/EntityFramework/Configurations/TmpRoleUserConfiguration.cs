using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User_Management.Models.Entities.EntityFramework.Configurations
{
    public class TmpRoleUserConfiguration : IEntityTypeConfiguration<TmpRoleUser>
    {
        public void Configure(EntityTypeBuilder<TmpRoleUser> builder)
        {
            builder.ToTable("tmp_role_user");

            builder.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");

            builder.Property(e => e.ApprovedBy)
                .HasColumnType("character varying")
                .HasColumnName("approved_by");

            builder.Property(e => e.ApprovedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("approved_date");

            builder.Property(e => e.CreatedBy)
                .HasColumnType("character varying")
                .HasColumnName("created_by");

            builder.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");

            builder.Property(e => e.DeletedBy)
                .HasColumnType("character varying")
                .HasColumnName("deleted_by");

            builder.Property(e => e.DeletedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_date");

            builder.Property(e => e.IsActive).HasColumnName("is_active");

            builder.Property(e => e.ModifiedBy)
                .HasColumnType("character varying")
                .HasColumnName("modified_by");

            builder.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_date");

            builder.Property(e => e.ParentId).HasColumnName("parent_id");

            builder.Property(e => e.RoleId).HasColumnName("role_id");

            builder.HasOne(d => d.Parent)
                .WithMany(p => p.TmpRoleUsers)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("ref_to_parent_role_user");

            builder.HasOne(d => d.Role)
                .WithMany(p => p.TmpRoleUsers)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("ref_tmp_role_user_md_role");
        }
    }
}