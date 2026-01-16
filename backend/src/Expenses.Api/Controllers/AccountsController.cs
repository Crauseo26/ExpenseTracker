using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Expenses.Api.DTOs;
using Expenses.Application.Commands;
using Expenses.Application.Queries;
using Expenses.Application.DTOs;

namespace Expenses.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly CreateAccountCommandHandler _createAccountHandler;
    private readonly UpdateAccountCommandHandler _updateAccountHandler;
    private readonly DeleteAccountCommandHandler _deleteAccountHandler;
    private readonly GetAccountByIdQueryHandler _getAccountByIdHandler;
    private readonly GetAccountsByUserQueryHandler _getAccountsByUserHandler;

    public AccountsController(
        CreateAccountCommandHandler createAccountHandler,
        UpdateAccountCommandHandler updateAccountHandler,
        DeleteAccountCommandHandler deleteAccountHandler,
        GetAccountByIdQueryHandler getAccountByIdHandler,
        GetAccountsByUserQueryHandler getAccountsByUserHandler)
    {
        _createAccountHandler = createAccountHandler;
        _updateAccountHandler = updateAccountHandler;
        _deleteAccountHandler = deleteAccountHandler;
        _getAccountByIdHandler = getAccountByIdHandler;
        _getAccountsByUserHandler = getAccountsByUserHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var command = new CreateAccountCommand
        {
            UserId = userId,
            Name = request.Name,
            ExpenseGroupId = request.ExpenseGroupId
        };

        var result = await _createAccountHandler.HandleAsync(command);

        if (!result.Success)
        {
            return BadRequest(new ErrorResponse { Message = result.ErrorMessage ?? "Failed to create account" });
        }

        return CreatedAtAction(
            nameof(GetAccountById),
            new { id = result.Account!.Id },
            result.Account);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAccountById(Guid id)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var query = new GetAccountByIdQuery
        {
            AccountId = id,
            UserId = userId
        };

        var result = await _getAccountByIdHandler.HandleAsync(query);

        if (!result.Success)
        {
            return NotFound(new ErrorResponse { Message = result.ErrorMessage ?? "Account not found" });
        }

        return Ok(result.Account);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAccounts([FromQuery] Guid? expenseGroupId)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var query = new GetAccountsByUserQuery
        {
            UserId = userId,
            ExpenseGroupId = expenseGroupId
        };

        var result = await _getAccountsByUserHandler.HandleAsync(query);

        return Ok(result.Accounts);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountRequest request)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var command = new UpdateAccountCommand
        {
            AccountId = id,
            UserId = userId,
            Name = request.Name,
            ExpenseGroupId = request.ExpenseGroupId
        };

        var result = await _updateAccountHandler.HandleAsync(command);

        if (!result.Success)
        {
            if (result.ErrorMessage?.Contains("not found") == true)
            {
                return NotFound(new ErrorResponse { Message = result.ErrorMessage });
            }
            return BadRequest(new ErrorResponse { Message = result.ErrorMessage ?? "Failed to update account" });
        }

        return Ok(result.Account);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var command = new DeleteAccountCommand
        {
            AccountId = id,
            UserId = userId
        };

        var result = await _deleteAccountHandler.HandleAsync(command);

        if (!result.Success)
        {
            return NotFound(new ErrorResponse { Message = result.ErrorMessage ?? "Account not found" });
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
