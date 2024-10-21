using SachkovTech.Core.Dtos;

namespace SachkovTech.Accounts.Contracts.Dtos;

public class SupportAccountDto
{
    public Guid Id { get; set; }

    public IEnumerable<SocialNetworkDto> SocialNetworks { get; set; } = [];

    public string AboutSelf { get; set; } = default!;
}