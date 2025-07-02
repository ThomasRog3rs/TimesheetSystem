namespace TimesheetSystem.Models;

public class TimesheetEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProjectId { get; set; }
    public DateOnly Date { get; set; }
    public Decimal HoursWorked { get; set; }
    public string? Description { get; set; }
}