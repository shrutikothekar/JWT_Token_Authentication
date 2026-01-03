using doctor_app_api.Data;
using doctor_app_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doctor_app_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext db;
        public AppointmentsController(AppDbContext db) => this.db = db;

        // GET: api/appointments/doctor/{doctorId}?from=2025-08-24&to=2025-08-31
        [HttpGet("doctor/{doctorId:guid}")]
        public async Task<IActionResult> GetForDoctor(Guid doctorId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
        {
            var q = db.Appointments.Where(a => a.DoctorId == doctorId);

            if (from.HasValue)
            {
                var f = from.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
                q = q.Where(a => a.StartUtc >= f);
            }
            if (to.HasValue)
            {
                var t = to.Value.ToDateTime(new TimeOnly(23, 59, 59), DateTimeKind.Utc);
                q = q.Where(a => a.StartUtc <= t);
            }

            var list = await q
                .OrderBy(a => a.StartUtc)
                .Select(a => new {
                    a.Id,
                    a.DoctorId,
                    a.PatientId,
                    a.ClinicId,
                    a.StartUtc,
                    a.EndUtc,
                    Status = a.Status.ToString()
                })
                .ToListAsync();

            return Ok(list);
        }

        // POST: api/appointments/book
        [HttpPost("book")]
        public async Task<IActionResult> Book([FromBody] Appointment appt)
        {
            // Simple server-side conflict guard (DB unique index is the final safeguard)
            bool conflict = await db.Appointments.AnyAsync(a =>
                a.DoctorId == appt.DoctorId &&
                a.StartUtc < appt.EndUtc &&
                appt.StartUtc < a.EndUtc
            );
            if (conflict) return Conflict("Slot already booked");

            appt.Id = Guid.NewGuid();
            appt.Status = AppointmentStatus.Confirmed; // or Pending if you prefer
            db.Appointments.Add(appt);
            await db.SaveChangesAsync();
            return Ok(appt);
        }

        // PUT: api/appointments/{id}/cancel
        [HttpPut("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var a = await db.Appointments.FindAsync(id);
            if (a == null) return NotFound();
            a.Status = AppointmentStatus.Cancelled;
            await db.SaveChangesAsync();
            return Ok(new { message = "Cancelled" });
        }

        // PUT: api/appointments/{id}/reschedule
        [HttpPut("{id:guid}/reschedule")]
        public async Task<IActionResult> Reschedule(Guid id, [FromBody] DateTime newStartUtc)
        {
            var a = await db.Appointments.FindAsync(id);
            if (a == null) return NotFound();

            var duration = (a.EndUtc - a.StartUtc);
            var newEndUtc = newStartUtc + duration;

            // conflict check
            bool conflict = await db.Appointments.AnyAsync(x =>
                x.Id != id &&
                x.DoctorId == a.DoctorId &&
                newStartUtc < x.EndUtc &&
                newEndUtc > x.StartUtc
            );
            if (conflict) return Conflict("New slot conflicts");

            a.StartUtc = newStartUtc;
            a.EndUtc = newEndUtc;
            await db.SaveChangesAsync();
            return Ok(a);
        }
    }
}
