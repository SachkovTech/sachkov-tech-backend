using SachkovTech.Core.Abstractions;

namespace SachkovTech.Accounts.Application.Commands.AddStudentRoleForParticipant;

public record AddStudentRoleForParticipantCommand(string Email) : ICommand;