using Microsoft.AspNetCore.Mvc;
using MoneyPilot.Application.DTOs;
using MoneyPilot.API.Controllers;
using MoneyPilot.Application.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MoneyPilot.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController : ControllerBase
    {

        //inject budget service
        private readonly IBudgetService _budgetService;
        /* Explanation:
         The BudgetController class is an API controller that handles HTTP requests related to budget operations.
         It uses dependency injection to receive an instance of IBudgetService, which contains the business logic for managing budgets.
         The controller defines several endpoints for CRUD operations on budgets, such as getting all budgets for a user, getting a specific budget by ID, creating a new budget, updating an existing budget, and deleting a budget.
        */

        public BudgetController(IBudgetService budgetService)
        {
            //what is happening here?
            // This is constructor injection. The framework will automatically provide an instance of IBudgetService when creating an instance of BudgetController.
            _budgetService = budgetService;
        }


        // GET: api/<BudgetController>
        [HttpGet]
        public async Task<IActionResult> GetAll(string userId)
        {
            var Budgets = await _budgetService.GetAllAsync(userId);
            return Ok(Budgets);
        }

        // GET api/<BudgetController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var Budget = await _budgetService.GetByIdAsync(id);
            return Budget == null ? NotFound() : Ok(Budget);
        }

        // POST api/<BudgetController>
        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] string userId, [FromBody] BudgetDto dto)
        {
            await _budgetService.AddAsync(dto, userId);
            return CreatedAtAction(nameof(GetAll),new {userId = userId },dto);
        }

        // PUT api/<BudgetController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BudgetDto dto)
        {
            var success = await _budgetService.UpdateAsync(id, dto);
            return success ? NoContent() : NotFound();
        }

        // DELETE api/<BudgetController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _budgetService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
