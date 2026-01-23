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


