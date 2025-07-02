using TimesheetSystem.Interfaces;
using TimesheetSystem.Models;

namespace TimesheetSystem.Repositories;

public class InMemoryTimesheetRepository : ITimesheetRepository
{
    private static readonly List<TimesheetEntry> _timesheetEntries = new();

    public TimesheetEntry AddTimesheet(TimesheetEntry timesheet)
    {
        timesheet.Id = Guid.NewGuid();
        _timesheetEntries.Add(timesheet);
        return timesheet;
    }

    public TimesheetEntry UpdateTimesheet(TimesheetEntry timesheet)
    {
        throw new NotImplementedException();
    }

    public List<TimesheetEntry> GetByUserAndWeek(Guid userId, DateOnly weekStartDate)
    {
        throw new NotImplementedException();
    }

    public TimesheetEntry GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public void DeleteTimesheet(Guid id)
    {
        throw new NotImplementedException();
    }
}