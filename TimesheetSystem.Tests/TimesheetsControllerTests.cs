using System.ComponentModel.DataAnnotations;
using Xunit;
using Moq;

using TimesheetSystem.Controllers;
using TimesheetSystem.Models;
using TimesheetSystem.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace TimesheetSystem.Tests
{
    public class TimesheetsControllerTests
    {
        //When making sure the test works as expected the validator was not working, realised I need to manually run the validator
        public static void ValidateModel(object model, Controller controller)
        {
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);

            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                }
            }
        }
        
        [Fact]
        public void AddTimesheet_ValidEntry_ReturnsCreatedAtAction()
        {
            //Arrange
            var mockRepo = new Mock<ITimesheetRepository>();
            var timesheet = new TimesheetEntry
            {
                UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                ProjectId = Guid.Parse("1fb99167-3c45-4116-a75a-0e131b69b7cf"),
                Date = DateOnly.FromDateTime(DateTime.Today),
                HoursWorked = 7.5m,
                Description = "Worked on feature X"
            };
            
            mockRepo.Setup(repo => repo.AddTimesheet(It.IsAny<TimesheetEntry>()))
                .Returns((TimesheetEntry ts) =>
                {
                    ts.Id = Guid.NewGuid(); 
                    return ts;
                });
            
            var controller = new TimesheetController(mockRepo.Object);
            ValidateModel(timesheet, controller);

            //action
            var result = controller.AddTimesheet(timesheet);

            //asserts
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedEntry = Assert.IsType<TimesheetEntry>(createdResult.Value);
            
            Assert.Equal(timesheet.UserId, returnedEntry.UserId);
            Assert.Equal(timesheet.ProjectId, returnedEntry.ProjectId);
            Assert.Equal(timesheet.HoursWorked, returnedEntry.HoursWorked);
            Assert.NotEqual(Guid.Empty, returnedEntry.Id);
        }
    }
}