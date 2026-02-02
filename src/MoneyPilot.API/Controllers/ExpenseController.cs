using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyPilot.Application.DTOs;
using System.Security.Claims;
using Microsoft.Extensions.Logging; // ADD THIS


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
    public async Task<IActionResult> GetAll()
    {
        //Logging the action
        var userId = GetUserId();
        _logger.LogInformation("User {userId} requested all the Expenses, Time:{time}. Request from {RemoteIpAddress} ",
                                userId,DateTime.Now,HttpContext.Connection.RemoteIpAddress);
        try
        {
        var expenses = await _expenseService.GetAllAsync(userId);
            _logger.LogDebug("User {userId} has {count} Expenses.",userId,expenses.Count());
            return Ok(expenses);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while User {userId} was retrieving Expenses at {time}.", userId, DateTime.Now);
            return BadRequest();
        }
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
