using System.ComponentModel.DataAnnotations;

namespace TimesheetSystem.Models;

public class TimesheetEntry
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid? UserId { get; set; }
    
    [Required]
    public Guid? ProjectId { get; set; }
    
    [Required]
    [NotInFuture]
    public DateOnly? Date { get; set; }
    
    [Required]
    [Range(0.1, 24.0, ErrorMessage = "Hours Worked must be greater than zero and less than 24 hours")]
    public Decimal HoursWorked { get; set; }
    
    public string? Description { get; set; }
}

//custom validation for not in the future (would be better to move this to a validation directory or something similar)
public class NotInFutureAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        if (value is DateOnly date)
        {
            return date <= DateOnly.FromDateTime(DateTime.Today);
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} cannot be in the future.";
    }
}