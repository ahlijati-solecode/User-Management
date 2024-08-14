using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User_Management.Models.Entities.Custom.Role;
namespace User_Management.Models.Entities.EntityFramework.Configurations
{
    public class MdRefMenuConfiguration : IEntityTypeConfiguration<MdRefMenu>
    {
        public void Configure(EntityTypeBuilder<MdRefMenu> builder)
        {
            builder.ToTable("md_ref_menu");

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("nextval('lg_ref_menu_id_seq'::regclass)");
            builder.Property(e => e.Key)
                .HasColumnName("key");
            builder.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        }
    }
}
