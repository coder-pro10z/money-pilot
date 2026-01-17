using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using MoneyPilot.Application.DTOs;
using MoneyPilot.Application.Interfaces;
using System.Runtime.CompilerServices;

namespace MoneyPilot.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        //inject expense service
        private readonly IExpenseService _expenseService;
        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }


        // GET: api/<Controller>?userid=test_user
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string userId)
        {
            var expenses= await _expenseService.GetAllAsync(userId);
            return Ok(expenses);
        }

        // GET api/<ExpenseController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var expense= await _expenseService.GetByIdAsync(id);
            return expense == null ? NotFound() : Ok(expense);
        }

        // POST api/<ExpensesController>    
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExpenseDto dto,[FromQuery] string userId)
        {
            await _expenseService.AddAsync(dto, userId);
            return CreatedAtAction(nameof(GetAll), new { userId = userId }, dto);
        }

        // PUT api/<ExpensesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExpenseDto dto)
        {
            var success = await _expenseService.UpdateAsync(id, dto);
            return success ? NoContent(): NotFound();
        }

        // DELETE api/<ExpensesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _expenseService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
