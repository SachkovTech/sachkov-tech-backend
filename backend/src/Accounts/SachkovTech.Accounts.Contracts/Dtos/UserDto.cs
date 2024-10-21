namespace SachkovTech.Accounts.Contracts.Dtos;

public class UserDto
{
    public Guid Id { get; init; }

    public string UserName { get; init; } = default!;

    public string FirstName { get; init; } = default!;

    public string LastName { get; init; } = default!;
}