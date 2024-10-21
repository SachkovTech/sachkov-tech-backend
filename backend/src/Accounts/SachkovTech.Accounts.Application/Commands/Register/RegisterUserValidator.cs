using FluentValidation;
using SachkovTech.Core.Validation;
using SachkovTech.SharedKernel.ValueObjects;

namespace SachkovTech.Accounts.Application.Commands.Register;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(c => c.FullName)
            .MustBeValueObject(f => FullName.Create(f.FirstName, f.SecondName));
    }
}