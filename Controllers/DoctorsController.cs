using Microsoft.AspNetCore.Mvc;
using OnlineClinic.Models;
using System.Linq;
using System;

namespace OnlineClinic.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly OnlineClinicContext _db;

        public DoctorsController(OnlineClinicContext db)
        {
            _db = db;
        }

        private string GetUserRole()
            => User?.Claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value ?? "";

        // Only Admin users can access
        private bool IsAdmin() => GetUserRole() == "Admin";

        // List Doctors
        public IActionResult Index()
        {
            if (!IsAdmin()) return Unauthorized();
            var doctors = _db.Doctors.ToList();
            return View(doctors);
        }

        // Doctor Details (optional, standard)
        public IActionResult Details(int id)
        {
            if (!IsAdmin()) return Unauthorized();
            var doctor = _db.Doctors.FirstOrDefault(d => d.DoctorId == id);
            if (doctor == null) return NotFound();
            return View(doctor);
        }

        // AJAX GET: User for edit modal
        [HttpGet]
        public IActionResult GetDoctorUser(int doctorId)
        {
            if (!IsAdmin()) return Unauthorized();
            var doctor = _db.Doctors.FirstOrDefault(d => d.DoctorId == doctorId);
            if (doctor == null) return Json(new { success = false });
            var user = _db.Users.FirstOrDefault(u => u.UserId == doctor.UserId);
            return Json(new
            {
                success = true,
                doctor,
                user
            });
        }

        // AJAX: Create Doctor + User
        [HttpPost]
        public IActionResult CreateDoctorModal([FromForm] string name, [FromForm] string email, [FromForm] string username, [FromForm] string password, [FromForm] string phone, [FromForm] string speciality)
        {
            if (!IsAdmin()) return Unauthorized();

            // 1. Create User
            var newUser = new User
            {
                Name = name,
                Email = email,
                Username = username,
                Password = password,
                Role = "Doctor",
                CreatedAt = DateTime.Now
            };
            _db.Users.Add(newUser);
            _db.SaveChanges(); // Save to get UserId

            // 2. Create Doctor, using user info and phone/speciality
            var newDoctor = new Doctor
            {
                UserId = newUser.UserId,
                Name = name,
                Email = email,
                Phone = phone,
                Speciality = speciality,
                CreatedAt = DateTime.Now
            };
            _db.Doctors.Add(newDoctor);
            _db.SaveChanges();

            return Json(new { success = true, message = "Doctor created." });
        }
        [HttpPost]
        public IActionResult DeleteDoctor([FromForm] int doctorId)
        {
            if (!IsAdmin()) return Unauthorized();

            var doctor = _db.Doctors.FirstOrDefault(d => d.DoctorId == doctorId);
            if (doctor == null)
                return Json(new { success = false, message = "Doctor not found." });

            // Optional: also delete doctor’s associated user, if desired.
            // var user = _db.Users.FirstOrDefault(u => u.UserId == doctor.UserId);
            // if (user != null) _db.Users.Remove(user);

            _db.Doctors.Remove(doctor);
            _db.SaveChanges();
            return Json(new { success = true });
        }
        // AJAX: Edit Doctor + User
        [HttpPost]
        public IActionResult EditDoctorModal(
            [FromForm] int doctorId,
            [FromForm] int userId,
            [FromForm] string name,
            [FromForm] string email,
            [FromForm] string username,
            [FromForm] string password,
            [FromForm] string phone,
            [FromForm] string speciality
        )
        {
            if (!IsAdmin()) return Unauthorized();

            var dbDoctor = _db.Doctors.FirstOrDefault(d => d.DoctorId == doctorId);
            var dbUser = _db.Users.FirstOrDefault(u => u.UserId == userId);

            if (dbDoctor == null || dbUser == null)
                return Json(new { success = false, message = "Doctor or User not found." });

            // Update user info
            dbUser.Name = name;
            dbUser.Email = email;
            dbUser.Username = username;
            dbUser.Password = password;
            dbUser.Role = "Doctor";
            dbUser.CreatedAt = dbUser.CreatedAt;

            // Update doctor info
            dbDoctor.Name = name;
            dbDoctor.Email = email;
            dbDoctor.Phone = phone;
            dbDoctor.Speciality = speciality;
            dbDoctor.CreatedAt = dbDoctor.CreatedAt;

            _db.Users.Update(dbUser);
            _db.Doctors.Update(dbDoctor);
            _db.SaveChanges();

            return Json(new { success = true, message = "Doctor updated." });
        }
    }
}