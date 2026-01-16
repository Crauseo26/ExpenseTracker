using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Expenses.Api.DTOs;
using Expenses.Application.Commands;
using Expenses.Application.Queries;
using Expenses.Application.DTOs;

namespace Expenses.Api.Controllers;

[ApiController]
[Route("api/account-groups")]
[Authorize]
public class AccountGroupsController : ControllerBase
{
    private readonly CreateAccountGroupCommandHandler _createAccountGroupHandler;
    private readonly UpdateAccountGroupCommandHandler _updateAccountGroupHandler;
    private readonly DeleteAccountGroupCommandHandler _deleteAccountGroupHandler;
    private readonly GetAccountGroupByIdQueryHandler _getAccountGroupByIdHandler;
    private readonly GetAccountGroupsByUserQueryHandler _getAccountGroupsByUserHandler;

    public AccountGroupsController(
        CreateAccountGroupCommandHandler createAccountGroupHandler,
        UpdateAccountGroupCommandHandler updateAccountGroupHandler,
        DeleteAccountGroupCommandHandler deleteAccountGroupHandler,
        GetAccountGroupByIdQueryHandler getAccountGroupByIdHandler,
        GetAccountGroupsByUserQueryHandler getAccountGroupsByUserHandler)
    {
        _createAccountGroupHandler = createAccountGroupHandler;
        _updateAccountGroupHandler = updateAccountGroupHandler;
        _deleteAccountGroupHandler = deleteAccountGroupHandler;
        _getAccountGroupByIdHandler = getAccountGroupByIdHandler;
        _getAccountGroupsByUserHandler = getAccountGroupsByUserHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AccountGroupDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAccountGroup([FromBody] CreateAccountGroupRequest request)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var command = new CreateAccountGroupCommand
        {
            UserId = userId,
            Name = request.Name
        };

        var result = await _createAccountGroupHandler.HandleAsync(command);

        if (!result.Success)
        {
            return BadRequest(new ErrorResponse { Message = result.ErrorMessage ?? "Failed to create account group" });
        }

        return CreatedAtAction(
            nameof(GetAccountGroupById),
            new { id = result.AccountGroup!.Id },
            result.AccountGroup);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AccountGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAccountGroupById(Guid id)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var query = new GetAccountGroupByIdQuery
        {
            AccountGroupId = id,
            UserId = userId
        };

        var result = await _getAccountGroupByIdHandler.HandleAsync(query);

        if (!result.Success)
        {
            return NotFound(new ErrorResponse { Message = result.ErrorMessage ?? "Account group not found" });
        }

        return Ok(result.AccountGroup);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccountGroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAccountGroups()
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var query = new GetAccountGroupsByUserQuery
        {
            UserId = userId
        };

        var result = await _getAccountGroupsByUserHandler.HandleAsync(query);

        return Ok(result.AccountGroups);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AccountGroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateAccountGroup(Guid id, [FromBody] UpdateAccountGroupRequest request)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var command = new UpdateAccountGroupCommand
        {
            AccountGroupId = id,
            UserId = userId,
            Name = request.Name
        };

        var result = await _updateAccountGroupHandler.HandleAsync(command);

        if (!result.Success)
        {
            if (result.ErrorMessage?.Contains("not found") == true)
            {
                return NotFound(new ErrorResponse { Message = result.ErrorMessage });
            }
            return BadRequest(new ErrorResponse { Message = result.ErrorMessage ?? "Failed to update account group" });
        }

        return Ok(result.AccountGroup);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAccountGroup(Guid id)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var command = new DeleteAccountGroupCommand
        {
            AccountGroupId = id,
            UserId = userId
        };

        var result = await _deleteAccountGroupHandler.HandleAsync(command);

        if (!result.Success)
        {
            return NotFound(new ErrorResponse { Message = result.ErrorMessage ?? "Account group not found" });
        }

        return NoContent();
    }

    private Guid GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Guid.Empty;
        }

        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
