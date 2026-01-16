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
public class ExpensesController : ControllerBase
{
    private readonly CreateExpenseCommandHandler _createExpenseHandler;
    private readonly UpdateExpenseCommandHandler _updateExpenseHandler;
    private readonly ConfirmExpenseCommandHandler _confirmExpenseHandler;
    private readonly DeleteExpenseCommandHandler _deleteExpenseHandler;
    private readonly GetExpenseByIdQueryHandler _getExpenseByIdHandler;
    private readonly GetExpensesByUserQueryHandler _getExpensesByUserHandler;

    public ExpensesController(
        CreateExpenseCommandHandler createExpenseHandler,
        UpdateExpenseCommandHandler updateExpenseHandler,
        ConfirmExpenseCommandHandler confirmExpenseHandler,
        DeleteExpenseCommandHandler deleteExpenseHandler,
        GetExpenseByIdQueryHandler getExpenseByIdHandler,
        GetExpensesByUserQueryHandler getExpensesByUserHandler)
    {
        _createExpenseHandler = createExpenseHandler;
        _updateExpenseHandler = updateExpenseHandler;
        _confirmExpenseHandler = confirmExpenseHandler;
        _deleteExpenseHandler = deleteExpenseHandler;
        _getExpenseByIdHandler = getExpenseByIdHandler;
        _getExpensesByUserHandler = getExpensesByUserHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequest request)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var command = new CreateExpenseCommand
        {
            UserId = userId,
            AccountId = request.AccountId,
            Amount = request.Amount,
            Currency = request.Currency,
            Description = request.Description,
            ExpenseType = request.ExpenseType,
            PurchaseDate = request.PurchaseDate
        };

        var result = await _createExpenseHandler.HandleAsync(command);

        if (!result.Success)
        {
            return BadRequest(new ErrorResponse { Message = result.ErrorMessage ?? "Failed to create expense" });
        }

        return CreatedAtAction(
            nameof(GetExpenseById),
            new { id = result.Expense!.Id },
            result.Expense);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpenseById(Guid id)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var query = new GetExpenseByIdQuery
        {
            ExpenseId = id,
            UserId = userId
        };

        var result = await _getExpenseByIdHandler.HandleAsync(query);

        if (!result.Success)
        {
            return NotFound(new ErrorResponse { Message = result.ErrorMessage ?? "Expense not found" });
        }

        return Ok(result.Expense);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ExpenseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpenses(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var query = new GetExpensesByUserQuery
        {
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate
        };

        var result = await _getExpensesByUserHandler.HandleAsync(query);

        return Ok(result.Expenses);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseRequest request)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var command = new UpdateExpenseCommand
        {
            ExpenseId = id,
            UserId = userId,
            AccountId = request.AccountId,
            Amount = request.Amount,
            Currency = request.Currency,
            Description = request.Description,
            ExpenseType = request.ExpenseType,
            PurchaseDate = request.PurchaseDate
        };

        var result = await _updateExpenseHandler.HandleAsync(command);

        if (!result.Success)
        {
            if (result.ErrorMessage?.Contains("not found") == true)
            {
                return NotFound(new ErrorResponse { Message = result.ErrorMessage });
            }
            return BadRequest(new ErrorResponse { Message = result.ErrorMessage ?? "Failed to update expense" });
        }

        return Ok(result.Expense);
    }

    [HttpPost("{id}/confirm")]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ConfirmExpense(Guid id)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var command = new ConfirmExpenseCommand
        {
            ExpenseId = id,
            UserId = userId
        };

        var result = await _confirmExpenseHandler.HandleAsync(command);

        if (!result.Success)
        {
            if (result.ErrorMessage?.Contains("not found") == true)
            {
                return NotFound(new ErrorResponse { Message = result.ErrorMessage });
            }
            return BadRequest(new ErrorResponse { Message = result.ErrorMessage ?? "Failed to confirm expense" });
        }

        return Ok(result.Expense);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteExpense(Guid id)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var command = new DeleteExpenseCommand
        {
            ExpenseId = id,
            UserId = userId
        };

        var result = await _deleteExpenseHandler.HandleAsync(command);

        if (!result.Success)
        {
            return NotFound(new ErrorResponse { Message = result.ErrorMessage ?? "Expense not found" });
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
