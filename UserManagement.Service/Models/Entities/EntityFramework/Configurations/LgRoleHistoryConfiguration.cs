using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User_Management.Constants;

namespace User_Management.Models.Entities.EntityFramework.Configurations
{
    public class LgRoleHistoryConfiguration : IEntityTypeConfiguration<LgRole>
    {
        public void Configure(EntityTypeBuilder<LgRole> builder)
        {
            builder.ToTable(RoleConstants.TableLogName);



            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("nextval('lg_role_histories_id_seq'::regclass)");

            builder.Property(e => e.Activity)
                .HasMaxLength(50)
                .HasColumnName("activity");

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

            builder.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");

            builder.Property(e => e.IsActive).HasColumnName("is_active");

            builder.Property(e => e.IsAdmin).HasColumnName("is_admin");

            builder.Property(e => e.ModifiedBy)
                .HasColumnType("character varying")
                .HasColumnName("modified_by");

            builder.Property(e => e.ModifiedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modified_date");

            builder.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");

            builder.Property(e => e.Note)
                .HasColumnType("character varying")
                .HasColumnName("note");

            builder.Property(e => e.RoleId).HasColumnName("role_id");

            builder.Property(e => e.Time)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time");

            builder.Property(e => e.User)
                .HasMaxLength(50)
                .HasColumnName("user");

            builder.HasOne(d => d.Role)
                .WithMany(p => p.LgRoleHistories)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("role_id");
        }
    }
}