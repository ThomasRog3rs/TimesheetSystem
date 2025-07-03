using TimesheetSystem.Interfaces;
using TimesheetSystem.Models;

namespace TimesheetSystem.Repositories;

public class InMemoryTimesheetRepository : ITimesheetRepository
{
    private static readonly List<TimesheetEntry> _timesheetEntries = new();

    public TimesheetEntry AddTimesheet(TimesheetEntry timesheet)
    {
        timesheet.Id = Guid.NewGuid();
        timesheet.HoursWorked = Math.Round(timesheet.HoursWorked, 2);
        _timesheetEntries.Add(timesheet);
        return timesheet;
    }

    public TimesheetEntry? UpdateTimesheet(TimesheetEntry timesheet)
    {
        var existingEntry = _timesheetEntries.Find(entry => entry.Id == timesheet.Id);
        if (existingEntry == null)
        {
            return null;
        }
        
        existingEntry.UserId = timesheet.UserId;
        existingEntry.ProjectId = timesheet.ProjectId;
        existingEntry.Date = timesheet.Date;
        existingEntry.HoursWorked = Math.Round(timesheet.HoursWorked, 2);
        existingEntry.Description = timesheet.Description;

        return existingEntry;
    }
    
    public List<TimesheetEntry> GetByUserAndWeek(Guid userId, DateOnly weekStartDate)
    {
        if (weekStartDate.DayOfWeek != DayOfWeek.Monday)
        {
            throw new ArgumentException("weekStartDate must be a Monday.", nameof(weekStartDate));
        }

        var weekEndDate = weekStartDate.AddDays(6);

        if (weekEndDate.DayOfWeek != DayOfWeek.Sunday)
        {
            throw new ArgumentException("week does not end on a Sunday.", nameof(weekEndDate));
        }

        return _timesheetEntries
            .Where(entry =>
                entry.UserId == userId &&
                entry.Date >= weekStartDate &&
                entry.Date <= weekEndDate)
            .ToList();
    }
    
    public Dictionary<Guid, decimal> GetTotalHoursPerProject(Guid userId, DateOnly weekStartDate)
    {
        if (weekStartDate.DayOfWeek != DayOfWeek.Monday)
        {
            throw new ArgumentException("weekStartDate must be a Monday.", nameof(weekStartDate));
        }

        var weekEndDate = weekStartDate.AddDays(6);

        if (weekEndDate.DayOfWeek != DayOfWeek.Sunday)
        {
            throw new ArgumentException("week does not end on a Sunday.", nameof(weekEndDate));
        }

        return _timesheetEntries
            .Where(entry =>
                entry.UserId == userId &&
                entry.Date >= weekStartDate &&
                entry.Date <= weekEndDate &&
                entry.ProjectId != null)
            .GroupBy(entry => entry.ProjectId!.Value)
            .ToDictionary(
                g => g.Key,
                g => Math.Round(g.Sum(e => e.HoursWorked), 2)
            );
    }

    public TimesheetEntry? GetById(Guid id)
    {
        return _timesheetEntries.Find(entry => entry.Id == id);
    }

    public bool DeleteTimesheet(Guid id)
    {
        var entry = _timesheetEntries.Find(e => e.Id == id);
        if (entry == null)
            return false;
        
        _timesheetEntries.Remove(entry);
        return true;
    }
}