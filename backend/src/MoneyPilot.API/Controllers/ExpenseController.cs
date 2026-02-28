using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyPilot.Application.DTOs;
using System.Security.Claims;
using Microsoft.Extensions.Logging; // ADD THIS
using MoneyPilot.Application.Common;
using MoneyPilot.Domain.Entities;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly ILogger<ExpenseController> _logger; 

    public ExpenseController(IExpenseService expenseService, ILogger<ExpenseController> logger)
    {
        _expenseService = expenseService;
        _logger = logger;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("UserId missing");

    // GET api/expense
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] int? categoryId = null)
    {
        var userId = GetUserId();
        _logger.LogInformation("User {userId} requested Expenses page {page} size {pageSize}", userId, page, pageSize);

        var expenses = (await _expenseService.GetAllAsync(userId)).AsQueryable();

        if (startDate.HasValue)
            expenses = expenses.Where(e => e.Date >= startDate.Value);
        if (endDate.HasValue)
            expenses = expenses.Where(e => e.Date <= endDate.Value);
        if (categoryId.HasValue)
            expenses = expenses.Where(e => e.CategoryId == categoryId.Value);

        var total = expenses.Count();
        var items = expenses.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var paged = new PagedResponse<ExpenseResponseDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };

        return Ok(ApiResponse<PagedResponse<ExpenseResponseDto>>.SuccessResponse(paged));
    }

    // GET api/expense/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var expense = await _expenseService.GetByIdAsync(id, GetUserId());
        return expense == null ? NotFound(ApiResponse<string>.FailureResponse("Expense not found")) 
                               : Ok(ApiResponse<ExpenseResponseDto>.SuccessResponse(expense));
    }

    // POST api/expense
    [HttpPost]
    public async Task<IActionResult> Create(ExpenseDto dto)
    {
        var created = await _expenseService.CreateAsync(dto, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT api/expense/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ExpenseDto dto)
    {
        var success = await _expenseService.UpdateAsync(id, dto, GetUserId());
        return success ? NoContent() : NotFound();
    }

    // DELETE api/expense/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _expenseService.DeleteAsync(id, GetUserId());
        return success ? NoContent() : NotFound();
    }
}
