using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Accounts.Domain;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Extensions;
using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects;

namespace SachkovTech.Accounts.Application.Commands.Register;

public class RegisterUserHandler : ICommandHandler<RegisterUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IAccountsManager _accountsManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterUserHandler> _logger;
    private readonly IValidator<RegisterUserCommand> _validator;

    public RegisterUserHandler(
        UserManager<User> userManager, 
        RoleManager<Role> roleManager,
        IAccountsManager accountsManager,
        [FromKeyedServices(Modules.Accounts)] IUnitOfWork unitOfWork,
        ILogger<RegisterUserHandler> logger,
        IValidator<RegisterUserCommand> validator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _accountsManager = accountsManager;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> Handle(
        RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            return validationResult.ToList();

        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        try
        {
            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == ParticipantAccount.PARTICIPANT, cancellationToken);

            if (role is null)
                return Errors.General.NotFound(null, "role").ToErrorList();
            
            var fullName = FullName.Create(
                command.FullName.FirstName,
                command.FullName.SecondName).Value;

            var userResult = User.CreateParticipant(command.UserName, command.Email, fullName, role);

            if (userResult.IsFailure)
                return userResult.Error.ToErrorList();

            var result = await _userManager.CreateAsync(userResult.Value, command.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => Error.Failure(e.Code, e.Description));
                return new ErrorList(errors);
            }

            var participantAccount = new ParticipantAccount(userResult.Value);

            await _accountsManager.CreateParticipantAccount(participantAccount, cancellationToken);

            await _unitOfWork.SaveChanges(cancellationToken);
            
            transaction.Commit();
            
            _logger.LogInformation("User was created with name {userName}", command.UserName);

            return Result.Success<ErrorList>();
        }
        catch (Exception ex)
        {
            _logger.LogError("User registration was failed");
            
            transaction.Rollback();

            return Error.Failure("register.user", "User registration was failed").ToErrorList();
        }
    }
}