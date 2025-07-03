using TimesheetSystem.Models;
using TimesheetSystem.Repositories;

namespace TimesheetSystem.Tests;

public class InMemoryTimesheetRepositoryTests
{
    private TimesheetEntry CreateEntry(Guid userId, DateOnly date, decimal hours = 8)
    {
        return new TimesheetEntry
        {
            UserId = userId,
            ProjectId = Guid.NewGuid(),
            Date = date,
            HoursWorked = hours
        };
    }
    
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
    
    [Fact]
    public void DeleteTimesheet_RemovesExistingEntry()
    {
        // Arrange
        var repo = new InMemoryTimesheetRepository();
        var timesheet = new TimesheetEntry
        {
            UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            ProjectId = Guid.Parse("1fb99167-3c45-4116-a75a-0e131b69b7cf"),
            Date = DateOnly.FromDateTime(DateTime.Today),
            HoursWorked = 7.5m,
            Description = "To be deleted"
        };
        var added = repo.AddTimesheet(timesheet);

        // Act
        var deleted = repo.DeleteTimesheet(added.Id);
        var fetched = repo.GetById(added.Id);

        // Assert
        Assert.True(deleted);
        Assert.Null(fetched);
    }

    [Fact]
    public void DeleteTimesheet_ReturnsFalse_WhenEntryDoesNotExist()
    {
        // Arrange
        var repo = new InMemoryTimesheetRepository();
        var nonExistentId = Guid.NewGuid();

        // Act
        var deleted = repo.DeleteTimesheet(nonExistentId);

        // Assert
        Assert.False(deleted);
    }
    
    [Fact]
    public void GetByUserAndWeek_ReturnsEntriesForUserAndWeek()
    {
        //arange
        var repo = new InMemoryTimesheetRepository();
        var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        var otherUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var weekStart = new DateOnly(2024, 7, 1); // Monday
        var weekEnd = weekStart.AddDays(6); // Sunday

        var entry1 = CreateEntry(userId, weekStart);
        var entry2 = CreateEntry(userId, weekStart.AddDays(3), 6);
        var entry3 = CreateEntry(userId, weekEnd.AddDays(1), 5); // Next Monday
        var entry4 = CreateEntry(otherUserId, weekStart, 7);

        repo.AddTimesheet(entry1);
        repo.AddTimesheet(entry2);
        repo.AddTimesheet(entry3);
        repo.AddTimesheet(entry4);

        //actions
        var results = repo.GetByUserAndWeek(userId, weekStart);
        
        Assert.Contains(results, e => e.Id == entry1.Id);
        Assert.Contains(results, e => e.Id == entry2.Id);
        Assert.DoesNotContain(results, e => e.Id == entry3.Id);
        Assert.DoesNotContain(results, e => e.Id == entry4.Id);
        Assert.Equal(2, results.Count);
    }
}