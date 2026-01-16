using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Expenses.Api.DTOs;
using Expenses.Application.Commands;
using Expenses.Application.Queries;

namespace Expenses.Api.Controllers;

[ApiController]
[Route("api/expense-inputs")]
[Authorize]
public class ExpenseInputsController : ControllerBase
{
    private readonly ProcessExpenseInputCommandHandler _processExpenseInputHandler;
    private readonly GetExpenseInputByIdQueryHandler _getExpenseInputByIdHandler;
    private readonly GetExpenseInputsByUserQueryHandler _getExpenseInputsByUserHandler;

    public ExpenseInputsController(
        ProcessExpenseInputCommandHandler processExpenseInputHandler,
        GetExpenseInputByIdQueryHandler getExpenseInputByIdHandler,
        GetExpenseInputsByUserQueryHandler getExpenseInputsByUserHandler)
    {
        _processExpenseInputHandler = processExpenseInputHandler;
        _getExpenseInputByIdHandler = getExpenseInputByIdHandler;
        _getExpenseInputsByUserHandler = getExpenseInputsByUserHandler;
    }

    [HttpPost("text")]
    [ProducesResponseType(typeof(ProcessExpenseInputResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ProcessTextInput([FromBody] ProcessExpenseInputRequest request)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var command = new ProcessExpenseInputCommand
        {
            UserId = userId,
            InputType = request.InputType,
            RawContent = request.RawContent
        };

        var result = await _processExpenseInputHandler.HandleAsync(command);

        if (!result.Success)
        {
            return BadRequest(new ErrorResponse { Message = result.ErrorMessage ?? "Failed to process expense input" });
        }

        var response = new ProcessExpenseInputResponse
        {
            ExpenseInput = result.ExpenseInput!,
            CreatedExpenses = result.CreatedExpenses
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Application.DTOs.ExpenseInputDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpenseInputById(Guid id)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var query = new GetExpenseInputByIdQuery
        {
            ExpenseInputId = id,
            UserId = userId
        };

        var result = await _getExpenseInputByIdHandler.HandleAsync(query);

        if (!result.Success)
        {
            return NotFound(new ErrorResponse { Message = result.ErrorMessage ?? "Expense input not found" });
        }

        return Ok(result.ExpenseInput);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Application.DTOs.ExpenseInputDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExpenseInputs([FromQuery] bool? pendingOnly)
    {
        var userId = GetUserIdFromClaims();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse { Message = "Invalid user authentication" });
        }

        var query = new GetExpenseInputsByUserQuery
        {
            UserId = userId,
            PendingOnly = pendingOnly ?? false
        };

        var result = await _getExpenseInputsByUserHandler.HandleAsync(query);

        return Ok(result.ExpenseInputs);
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
