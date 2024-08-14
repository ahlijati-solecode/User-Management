using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace User_Management.Models.Entities.EntityFramework.Configurations
{
    public class LgActivityConfiguration : IEntityTypeConfiguration<Shared.Models.Entities.LgActivity>
    {
        public void Configure(EntityTypeBuilder<Shared.Models.Entities.LgActivity> builder)
        {
            builder.ToTable("lg_activity");

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Activity)
                .HasColumnType("character varying")
                .HasColumnName("activity");

            builder.Property(e => e.Time)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time");

            builder.Property(e => e.Username)
                .HasColumnType("character varying")
                .HasColumnName("username");
        }
    }
}