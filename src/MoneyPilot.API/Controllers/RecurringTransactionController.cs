using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoneyPilot.Application.DTOs;
using MoneyPilot.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoneyPilot.API.Controllers
{
    [Authorize]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecurringTransactionDto>>> GetAll()
        {
            try
            {
                var userId = GetUserId();
                var transactions = await _recurringTransactionService.GetAllAsync(userId);
                return Ok(transactions);
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

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found in token");
        }
    }
}