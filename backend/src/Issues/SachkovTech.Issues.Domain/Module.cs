using CSharpFunctionalExtensions;
using SachkovTech.Issues.Domain.Entities;
using SachkovTech.Issues.Domain.ValueObjects;
using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects;
using SachkovTech.SharedKernel.ValueObjects.Ids;

namespace SachkovTech.Issues.Domain;

public class Module : SoftDeletableEntity<ModuleId>
{
    private readonly List<Issue> _issues = [];

    // ef core
    private Module(ModuleId id) : base(id)
    {
    }

    public Module(ModuleId moduleId, Title title, Description description)
        : base(moduleId)
    {
        Title = title;
        Description = description;
    }

    public Title Title { get; private set; } = default!;

    public Description Description { get; private set; } = default!;

    public IReadOnlyList<Issue> Issues => _issues;

    public int GetNumberOfIssues() => _issues.Count;

    public Result<Issue, Error> GetIssueById(IssueId issueId)
    {
        var issue = _issues.FirstOrDefault(i => i.Id == issueId);
        if (issue is null)
            return Errors.General.NotFound(issueId.Value);

        return issue;
    }

    public void UpdateMainInfo(Title title, Description description)
    {
        Title = title;
        Description = description;
    }

    public UnitResult<Error> DeleteIssue(IssueId issueId)
    {
        var issue = _issues.FirstOrDefault(i => i.Id == issueId);
        if (issue is null)
            return Result.Success<Error>();

        RecalculatePositionOfOtherIssues(issue.Position);

        _issues.Remove(issue);
        return Result.Success<Error>();
    }

    public UnitResult<Error> SoftDeleteIssue(IssueId issueId)
    {
        var issue = _issues.FirstOrDefault(i => i.Id == issueId);
        if (issue is null)
            return Result.Success<Error>();

        var result = RecalculatePositionOfOtherIssues(issue.Position);
        if (result.IsFailure)
            return result.Error;

        issue.SoftDelete();
        return Result.Success<Error>();
    }

    public void DeleteExpiredIssues()
    {
        _issues.RemoveAll(i => i.DeletionDate != null
                               && DateTime.UtcNow >= i.DeletionDate.Value
                                   .AddDays(Constants.Issues.LIFETIME_AFTER_DELETION));
    }

    public UnitResult<Error> RestoreIssue(IssueId issueId)
    {
        var issue = _issues.FirstOrDefault(i => i.Id == issueId);
        if (issue is null)
            return Result.Success<Error>();

        issue.Restore();

        var resultMove = MoveIssue(issue, Position.Create(_issues.Count).Value);
        if (resultMove.IsFailure)
            return resultMove.Error;

        return Result.Success<Error>();
    }

    public override void SoftDelete()
    {
        base.SoftDelete();

        foreach (var issue in _issues)
            issue.SoftDelete();
    }

    public override void Restore()
    {
        base.Restore();

        foreach (var issue in _issues)
            issue.Restore();
    }

    public UnitResult<Error> AddIssue(Issue issue)
    {
        var serialNumberResult = Position.Create(_issues.Count + 1);
        if (serialNumberResult.IsFailure)
            return serialNumberResult.Error;

        issue.SetPosition(serialNumberResult.Value);

        _issues.Add(issue);
        return Result.Success<Error>();
    }

    public UnitResult<Error> UpdateIssueInfo(
        IssueId issueId,
        Title title,
        Description description,
        LessonId lessonId,
        Experience experience)
    {
        var issue = _issues.FirstOrDefault(i => i.Id == issueId);
        if (issue is null)
            return Errors.General.NotFound(issueId);

        var issueResult = issue.UpdateMainInfo(title, description, lessonId, experience);
        if (issueResult.IsFailure)
            return issueResult.Error;

        return Result.Success<Error>();
    }

    public UnitResult<Error> MoveIssue(Issue issue, Position newPosition)
    {
        var currentPosition = issue.Position;

        if (currentPosition == newPosition || _issues.Count == 1)
            return Result.Success<Error>();

        var adjustedPosition = AdjustNewPositionIfOutOfRange(newPosition);
        if (adjustedPosition.IsFailure)
            return adjustedPosition.Error;

        newPosition = adjustedPosition.Value;

        var moveResult = MoveIssuesBetweenPositions(newPosition, currentPosition);
        if (moveResult.IsFailure)
            return moveResult.Error;

        issue.Move(newPosition);

        return Result.Success<Error>();
    }

    private UnitResult<Error> MoveIssuesBetweenPositions(Position newPosition, Position currentPosition)
    {
        if (newPosition < currentPosition)
        {
            var issuesToMove = _issues.Where(i => i.Position >= newPosition
                                                  && i.Position < currentPosition);

            foreach (var issueToMove in issuesToMove)
            {
                var result = issueToMove.MoveForward();
                if (result.IsFailure)
                {
                    return result.Error;
                }
            }
        }
        else if (newPosition > currentPosition)
        {
            var issuesToMove = _issues.Where(i => i.Position > currentPosition
                                                  && i.Position <= newPosition);

            foreach (var issueToMove in issuesToMove)
            {
                var result = issueToMove.MoveBack();
                if (result.IsFailure)
                {
                    return result.Error;
                }
            }
        }

        return Result.Success<Error>();
    }

    private Result<Position, Error> AdjustNewPositionIfOutOfRange(Position newPosition)
    {
        if (newPosition.Value <= _issues.Count)
            return newPosition;

        var lastPosition = Position.Create(_issues.Count);
        if (lastPosition.IsFailure)
            return lastPosition.Error;

        return lastPosition.Value;
    }


    private UnitResult<Error> RecalculatePositionOfOtherIssues(Position currentPosition)
    {
        if (currentPosition == _issues.Count)
            return Result.Success<Error>();

        var issuesToMove = _issues.Where(i => i.Position > currentPosition);
        foreach (var issue in issuesToMove)
        {
            var result = issue.MoveBack();
            if (result.IsFailure)
            {
                return result.Error;
            }
        }

        return Result.Success<Error>();
    }
}