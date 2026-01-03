using doctor_app_api.Data;
using doctor_app_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace doctor_app_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext db;
        public UsersController(AppDbContext db) => this.db = db;

        // READ single
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOne(Guid id)
        {
            var u = await db.Users.FindAsync(id);
            if (u == null) return NotFound();
            return Ok(u);
        }

        // CREATE user (Admin). PasswordHash must be provided by caller.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User input)
        {
            input.Id = Guid.NewGuid(); // server generates
            db.Users.Add(input);
            await db.SaveChangesAsync();
            return Ok(input);
        }

        // UPDATE user (Admin or Owner doctor updates own profile)
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] User input)
        {
            var u = await db.Users.FindAsync(id);
            if (u == null) return NotFound();

            u.FullName = input.FullName;
            u.Email = input.Email;
            u.Role = input.Role;

            // doctor-only fields
            u.Specialization = input.Specialization;
            u.Bio = input.Bio;
            u.ConsultationFee = input.ConsultationFee;

            await db.SaveChangesAsync();
            return Ok(u);
        }

    }
}
