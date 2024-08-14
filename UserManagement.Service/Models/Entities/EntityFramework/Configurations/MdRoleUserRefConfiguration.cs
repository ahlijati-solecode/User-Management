using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User_Management.Models.Entities.EntityFramework.Configurations
{
    public class MdRoleUserRefConfiguration : IEntityTypeConfiguration<MdRoleUserRef>
    {
        public void Configure(EntityTypeBuilder<MdRoleUserRef> builder)
        {
            builder.ToTable("md_role_user_ref");

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");

            builder.Property(e => e.FullName)
                .HasColumnType("character varying")
                .HasColumnName("full_name");

            builder.Property(e => e.ParentId).HasColumnName("parent_id");
            builder.Property(e => e.Departement)
          .HasColumnType("character varying")
          .HasColumnName("departement");
            builder.Property(e => e.Username)
                .HasColumnType("character varying")
                .HasColumnName("username");
            builder.Property(e => e.IsApprover).HasColumnName("is_approver");
            builder.HasOne(d => d.Parent)
                .WithMany(p => p.MdRoleUserRefs)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ref_to_parent");
        }
    }
}