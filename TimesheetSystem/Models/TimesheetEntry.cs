using System.ComponentModel.DataAnnotations;

namespace TimesheetSystem.Models;

public class TimesheetEntry
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid ProjectId { get; set; }
    
    [Required]
    public DateOnly Date { get; set; }
    
    [Required]
    [Range(0.1, 24.0, ErrorMessage = "Hours Worked must be greater than zero and less than 24 hours")]
    public Decimal HoursWorked { get; set; }
    
    public string? Description { get; set; }
}