using CANTEEN_SYSTEM.Contracts;
using CANTEEN_SYSTEM.Data;
using CANTEEN_SYSTEM.Data.Entities;
using CANTEEN_SYSTEM.Extensions;
using CANTEEN_SYSTEM.Services.Sync;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CANTEEN_SYSTEM.Controllers.Api;

[ApiController]
[Route("api/staff")]
public class StaffController(CanteenDbContext db, SyncQueueService syncQueue) : ControllerBase

{
    
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // 2. Update the Login method (around Line 16)
    [HttpPost("login")]
    public async Task<ActionResult<EmployeeDto>> Login([FromBody] LoginRequest request)
    {
        // We use the Username to search the QrCode column where ADMIN123 is stored
        var employee = await db.Employees.FirstOrDefaultAsync(item =>
            item.QrCode == request.Username.Trim().ToUpper());

        // We use the Password to verify against the stored hash
        if (employee is null || !BCrypt.Net.BCrypt.Verify(request.Password.Trim(), employee.PinHash))
        {
            return Unauthorized(new { message = "Invalid Username or Password." });
        }

        return Ok(employee.ToDto());
    }

    [HttpGet("employees")]
    public async Task<ActionResult<IReadOnlyList<EmployeeDto>>> GetEmployees()
    {
        var employees = await db.Employees
            .OrderBy(item => item.CreatedAt)
            .ToListAsync();

        return Ok(employees.Select(item => item.ToDto()).ToList());
    }

    [HttpPost("employees")]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        var actor = await db.Employees.FirstOrDefaultAsync(item => item.Id == request.ActorEmployeeId);
        if (actor is null || !string.Equals(actor.Role, "admin", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins can manage employees." });
        }

        var pin = request.Pin.Trim();
        var name = request.Name.Trim();
        var role = request.Role.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(pin))
        {
            return BadRequest(new { message = "Name and PIN are required." });
        }

        if (pin.Length < 4 || pin.Length > 20)
        {
            return BadRequest(new { message = "Password must be between 4 and 20 characters." });
        }

        if (role is not ("admin" or "cashier"))
        {
            return BadRequest(new { message = "Role must be admin or cashier." });
        }

        var employee = new Employee
        {
            SyncId = Guid.NewGuid().ToString("N"),
            Name = name,
            PinHash = BCrypt.Net.BCrypt.HashPassword(pin),
            Role = role,
            QrCode = $"EMP{DateTime.UtcNow.Ticks.ToString()[^6..]}",
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow
        };

        db.Employees.Add(employee);
        await syncQueue.QueueUpsertAsync(db, "employee", employee.SyncId!, employee.LastModifiedAt!.Value);
        await db.SaveChangesAsync();

        return Created(string.Empty, employee.ToDto());
    }

    [HttpDelete("employees/{id:int}")]
    public async Task<IActionResult> DeleteEmployee(int id, [FromQuery] int actorEmployeeId)
    {
        var actor = await db.Employees.FirstOrDefaultAsync(item => item.Id == actorEmployeeId);
        if (actor is null || !string.Equals(actor.Role, "admin", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins can manage employees." });
        }

        if (actor.Id == id)
        {
            return BadRequest(new { message = "You cannot delete the currently logged in admin." });
        }

        var employee = await db.Employees.FirstOrDefaultAsync(item => item.Id == id);
        if (employee is null)
        {
            return NotFound();
        }

        employee.SyncId ??= Guid.NewGuid().ToString("N");
        await syncQueue.QueueDeleteAsync(db, "employee", employee.SyncId);
        db.Employees.Remove(employee);
        await db.SaveChangesAsync();
        return NoContent();
    }
}