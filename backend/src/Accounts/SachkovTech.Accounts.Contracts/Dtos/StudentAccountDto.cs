using SachkovTech.Core.Dtos;

namespace SachkovTech.Accounts.Contracts.Dtos;

public class StudentAccountDto
{
    public Guid Id { get; set; }

    public IEnumerable<SocialNetworkDto>? SocialNetworks { get; set; } = [];
    
    public DateTime DateStartedStudying { get; set; }
}