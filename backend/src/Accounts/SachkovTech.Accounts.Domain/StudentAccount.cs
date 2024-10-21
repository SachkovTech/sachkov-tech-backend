using SachkovTech.SharedKernel.ValueObjects;

namespace SachkovTech.Accounts.Domain;

public class StudentAccount
{
    public const string STUDENT = "Student";

    private StudentAccount()
    {
        
    }

    public StudentAccount(User user, List<SocialNetwork>? socialNetworks = null)
    {
        Id = Guid.NewGuid();
        User = user;
        SocialNetworks = socialNetworks;
        DateStartedStudying = DateTime.UtcNow;
    }
    
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public User User { get; set; }
    
    public IReadOnlyList<SocialNetwork>? SocialNetworks { get; set; }
    
    public DateTime DateStartedStudying { get; set; }
}