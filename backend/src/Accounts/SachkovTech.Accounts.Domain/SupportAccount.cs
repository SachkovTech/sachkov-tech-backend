using SachkovTech.SharedKernel.ValueObjects;

namespace SachkovTech.Accounts.Domain;

public class SupportAccount
{
    public const string SUPPORT = "Support";

    private SupportAccount()
    {
        
    }

    public SupportAccount(
        User user, 
        List<SocialNetwork> socialNetworks, 
        string aboutSelf)
    {
        Id = Guid.NewGuid();
        User = user; ;
        SocialNetworks = socialNetworks;
        AboutSelf = aboutSelf;
    }
    
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public User User { get; set; }
    
    public IReadOnlyList<SocialNetwork> SocialNetworks { get; set; }

    public string AboutSelf { get; set; }
}