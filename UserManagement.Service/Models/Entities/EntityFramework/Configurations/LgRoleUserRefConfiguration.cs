using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User_Management.Models.Entities.EntityFramework.Configurations
{
    public class LgRoleUserRefConfiguration : IEntityTypeConfiguration<LgRoleUserRef>
    {
        public void Configure(EntityTypeBuilder<LgRoleUserRef> builder)
        {
            builder.ToTable("lg_role_user_ref");

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
                .WithMany(p => p.LgRoleUserRefs)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ref_to_parent");
        }
    }
}