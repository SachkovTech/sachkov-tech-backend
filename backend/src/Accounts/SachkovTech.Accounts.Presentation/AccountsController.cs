using Microsoft.AspNetCore.Mvc;
using SachkovTech.Accounts.Application.Commands.AddStudentRoleForParticipant;
using SachkovTech.Accounts.Application.Commands.Login;
using SachkovTech.Accounts.Application.Commands.RefreshTokens;
using SachkovTech.Accounts.Application.Commands.Register;
using SachkovTech.Accounts.Contracts.Requests;
using SachkovTech.Framework;
using SachkovTech.Framework.Authorization;

namespace SachkovTech.Accounts.Presentation;

public class AccountsController : ApplicationController
{
    [Permission(Permissions.Issues.CreateIssue)]
    [HttpPost("create")]
    public IActionResult CreateIssue()
    {
        return Ok();
    }

    [Permission("update.create")]
    [HttpPost("update")]
    public IActionResult UpdateIssue()
    {
        return Ok();
    }

    [HttpPost("registration")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        [FromServices] RegisterUserHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(
            new RegisterUserCommand(request.Email, request.UserName, request.Password, request.FullName),
            cancellationToken);
    
        if (result.IsFailure)
            return result.Error.ToResponse();
    
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginUserRequest request,
        [FromServices] LoginHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokens(
        [FromBody] RefreshTokensRequest request,
        [FromServices] RefreshTokensHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(
            new RefreshTokensCommand(request.AccessToken, request.RefreshToken),
            cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result.Value);
    }

    [Permission("accounts.add.role")]
    [HttpPut("student-role")]
    public async Task<ActionResult> AddStudentRoleForParticipant(
        [FromBody] AddStudentRoleForParticipantRequest request,
        [FromServices] AddStudentRoleForParticipantHandler handler,
        CancellationToken cancellationToken = default)
    {
        var command = new AddStudentRoleForParticipantCommand(request.Email);

        var result = await handler.Handle(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }
}