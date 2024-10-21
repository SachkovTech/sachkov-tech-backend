using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SachkovTech.Accounts.Domain;
using SachkovTech.Core.Dtos;
using SachkovTech.Core.Extensions;
using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects;

namespace SachkovTech.Accounts.Infrastructure.Configurations.Write;

public class SupportAccountConfiguration : IEntityTypeConfiguration<SupportAccount>
{
    public void Configure(EntityTypeBuilder<SupportAccount> builder)
    {
        builder.ToTable("support_accounts");
        
        builder.HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<SupportAccount>(s => s.UserId);

        builder.Property(s => s.AboutSelf)
            .HasMaxLength(Constants.Default.MAX_HIGH_TEXT_LENGTH)
            .IsRequired();
        
        builder.Property(s => s.SocialNetworks)
            .ValueObjectsCollectionJsonConversion(
                input => new SocialNetworkDto(input.Name, input.Link),
                output => SocialNetwork.Create(output.Name, output.Link).Value)
            .HasColumnName("social_networks");
    }
}