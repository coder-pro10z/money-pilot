using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyPilot.Application.Common;
using MoneyPilot.Application.DTOs;
using MoneyPilot.Application.Interfaces;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("UserId missing");

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _categoryService.GetAllAsync(GetUserId());
        return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(list));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        var created = await _categoryService.CreateAsync(GetUserId(), dto);
        return Ok(ApiResponse<CategoryDto>.SuccessResponse(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateCategoryDto dto)
    {
        var updated = await _categoryService.UpdateAsync(GetUserId(), id, dto);
        return Ok(ApiResponse<CategoryDto>.SuccessResponse(updated));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteAsync(GetUserId(), id);
        return Ok(ApiResponse<string>.SuccessResponse("Deleted"));
    }
}
