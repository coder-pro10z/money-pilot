using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyPilot.Application.DTOs;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("UserId missing");

    // GET api/expense
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var expenses = await _expenseService.GetAllAsync(GetUserId());
        return Ok(expenses);
    }

    // GET api/expense/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var expense = await _expenseService.GetByIdAsync(id, GetUserId());
        return expense == null ? NotFound() : Ok(expense);
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
