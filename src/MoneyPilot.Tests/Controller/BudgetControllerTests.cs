using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.API.Controllers;
using MoneyPilot.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MoneyPilot.Tests.Controller
{
    public class BudgetControllerTests
    {
        // Add your test methods here

        //for BudgetController
        [Fact]
        public async Task GetAllAsyncBudget() {
            // Arrange
            // Mock dependencies
            //services like UserManager, BudgetRepository, etc.
            //mock service BudgetService
            var mockBudgetService = new Mock<IBudgetService>();
            //setup mock methods if needed
            //using .setup() method of Moq
            mockBudgetService.Setup(s => s.GetAllAsync("test-user"))
                .ReturnsAsync(new List<BudgetResponseDto>
                { new BudgetResponseDto
                    {
                        Id = 1,
                        MonthlyLimit = 500,
                        Month = new DateTime(2024, 6, 1),
                        CategoryId = 1,
                        //CategoryName = "Groceries"
                    }
                });

            //Controller instantiation         
            var controller = new BudgetController(mockBudgetService.Object);
            // Act
            var result =await controller.GetAll();
            // Call the method you want to test here
            // Assert
            // Add your assertions here
            //type of result
            var okResult = Assert.IsType<OkObjectResult>(result);
            var budgets = Assert.IsAssignableFrom<IEnumerable<BudgetResponseDto>>(okResult.Value);
            //if single budget returned
            Assert.Single(budgets);
        }
        }
    }
