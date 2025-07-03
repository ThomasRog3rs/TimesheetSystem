using TimesheetSystem.Models;
using TimesheetSystem.Repositories;

namespace TimesheetSystem.Tests;

public class InMemoryTimesheetRepositoryTests
{
    [Fact]
    public void AddTimesheetToList()
    {
        //Arrang
        var repo = new InMemoryTimesheetRepository();
        var timesheet = new TimesheetEntry
        {
            UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            ProjectId = Guid.Parse("1fb99167-3c45-4116-a75a-0e131b69b7cf"),
            Date = DateOnly.FromDateTime(DateTime.Today),
            HoursWorked = 7.5m
        };
        
        //act
        var res = repo.AddTimesheet(timesheet);
        var value = repo.GetById(res.Id);
        
        //Asserts
        Assert.NotNull(value);
        Assert.Equal(Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), value.UserId);
        Assert.Equal(timesheet.ProjectId, value.ProjectId);
        Assert.Equal(timesheet.HoursWorked, value.HoursWorked);
    }
    
    [Fact]
    public void UpdateTimesheet_UpdatesExistingEntry()
    {
        //arrange
        var repo = new InMemoryTimesheetRepository();
        var original = new TimesheetEntry
        {
            UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            ProjectId = Guid.Parse("1fb99167-3c45-4116-a75a-0e131b69b7cf"),
            Date = DateOnly.FromDateTime(DateTime.Today),
            HoursWorked = 7.5m,
            Description = "Initial"
        };
        var added = repo.AddTimesheet(original);

        //action
        var updatedEntry = new TimesheetEntry
        {
            Id = added.Id,
            UserId = added.UserId,
            ProjectId = added.ProjectId,
            Date = added.Date,
            HoursWorked = 8.5m,
            Description = "Updated"
        };
        var res = repo.UpdateTimesheet(updatedEntry);
        var fetched = repo.GetById(added.Id);

        //Assert
        Assert.NotNull(res);
        Assert.Equal(8.5m, fetched?.HoursWorked);
        Assert.Equal("Updated", fetched?.Description);
    }
    
    //Edge case test
    [Fact]
    public void UpdateTimesheet_ReturnsNull_WhenEntryDoesNotExist()
    {
        // Arrange
        var repo = new InMemoryTimesheetRepository();
        var nonExistentId = Guid.NewGuid();
        var updateAttempt = new TimesheetEntry
        {
            Id = nonExistentId,
            UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            ProjectId = Guid.Parse("1fb99167-3c45-4116-a75a-0e131b69b7cf"),
            Date = DateOnly.FromDateTime(DateTime.Today),
            HoursWorked = 5.0m,
            Description = "Should not update"
        };

        // Act
        var res = repo.UpdateTimesheet(updateAttempt);

        // Assert
        Assert.Null(res);
    }
}