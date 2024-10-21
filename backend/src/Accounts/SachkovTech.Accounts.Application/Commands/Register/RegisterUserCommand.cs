using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Dtos;

namespace SachkovTech.Accounts.Application.Commands.Register;

public record RegisterUserCommand(
    string Email, 
    string UserName, 
    string Password,
    FullNameDto FullName) : ICommand;