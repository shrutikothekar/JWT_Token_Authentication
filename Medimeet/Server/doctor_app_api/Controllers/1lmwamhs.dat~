using doctor_app_api.Data;
using doctor_app_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doctor_app_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicsController : Controller
    {
        private readonly AppDbContext db;
        public ClinicsController(AppDbContext db) => this.db = db;

        // GET: api/clinics
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await db.Clinics
                .Select(c => new { c.Id, c.Name, c.Address })
                .ToListAsync());

        // POST: api/clinics
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Clinic clinic)
        {
            clinic.Id = Guid.NewGuid();
            db.Clinics.Add(clinic);
            await db.SaveChangesAsync();
            return Ok(clinic);
        }

        // PUT: api/clinics/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Clinic input)
        {
            var c = await db.Clinics.FindAsync(id);
            if (c == null) return NotFound();

            c.Name = input.Name;
            c.Address = input.Address;
            await db.SaveChangesAsync();
            return Ok(c);
        }

        //// DELETE: api/clinics/{id}
        //// NOTE: Will also remove DoctorClinic links to this clinic.
        //[HttpDelete("{id:guid}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var c = await db.Clinics.FindAsync(id);
        //    if (c == null) return NotFound();

        //    var links = db.DoctorClinics.Where(x => x.ClinicId == id);
        //    db.DoctorClinics.RemoveRange(links);

        //    db.Clinics.Remove(c);
        //    await db.SaveChangesAsync();
        //    return Ok(new { message = "Clinic deleted" });
        //}
    }
}
