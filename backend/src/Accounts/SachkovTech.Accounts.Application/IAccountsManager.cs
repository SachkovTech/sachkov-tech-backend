using SachkovTech.Accounts.Domain;

namespace SachkovTech.Accounts.Application;

public interface IAccountsManager
{
    Task CreateParticipantAccount(
        ParticipantAccount participantAccount,
        CancellationToken cancellationToken = default);

    Task CreateStudentAccount(
        StudentAccount studentAccount,
        CancellationToken cancellationToken = default);
}