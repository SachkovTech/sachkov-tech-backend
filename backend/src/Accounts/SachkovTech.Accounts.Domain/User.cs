using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects;

namespace SachkovTech.Accounts.Domain;

public class User : IdentityUser<Guid>
{
    private User()
    {
    }

    private List<Role> _roles = [];
    
    public DateTime RegistrationDate { get; set; }
    
    public FullName FullName { get; set; }

    public IReadOnlyList<Role> Roles => _roles;

    public static Result<User, Error> CreateAdmin(
        string userName, 
        string email, 
        FullName fullName, 
        Role role)
    {
        if (role.Name != AdminAccount.ADMIN)
            return Errors.User.InvalidRole();
        
        return new User
        {
            UserName = userName,
            Email = email,
            RegistrationDate = DateTime.UtcNow,
            FullName = fullName,
            _roles = [role]
        };
    }

    public static Result<User, Error> CreateParticipant(
        string userName, 
        string email, 
        FullName fullName, 
        Role role)
    {
        if (role.Name != ParticipantAccount.PARTICIPANT)
            return Errors.User.InvalidRole();

        return new User
        {
            UserName = userName,
            Email = email,
            RegistrationDate = DateTime.UtcNow,
            FullName = fullName,
            _roles = [role]
        };
    }

    public void AddRole(Role role)
    {
        _roles.Add(role);
    }
}