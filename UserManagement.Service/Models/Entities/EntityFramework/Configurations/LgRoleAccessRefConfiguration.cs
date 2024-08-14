using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User_Management.Models.Entities.EntityFramework.Configurations
{
    public class LgRoleAccessRefConfiguration : IEntityTypeConfiguration<LgRoleAccessRef>
    {
        public void Configure(EntityTypeBuilder<LgRoleAccessRef> builder)
        {
            builder.ToTable("lg_role_access_ref");

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("nextval('lg_ref_user_access_id_seq'::regclass)");

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

            builder.Property(e => e.IsCreate).HasColumnName("is_create");

            builder.Property(e => e.IsDelete).HasColumnName("is_delete");

            builder.Property(e => e.IsEdit).HasColumnName("is_edit");

            builder.Property(e => e.IsView).HasColumnName("is_view");

            builder.Property(e => e.ModifiedBy)
                .HasColumnType("character varying")
                .HasColumnName("modified_by");

            builder.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_date");

            builder.Property(e => e.ParentId).HasColumnName("parent_id");

            builder.Property(e => e.RefMenuId).HasColumnName("ref_menu_id");

            builder.Property(e => e.RefUserAccess).HasColumnName("ref_user_access");

            builder.HasOne(d => d.Parent)
                .WithMany(p => p.LgUserAccessRefs)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("ref_to_parent");

            builder.HasOne(d => d.RefMenu)
                .WithMany(p => p.LgUserAccessRefs)
                .HasForeignKey(d => d.RefMenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ref_user_access_menu");

            builder.HasOne(d => d.RefUserAccessNavigation)
                .WithMany(p => p.LgUserAccessRefs)
                .HasForeignKey(d => d.RefUserAccess)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ref_user_access");
        }
    }
}