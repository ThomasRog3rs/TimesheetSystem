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
}