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

        // Helpers
        private bool IsAdmin() => string.Equals(GetUserRole(), "Admin", StringComparison.OrdinalIgnoreCase);
        private bool IsDoctor() => string.Equals(GetUserRole(), "Doctor", StringComparison.OrdinalIgnoreCase);
        private bool IsPatient() => string.Equals(GetUserRole(), "Patient", StringComparison.OrdinalIgnoreCase);
        private bool IsAuthenticated() => User?.Identity?.IsAuthenticated ?? false;

        // List Doctors
        // Allow any authenticated user to view the index. Mutating actions remain restricted to Admin.
        public IActionResult Index(string mode = "")
        {
            if (!IsAuthenticated()) return Challenge(); // ask to login

            var doctors = _db.Doctors.ToList();
            ViewBag.Mode = mode; // view can use ?mode=view to render read-only UI if desired
            return View(doctors);
        }

        // Doctor Details (allow authenticated users to view)
        public IActionResult Details(int id)
        {
            if (!IsAuthenticated()) return Challenge();

            var doctor = _db.Doctors.FirstOrDefault(d => d.DoctorId == id);
            if (doctor == null) return NotFound();

            return View(doctor);
        }

        // AJAX GET: User for edit modal
        // Only Admins and Doctors may fetch user+doctor info for editing
        [HttpGet]
        public IActionResult GetDoctorUser(int doctorId)
        {
            if (!IsAdmin() && !IsDoctor()) return Unauthorized();

            var doctor = _db.Doctors.FirstOrDefault(d => d.DoctorId == doctorId);
            if (doctor == null) return Json(new { success = false, message = "Doctor not found." });

            var user = _db.Users.FirstOrDefault(u => u.UserId == doctor.UserId);
            return Json(new
            {
                success = true,
                doctor,
                user
            });
        }

        // AJAX: Create Doctor + User (Admin only)
        [HttpPost]
        public IActionResult CreateDoctorModal([FromForm] string name, [FromForm] string email, [FromForm] string username, [FromForm] string password, [FromForm] string phone, [FromForm] string speciality)
        {
            if (!IsAdmin()) return Unauthorized();

            // Prevent duplicate email/username if necessary
            if (_db.Users.Any(u => u.Email == email))
                return Json(new { success = false, message = "Email already in use." });

            if (_db.Users.Any(u => u.Username == username))
                return Json(new { success = false, message = "Username already in use." });

            try
            {
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
            catch (Exception ex)
            {
                // log exception (not shown here)
                return Json(new { success = false, message = "Server error creating doctor." });
            }
        }

        // AJAX: Delete Doctor (Admin only)
        [HttpPost]
        public IActionResult DeleteDoctor([FromForm] int doctorId)
        {
            if (!IsAdmin()) return Unauthorized();

            var doctor = _db.Doctors.FirstOrDefault(d => d.DoctorId == doctorId);
            if (doctor == null)
                return Json(new { success = false, message = "Doctor not found." });

            try
            {
                _db.Doctors.Remove(doctor);
                _db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Server error deleting doctor." });
            }
        }

        // AJAX: Edit Doctor + User (Admin only)
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

            // Optionally check for duplicate email/username (exclude current user)
            if (_db.Users.Any(u => u.Email == email && u.UserId != userId))
                return Json(new { success = false, message = "Email already in use by another user." });

            if (_db.Users.Any(u => u.Username == username && u.UserId != userId))
                return Json(new { success = false, message = "Username already in use by another user." });

            try
            {
                // Update user info
                dbUser.Name = name;
                dbUser.Email = email;
                dbUser.Username = username;
                dbUser.Password = password;
                dbUser.Role = "Doctor";
                // keep CreatedAt unchanged

                // Update doctor info
                dbDoctor.Name = name;
                dbDoctor.Email = email;
                dbDoctor.Phone = phone;
                dbDoctor.Speciality = speciality;
                // keep CreatedAt unchanged

                _db.Users.Update(dbUser);
                _db.Doctors.Update(dbDoctor);
                _db.SaveChanges();

                return Json(new { success = true, message = "Doctor updated." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Server error updating doctor." });
            }
        }
    }
}