using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoneyPilot.Application.Common;
using MoneyPilot.Application.DTOs;
using MoneyPilot.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoneyPilot.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RecurringTransactionsController : ControllerBase
    {
        private readonly IRecurringTransactionService _recurringTransactionService;
        private readonly ILogger<RecurringTransactionsController> _logger;

        public RecurringTransactionsController(
            IRecurringTransactionService recurringTransactionService,
            ILogger<RecurringTransactionsController> logger)
        {
            _recurringTransactionService = recurringTransactionService;
            _logger = logger;
        }

        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<RecurringTransactionDto>>> GetAll()
        // {
        //     try
        //     {
        //         var userId = GetUserId();
        //         var transactions = await _recurringTransactionService.GetAllAsync(userId);
        //         return Ok(transactions);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error getting all recurring transactions for user {UserId}", GetUserId());
        //         return StatusCode(500, "An error occurred while retrieving recurring transactions.");
        //     }
        // }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                
            var userId = GetUserId();
            var transactions = await _recurringTransactionService.GetAllAsync(userId);
            var list = transactions.ToList();

            var paged = new PagedResponse<RecurringTransactionDto>
            {
                Items = list,
                TotalCount = list.Count,
                Page = 1,
                PageSize = list.Count
            };
            // return Ok(ApiResponse<IEnumerable<RecurringTransactionDto>>
            //  .SuccessResponse(transactions));
            return Ok(ApiResponse<PagedResponse<RecurringTransactionDto>>.SuccessResponse(paged));
            }
            catch (Exception ex)
            {
            _logger.LogError(ex, "Error getting all recurring transactions for user {UserId}", GetUserId());
             return StatusCode(500, "An error occurred while retrieving recurring transactions.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecurringTransactionDto>> GetById(int id)
        {
            try
            {
                var userId = GetUserId();
                var transaction = await _recurringTransactionService.GetByIdAsync(id, userId);
                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recurring transaction {TransactionId} for user {UserId}",
                    id, GetUserId());
                return StatusCode(500, "An error occurred while retrieving the recurring transaction.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<RecurringTransactionDto>> Create(CreateRecurringTransactionDto dto)
        {
            try
            {
                var userId = GetUserId();
                var created = await _recurringTransactionService.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating recurring transaction for user {UserId}", GetUserId());
                return StatusCode(500, "An error occurred while creating the recurring transaction.");
            }
        }

        //Update the recurring expense
        [HttpPut("{id}")]
        public async Task<ActionResult<RecurringTransactionDto>> Update(int id,UpdateRecurringTransactionDto dto)
        {
            try {
                var userId = GetUserId();
                var updated = await _recurringTransactionService.UpdateAsync(id, dto, userId);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating recurring transaction {TransactionId} for user {UserId}",
            id, GetUserId());
                return StatusCode(500, "An error occurred while updating the recurring transaction.");
            }

        }

        //Delete the recurring Expense
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try {
                var userId = GetUserId();
                await _recurringTransactionService.DeleteAsync(id, userId);
                return NoContent(); 
            }
            catch(KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error Deleting recurring transaction {TransactionId} for user {UserId}",
            id, GetUserId());
                return StatusCode(500, "An error occurred while updating the recurring transaction.");
            }
        }
        //Get the Due Expense of User
        [HttpGet("due")]
        public async Task<ActionResult<IEnumerable<RecurringTransactionDto>>> GetDueTransactions()
        {
            try
            {
                var userId = GetUserId();
                var dueTransactions = await _recurringTransactionService.GetDueTransactionsAsync(userId);
                return Ok(dueTransactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting due recurring transactions for user {UserId}", GetUserId());
                return StatusCode(500, "An error occurred while retrieving due recurring transactions.");
            }
        }

        //Add a due Expense
        [HttpPost("process-due")]
        public async Task<ActionResult> ProcessDueTransactions()
        {
            try
            {
                var userId = GetUserId();
                var processedCount = await _recurringTransactionService.ProcessDueTransactionsAsync();
                return Ok(new
                {
                    message = $"Processed {processedCount} recurring transactions",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing due recurring transactions");
                return StatusCode(500, "An error occurred while processing recurring transactions.");
            }
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token");
        }
    }
}