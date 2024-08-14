using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User_Management.Models.Entities.EntityFramework.Configurations
{
    public class LgRoleUserConfiguration : IEntityTypeConfiguration<LgRoleUser>
    {
        public void Configure(EntityTypeBuilder<LgRoleUser> builder)
        {
            builder.ToTable("lg_role_user");

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Activity)
                .HasColumnType("character varying")
                .HasColumnName("activity");

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

            builder.Property(e => e.Note)
                .HasColumnType("character varying")
                .HasColumnName("note");

            builder.Property(e => e.ParentId).HasColumnName("parent_id");

            builder.Property(e => e.RoleId).HasColumnName("role_id");

            builder.HasOne(d => d.Parent)
                .WithMany(p => p.LgRoleUsers)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ref_to_parent");

            builder.HasOne(d => d.Role)
                .WithMany(p => p.LgRoleUsers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ref_lg_role_user_md_role");
        }
    }
}