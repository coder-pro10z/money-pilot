using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using MoneyPilot.Application.Interfaces;
using MoneyPilot.API.Controllers;
using MoneyPilot.Application.DTOs;
namespace MoneyPilot.Tests.Controller
{
    //    public class ExpenseContollerTests
    //    {
    //        [Fact]
    //        public async Task GetAll_ReturnsOkResult_WithExpenses()
    //        {
    //            // Arrange
    //            var mockService = new Mock<IExpenseService>();
    //            //Setup MockService methods and properties here
    //            mockService.Setup(s=> s.GetAllAsync("test-user"))
    //                                    .ReturnsAsync(new List<ExpenseResponseDto>
    //                                    {
    //                                        new ExpenseResponseDto { Id = 1,
    //                                            Amount = 100,
    //                                            Description = "Test Expense 1",
    //                                            CategoryId=1,
    //                                            CategoryName="Food" },
    //                                        //new ExpenseDto { Id = 2, Amount = 200, Description = "Test Expense 2" }
    //                                    }
    //                );
    //            //controller
    //            //passing mockService object setup to controller
    //            //var controller = new ExpenseController(mockService.Object);

    //            //// Act
    //            ////result from controller method
    //            ////var result = await controller.GetAll("test-user");
    //            //var result = await controller.GetAll();

    //            //// Assert
    //            ////var okResult=Assert.IsType<OkObjectResult>(result);
    //            ////var returnValue = Assert.IsAssignableFrom<IEnumerable<ExpenseResponseDto>>(okResult);

    //            //var okResult = Assert.IsType<OkObjectResult>(result); // This asserts the type
    //            //var returnValue = Assert.IsAssignableFrom<IEnumerable<ExpenseResponseDto>>(okResult.Value); // This inspects the Value property inside the HTTP response
    //            //Assert.Single(returnValue);

    //        }

    //    }
}
