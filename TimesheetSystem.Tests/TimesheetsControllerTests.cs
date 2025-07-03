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
        
        [Fact]
        public void AddEntry_ZeroHours_ReturnsBadRequest()
        {
            var mockRepo = new Mock<ITimesheetRepository>();
            
            var timesheet = new TimesheetEntry
            {
                UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                ProjectId = Guid.Parse("1fb99167-3c45-4116-a75a-0e131b69b7cf"),
                Date = DateOnly.FromDateTime(DateTime.Today),
                HoursWorked = 0.0m,
                Description = "Worked on feature X"
            };
            
            var controller = new TimesheetController(mockRepo.Object);
            ValidateModel(timesheet, controller);
            
            var res = controller.AddTimesheet(timesheet);
            var badRequestRes = Assert.IsType<BadRequestObjectResult>(res);

            var errors = badRequestRes.Value as SerializableError;
            Assert.NotNull(errors);
            Assert.True(errors.ContainsKey(nameof(TimesheetEntry.HoursWorked)));
            var errorMessages = errors[nameof(TimesheetEntry.HoursWorked)] as string[];
            Assert.Contains("Hours Worked must be greater than zero and less than 24 hours", errorMessages);
        }
        
        [Fact]
        public void AddEntry_MissingUserId_ReturnsBadRequest()
        {
            var mockRepo = new Mock<ITimesheetRepository>();
            var timesheet = new TimesheetEntry
            {
                //UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6") - missing user id
                ProjectId = Guid.Parse("1fb99167-3c45-4116-a75a-0e131b69b7cf"),
                Date = DateOnly.FromDateTime(DateTime.Today),
                HoursWorked = 7.5m
            };
            
            var controller = new TimesheetController(mockRepo.Object);
            ValidateModel(timesheet, controller);

            var res = controller.AddTimesheet(timesheet);

            Assert.IsType<BadRequestObjectResult>(res);
        }
        
        [Fact]
        public void GetTimesheetById_ExistingId_ReturnsOk()
        {
            //Arrange
            var mockRepo = new Mock<ITimesheetRepository>();
            var id = Guid.NewGuid();
            var timesheet = new TimesheetEntry { Id = id };
            mockRepo.Setup(r => r.GetById(id)).Returns(timesheet);

            var controller = new TimesheetController(mockRepo.Object);

            //action
            var result = controller.GetTimesheetById(id);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(timesheet, okResult.Value);
        }
        
        
        [Fact]
        public void GetTimesheetById_NonExistingId_ReturnsNotFound()
        {
            var mockRepo = new Mock<ITimesheetRepository>();
            var id = Guid.NewGuid();
            mockRepo.Setup(r => r.GetById(id)).Returns((TimesheetEntry)null);

            var controller = new TimesheetController(mockRepo.Object);
            
            var result = controller.GetTimesheetById(id);
            
            Assert.IsType<NotFoundResult>(result);
        }
        
        [Fact]
        public void UpdateTimesheet_ValidUpdate_ReturnsOk()
        {
            var mockRepo = new Mock<ITimesheetRepository>();
            var id = Guid.NewGuid();
            var timesheet = new TimesheetEntry
            {
                UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                ProjectId = Guid.Parse("1fb99167-3c45-4116-a75a-0e131b69b7cf"),
                Date = DateOnly.FromDateTime(DateTime.Today),
                HoursWorked = 7.5m,
                Description = "Worked on feature X"
            };
            
            mockRepo.Setup(repo => repo.UpdateTimesheet(timesheet)).Returns(timesheet);

            var controller = new TimesheetController(mockRepo.Object);
            ValidateModel(timesheet, controller);
            
            var result = controller.UpdateTimesheet(id, timesheet);
            
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(timesheet, okResult.Value);
        }
        
        [Fact]
        public void UpdateTimesheet_NotFound_ReturnsNotFound()
        {
            var mockRepo = new Mock<ITimesheetRepository>();
            var id = Guid.NewGuid();
            var timesheet = new TimesheetEntry
            {
                UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                ProjectId = Guid.Parse("1fb99167-3c45-4116-a75a-0e131b69b7cf"),
                Date = DateOnly.FromDateTime(DateTime.Today),
                HoursWorked = 7.5m,
                Description = "Worked on feature X"
            };
            
            mockRepo.Setup(r => r.UpdateTimesheet(timesheet)).Returns((TimesheetEntry)null);

            var controller = new TimesheetController(mockRepo.Object);
            ValidateModel(timesheet, controller);
            
            var result = controller.UpdateTimesheet(id, timesheet);
            
            Assert.IsType<NotFoundResult>(result);
        }
        
        [Fact]
        public void GetByUserAndWeek_ValidRequest_ReturnsOk()
        {
            // Arrange
            var mockRepo = new Mock<ITimesheetRepository>();
            var userId = Guid.NewGuid();
            var weekStart = new DateOnly(2024, 7, 1); // Monday
            var entries = new List<TimesheetEntry>
            {
                new TimesheetEntry { Id = Guid.NewGuid(), UserId = userId, Date = weekStart }
            };
            mockRepo.Setup(r => r.GetByUserAndWeek(userId, weekStart)).Returns(entries);

            var controller = new TimesheetController(mockRepo.Object);

            // Act
            var result = controller.GetTimesheetByUserAndWeek(userId, weekStart.ToString("yyyy-MM-dd"));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(entries, okResult.Value);
        }

        [Fact]
        public void GetByUserAndWeek_InvalidDate_ReturnsBadRequest()
        {
            // Arrange
            var mockRepo = new Mock<ITimesheetRepository>();
            var userId = Guid.NewGuid();
            var controller = new TimesheetController(mockRepo.Object);

            // Act
            var result = controller.GetTimesheetByUserAndWeek(userId, "not-a-date");

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid weekStartDate format. Use YYYY-MM-DD.", badRequest.Value);
        }

        [Fact]
        public void GetByUserAndWeek_ArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var mockRepo = new Mock<ITimesheetRepository>();
            var userId = Guid.NewGuid();
            var weekStart = new DateOnly(2024, 7, 2); // Not a Monday
            mockRepo.Setup(r => r.GetByUserAndWeek(userId, weekStart))
                .Throws(new ArgumentException("weekStartDate must be a Monday.", "weekStartDate"));

            var controller = new TimesheetController(mockRepo.Object);

            // Act
            var result = controller.GetTimesheetByUserAndWeek(userId, weekStart.ToString("yyyy-MM-dd"));

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("weekStartDate must be a Monday. (Parameter 'weekStartDate')", badRequest.Value);
        }
    }
}