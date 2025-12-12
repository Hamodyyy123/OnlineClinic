using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClinic.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace OnlineClinic.Controllers
{
    [Route("[controller]")]
    public class ProfileController : Controller
    {
        private readonly OnlineClinicContext _db;

        public ProfileController(OnlineClinicContext db)
        {
            _db = db;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        private string GetUserRole() =>
            User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        private int? GetDoctorIdForUser(int userId) =>
            _db.Doctors
               .Where(d => d.UserId == userId)
               .Select(d => d.DoctorId)
               .FirstOrDefault();

        private int? GetPatientIdForUser(int userId) =>
            _db.Patients
               .Where(p => p.UserId == userId)
               .Select(p => p.PatientId)
               .FirstOrDefault();

        // ============== VIEW ==============

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // ============== API: load profile ==============

        // GET /Profile/GetProfile
        [HttpGet("GetProfile")]
        public IActionResult GetProfile()
        {
            var userId = GetUserId();
            var role = GetUserRole();

            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            var profile = new
            {
                userId = user.UserId,
                role = role,
                fullName = user.Name,
                email = user.Email,
                status = user.Status,
                createdAt = user.CreatedAt,
                isPatient = role == "Patient",
                isDoctor = role == "Doctor",
                isAdmin = role == "Admin"
            };

            object patient = null;
            object doctor = null;
            object medicalHistories = null;

            if (role == "Patient")
            {
                var p = _db.Patients
                    .Include(x => x.MedicalHistories)
                    .FirstOrDefault(x => x.UserId == userId);

                if (p != null)
                {
                    patient = new
                    {
                        patientId = p.PatientId,
                        gender = p.Gender,
                        age = p.Age,                 // stored as Age (int?)
                        contactInfo = p.ContactInfo, // phone / contact
                        status = p.Status,
                        createdAt = p.CreatedAt
                    };

                    medicalHistories = p.MedicalHistories
                        .OrderByDescending(h => h.CreatedAt)
                        .Select(h => new
                        {
                            historyId = h.HistoryId,
                            condition = h.Diagnosis,
                            description = h.Description,
                            createdAt = h.CreatedAt,
                            updatedAt = h.UpdatedAt
                        })
                        .ToList();
                }
            }
            else if (role == "Doctor")
            {
                var d = _db.Doctors.FirstOrDefault(x => x.UserId == userId);
                if (d != null)
                {
                    doctor = new
                    {
                        doctorId = d.DoctorId,
                        specialty = d.Speciality,
                        phoneNumber = d.Phone   // doctor’s phone
                    };
                }
            }

            return Json(new
            {
                user = profile,
                patient,
                doctor,
                medicalHistories
            });
        }

        // ============== API: update basic user profile ==============

        // POST /Profile/UpdateProfile
        [HttpPost("UpdateProfile")]
        public IActionResult UpdateProfile(string fullName, string email)
        {
            var userId = GetUserId();

            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
                return Json(new { success = false, message = "User not found." });

            user.Name = fullName?.Trim();
            user.Email = email?.Trim();
            user.UpdatedAt = DateTime.Now;

            _db.SaveChanges();

            return Json(new { success = true });
        }

        // ============== API: update patient details (if patient) ==============

        // POST /Profile/UpdatePatientDetails
        [HttpPost("UpdatePatientDetails")]
        public IActionResult UpdatePatientDetails(int? age, string gender, string contactInfo)
        {
            var userId = GetUserId();
            var role = GetUserRole();
            if (role != "Patient")
                return Unauthorized();

            var p = _db.Patients.FirstOrDefault(x => x.UserId == userId);
            if (p == null)
                return Json(new { success = false, message = "Patient record not found." });

            p.Age = age;
            p.Gender = gender?.Trim();
            p.ContactInfo = contactInfo?.Trim();
            p.UpdatedAt = DateTime.Now;

            _db.SaveChanges();

            return Json(new { success = true });
        }

        // ============== API: update doctor details (if doctor) ==============

        // POST /Profile/UpdateDoctorDetails
        [HttpPost("UpdateDoctorDetails")]
        public IActionResult UpdateDoctorDetails(string specialty, string phoneNumber)
        {
            var userId = GetUserId();
            var role = GetUserRole();
            if (role != "Doctor")
                return Unauthorized();

            var d = _db.Doctors.FirstOrDefault(x => x.UserId == userId);
            if (d == null)
                return Json(new { success = false, message = "Doctor record not found." });

            d.Speciality = specialty?.Trim();
            d.Phone = phoneNumber?.Trim();

            _db.SaveChanges();

            return Json(new { success = true });
        }
    }
}