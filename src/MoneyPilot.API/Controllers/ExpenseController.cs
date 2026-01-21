namespace MoneyPilot.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyPilot.Application.DTOs;
using MoneyPilot.Application.Interfaces;
using System.Security.Claims;



[ApiController]
[Route("api/[controller]")]
[Authorize] // 🔒 Protect entire controller
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("UserId missing in token");
    }

    // GET api/expense
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        var expenses = await _expenseService.GetAllAsync(userId);
        return Ok(expenses);
    }


    // DEV ONLY
    [AllowAnonymous]
    [HttpGet("debug")]
    public IActionResult Debug()
    {
        return Ok("Expense controller reached");
    }
}



//// GET api/expense/{id}
//[HttpGet("{id:int}")]
//public async Task<IActionResult> Get(int id)
//{
//    var userId = GetUserId();
//    var expense = await _expenseService.GetByIdAsync(id, userId);
//    return expense == null ? NotFound() : Ok(expense);
//}

//// POST api/expense
//[HttpPost]
//public async Task<IActionResult> Create([FromBody] ExpenseDto dto)
//{
//    var userId = GetUserId();
//    var createdId = await _expenseService.AddAsync(dto, userId);
//    return CreatedAtAction(nameof(Get), new { id = createdId }, dto);
//}

//// PUT api/expense/{id}
//[HttpPut("{id:int}")]
//public async Task<IActionResult> Update(int id, [FromBody] ExpenseDto dto)
//{
//    var userId = GetUserId();
//    var success = await _expenseService.UpdateAsync(id, dto, userId);
//    return success ? NoContent() : NotFound();
//}

//// DELETE api/expense/{id}
//[HttpDelete("{id:int}")]
//public async Task<IActionResult> Delete(int id)
//{
//    var userId = GetUserId();
//    var success = await _expenseService.DeleteAsync(id, userId);
//    return success ? NoContent() : NotFound();
//}