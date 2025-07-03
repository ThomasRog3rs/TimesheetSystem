using TimesheetSystem.Models;

namespace TimesheetSystem.Interfaces;

public interface ITimesheetRepository
{
    TimesheetEntry AddTimesheet(TimesheetEntry timesheet);
    TimesheetEntry? UpdateTimesheet(TimesheetEntry timesheet);
    List<TimesheetEntry> GetByUserAndWeek(Guid userId, DateOnly weekStartDate);
    TimesheetEntry GetById(Guid id);
    void DeleteTimesheet(Guid id);
}