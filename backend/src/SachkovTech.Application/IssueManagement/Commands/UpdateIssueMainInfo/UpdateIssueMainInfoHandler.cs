﻿using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SachkovTech.Application.Abstraction;
using SachkovTech.Application.Database;
using SachkovTech.Application.Extensions;
using SachkovTech.Domain.IssueManagement.ValueObjects;
using SachkovTech.Domain.Shared;
using SachkovTech.Domain.Shared.ValueObjects;
using SachkovTech.Domain.Shared.ValueObjects.Ids;

namespace SachkovTech.Application.IssueManagement.Commands.UpdateIssueMainInfo;

public class UpdateIssueMainInfoHandler : ICommandHandler<Guid, UpdateIssueMainInfoCommand>
{
    private readonly IModulesRepository _modulesRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateIssueMainInfoCommand> _validator;
    private readonly ILogger<UpdateIssueMainInfoHandler> _logger;

    public UpdateIssueMainInfoHandler(
        IModulesRepository modulesRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateIssueMainInfoCommand> validator,
        ILogger<UpdateIssueMainInfoHandler> logger)
    {
        _modulesRepository = modulesRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        UpdateIssueMainInfoCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsValid == false)
        {
            return validationResult.ToList();
        }

        var moduleResult = await _modulesRepository.GetById(command.ModuleId, cancellationToken);
        if (moduleResult.IsFailure)
            return moduleResult.Error.ToErrorList();

        var issueResult = moduleResult.Value.Issues.FirstOrDefault(i => i.Id.Value == command.IssueId);
        if (issueResult == null)
            return Errors.General.NotFound(command.IssueId).ToErrorList();

        var title = Title.Create(command.Title).Value;
        var description = Description.Create(command.Description).Value;
        var experience = Experience.Create(command.Experience).Value;
        var lessonId = LessonId.Empty();

        var updateResult = moduleResult.Value.UpdateIssueInfo(
            issueResult.Id.Value,
            title,
            description,
            lessonId,
            experience);
        if (updateResult.IsFailure)
            return updateResult.Error.ToErrorList();

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation(
            "Issue main info was updated with id {issueId} in module {moduleId}",
            command.IssueId,
            command.ModuleId);

        return moduleResult.Value.Id.Value;
    }
}