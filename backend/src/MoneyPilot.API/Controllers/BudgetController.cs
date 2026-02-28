using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyPilot.Application.Common;
using MoneyPilot.Application.DTOs;
using MoneyPilot.Application.Interfaces;
using System.Security.Claims;
using MoneyPilot.Application.Common;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BudgetController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("UserId missing");

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var budgets = await _budgetService.GetAllAsync(GetUserId());

        if (budgets == null)
            return NotFound();

        return Ok(
            ApiResponse<PagedResponse<BudgetResponseDto>>.SuccessResponse(budgets)
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var budget = await _budgetService.GetByIdAsync(id, GetUserId());

        if (budget == null)
            return NotFound();

        return Ok(
            ApiResponse<BudgetResponseDto>
            .SuccessResponse(budget)
        );
    }

    [HttpPost]
    public async Task<IActionResult> Create(BudgetDto dto)
    {
        var created = await _budgetService.AddAsync(dto, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BudgetDto dto)
    {
        var success = await _budgetService.UpdateAsync(id, dto, GetUserId());
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _budgetService.DeleteAsync(id, GetUserId());
        return success ? NoContent() : NotFound();
    }
}
