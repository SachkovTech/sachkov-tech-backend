using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SachkovTech.Accounts.Domain;

namespace SachkovTech.Accounts.Infrastructure.Configurations.Write;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity<IdentityUserRole<Guid>>();
        
        builder.ComplexProperty(a => a.FullName, fb =>
        {
            fb.Property(a => a.FirstName).IsRequired().HasColumnName("first_name");
            fb.Property(a => a.SecondName).IsRequired().HasColumnName("second_name");
        });
    }
}