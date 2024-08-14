using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User_Management.Models.Entities.EntityFramework.Configurations
{
    public class MdRoleAccessRefConfiguration : IEntityTypeConfiguration<MdRoleAccessRef>
    {
        public void Configure(EntityTypeBuilder<MdRoleAccessRef> builder)
        {
            builder.ToTable("md_role_access_ref");

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("nextval('md_ref_user_access_id_seq'::regclass)");

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");

            builder.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");

            builder.Property(e => e.DeletedBy)
                .HasMaxLength(50)
                .HasColumnName("deleted_by");

            builder.Property(e => e.DeletedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_date");

            builder.Property(e => e.IsCreate).HasColumnName("is_create");

            builder.Property(e => e.IsDelete).HasColumnName("is_delete");

            builder.Property(e => e.IsEdit).HasColumnName("is_edit");

            builder.Property(e => e.IsView).HasColumnName("is_view");

            builder.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .HasColumnName("modified_by");

            builder.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_date");

            builder.Property(e => e.RefMenuId).HasColumnName("ref_menu_id");

            builder.Property(e => e.RefUserAccess).HasColumnName("ref_user_access");

            builder.HasOne(d => d.RefMenu)
                .WithMany(p => p.MdUserAccessRefs)
                .HasForeignKey(d => d.RefMenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ref_user_access_menu");

            builder.HasOne(d => d.RefUserAccessNavigation)
                .WithMany(p => p.MdUserAccessRefs)
                .HasForeignKey(d => d.RefUserAccess)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ref_user_access");
        }
    }
}