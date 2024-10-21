using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SachkovTech.Accounts.Domain;
using SachkovTech.Core.Dtos;
using SachkovTech.Core.Extensions;
using SachkovTech.SharedKernel.ValueObjects;

namespace SachkovTech.Accounts.Infrastructure.Configurations.Write;

public class StudentAccountConfiguration : IEntityTypeConfiguration<StudentAccount>
{
    public void Configure(EntityTypeBuilder<StudentAccount> builder)
    {
        builder.ToTable("student_accounts");
        
        builder.HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<StudentAccount>(s => s.UserId);
        
        builder.Property(s => s.SocialNetworks)
            .ValueObjectsCollectionJsonConversion(
                input => new SocialNetworkDto(input.Name, input.Link),
                output => SocialNetwork.Create(output.Name, output.Link).Value)
            .HasColumnName("social_networks");
    }
}