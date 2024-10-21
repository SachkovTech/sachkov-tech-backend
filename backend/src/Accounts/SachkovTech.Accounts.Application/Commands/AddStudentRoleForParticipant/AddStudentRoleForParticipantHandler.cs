using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Accounts.Domain;
using SachkovTech.Core.Abstractions;
using SachkovTech.SharedKernel;

namespace SachkovTech.Accounts.Application.Commands.AddStudentRoleForParticipant;

public class AddStudentRoleForParticipantHandler : ICommandHandler<AddStudentRoleForParticipantCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IAccountsManager _accountsManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddStudentRoleForParticipantHandler> _logger;

    public AddStudentRoleForParticipantHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IAccountsManager accountsManager,
        [FromKeyedServices(Modules.Accounts)] IUnitOfWork unitOfWork,
        ILogger<AddStudentRoleForParticipantHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _accountsManager = accountsManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    
    public async Task<UnitResult<ErrorList>> Handle(
        AddStudentRoleForParticipantCommand command,
        CancellationToken cancellationToken = default)
    {
        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == StudentAccount.STUDENT, cancellationToken);

            if (role is null)
                return Errors.General.NotFound(null, "role").ToErrorList();

            var user = await _userManager.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

            if (user is null)
                return Errors.General.NotFound(null, "user").ToErrorList();

            user.AddRole(role);

            var studentAccount = new StudentAccount(user);

            await _accountsManager.CreateStudentAccount(studentAccount, cancellationToken);

            await _unitOfWork.SaveChanges(cancellationToken);
            
            transaction.Commit();
            
            _logger.LogInformation("Student role was added for user {userName}", user.UserName);

            return Result.Success<ErrorList>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}