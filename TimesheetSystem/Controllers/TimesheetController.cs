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
    public IActionResult AddEntry(TimesheetEntry timesheet)
    {
        var newTimesheet = _repository.AddTimesheet(timesheet);
        return CreatedAtAction(nameof(GetTimesheetById), new { id = newTimesheet.Id }, newTimesheet);
    }

    [HttpGet]
    public IActionResult GetTimesheetById(Guid timesheetId)
    {
        var timeSheet = _repository.GetById(timesheetId);
        return Ok(timeSheet);
    }
}