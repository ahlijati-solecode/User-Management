using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User_Management.Models.Entities.EntityFramework.Configurations
{
    public class MdRoleAccessConfiguration : IEntityTypeConfiguration<MdRoleAccess>
    {
        public void Configure(EntityTypeBuilder<MdRoleAccess> builder)
        {
            builder.ToTable("md_role_access");

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.ApprovedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("approved_date");

            builder.Property(e => e.ApprovedBy)
                .HasColumnType("character varying")
                .HasColumnName("approved_by");

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

            builder.Property(e => e.RoleId).HasColumnName("role_id");

            builder.HasOne(d => d.Role)
                .WithMany(p => p.MdUserAccesses)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ref_md_user_access_md_role");
        }
    }
}