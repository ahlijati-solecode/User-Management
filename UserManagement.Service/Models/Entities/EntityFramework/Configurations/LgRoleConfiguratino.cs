using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User_Management.Constants;

namespace User_Management.Models.Entities.EntityFramework.Configurations
{
    public class LgRoleConfiguratino : IEntityTypeConfiguration<MdRole>
    {
        public void Configure(EntityTypeBuilder<MdRole> builder)
        {
            builder.ToTable(RoleConstants.TableName);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("nextval('lg_roles_id_seq'::regclass)");

            builder.Property(e => e.ApprovedBy)
                .HasColumnType("character varying")
                .HasColumnName("approved_by");

            builder.Property(e => e.ApprovedTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("approved_time");

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

            builder.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");

            builder.Property(e => e.IsActive).HasColumnName("is_active");

            builder.Property(e => e.IsAdmin).HasColumnName("is_admin");

            builder.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .HasColumnName("modified_by");

            builder.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_date");
            builder.Property(e => e.ApprovalStatus).HasColumnName("approval_status");
            builder.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        }
    }
}