using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SachkovTech.Accounts.Contracts.Dtos;
using SachkovTech.Core.Dtos;

namespace SachkovTech.Accounts.Infrastructure.Configurations.Read;

public class StudentAccountDtoConfiguration : IEntityTypeConfiguration<StudentAccountDto>
{
    public void Configure(EntityTypeBuilder<StudentAccountDto> builder)
    {
        builder.ToTable("student_accounts");

        builder.HasKey(s => s.Id);
        
        builder.Property(v => v.SocialNetworks)
            .HasConversion(
                values => JsonSerializer.Serialize(string.Empty, JsonSerializerOptions.Default),
                json => JsonSerializer.Deserialize<IEnumerable<SocialNetworkDto>>
                    (json, JsonSerializerOptions.Default)!);
    }
}