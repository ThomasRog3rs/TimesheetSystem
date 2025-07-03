using TimesheetSystem.Models;

namespace TimesheetSystem.Interfaces;

public interface ITimesheetRepository
{
    TimesheetEntry AddTimesheet(TimesheetEntry timesheet);
    TimesheetEntry? UpdateTimesheet(TimesheetEntry timesheet);
    List<TimesheetEntry> GetByUserAndWeek(Guid userId, DateOnly weekStartDate);
    Dictionary<Guid, decimal> GetTotalHoursPerProject(Guid userId, DateOnly weekStartDate);
    TimesheetEntry? GetById(Guid id);
    bool DeleteTimesheet(Guid id);
}