using Microsoft.AspNetCore.Mvc;
using TimesheetSystem.Interfaces;
using TimesheetSystem.Models;

namespace TimesheetSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimesheetController : Controller
{
    private readonly ITimesheetRepository _repository;

    public TimesheetController(ITimesheetRepository repository)
    {
        _repository = repository;
    }
    
    [HttpPost]
    public IActionResult AddTimesheet(TimesheetEntry timesheet)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var newTimesheet = _repository.AddTimesheet(timesheet);
        return CreatedAtAction(nameof(GetTimesheetById), new { id = newTimesheet.Id }, newTimesheet);
    }

    [HttpGet]
    public IActionResult GetTimesheetById(Guid timesheetId)
    {
        var timeSheet = _repository.GetById(timesheetId);
        if (timeSheet == null)
            return NotFound();
        
        return Ok(timeSheet);
    }
    
    [HttpPut("{id:guid}")]
    public IActionResult UpdateTimesheet(Guid id, [FromBody] TimesheetEntry timesheet)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        timesheet.Id = id;
        var updatedTimesheet = _repository.UpdateTimesheet(timesheet);
        if (updatedTimesheet == null)
        {
            return NotFound();
        }

        return Ok(updatedTimesheet);
    }
    
    [HttpDelete("{id:guid}")]
    public IActionResult DeleteTimesheet(Guid id)
    {
        var deleted = _repository.DeleteTimesheet(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
    
    [HttpGet("userId/{userId:guid}/weekStart/{weekStartDate}")]
    public IActionResult GetTimesheetByUserAndWeek(Guid userId, string weekStartDate)
    {
        if (!DateOnly.TryParse(weekStartDate, out var weekStart))
        {
            return BadRequest("Invalid weekStartDate format. Use YYYY-MM-DD.");
        }

        try
        {
            var results = _repository.GetByUserAndWeek(userId, weekStart);
            return Ok(results);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}