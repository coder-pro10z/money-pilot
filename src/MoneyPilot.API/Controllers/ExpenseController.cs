using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyPilot.Application.DTOs;
using MoneyPilot.Application.Interfaces;
using System.Security.Claims;

namespace MoneyPilot.API.Controllers
{
    [Authorize] // 🔒 All endpoints require JWT
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        /// <summary>
        /// Extract logged-in user's Id from JWT
        /// </summary>
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("UserId missing in JWT token");
        }

        // GET: api/Expense
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var expenses = await _expenseService.GetAllAsync(userId);
            return Ok(expenses);
        }

        // GET: api/Expense/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            var expense = await _expenseService.GetByIdAsync(id, userId);

            if (expense == null)
                return NotFound();

            return Ok(expense);
        }

        //// POST: api/Expense
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] ExpenseDto dto)
        //{
        //    var userId = GetUserId();
        //    var created = await _expenseService.CreateAsync(dto, userId);

        //    return CreatedAtAction(
        //        nameof(GetById),
        //        new { id = created.Id },
        //        created
        //    );
        //}
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExpenseDto dto)
        {
            var userId = GetUserId();

            await _expenseService.CreateAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetAll),
                null,
                "Expense created successfully"
            );
        }


        // PUT: api/Expense/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExpenseDto dto)
        {
            var userId = GetUserId();
            var updated = await _expenseService.UpdateAsync(id, dto, userId);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Expense/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _expenseService.DeleteAsync(id, userId);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        // 🔍 Debug endpoint (temporary)
        [AllowAnonymous]
        [HttpGet("debug")]
        public IActionResult Debug()
        {
            return Ok(new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated,
                Claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
    }
}
