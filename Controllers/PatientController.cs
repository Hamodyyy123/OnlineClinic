using Microsoft.AspNetCore.Mvc;
using OnlineClinic.Models;
using System.Linq;
using System.Security.Claims;
using System;

namespace OnlineClinic.Controllers
{
    public class PatientsController : Controller
    {
        private readonly OnlineClinicContext _db;

        public PatientsController(OnlineClinicContext db)
        {
            _db = db;
        }

        private string GetUserRole() =>
            User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        // ======================
        // == PATIENTS TABLE  ===
        // ======================

        // List all patients
        public IActionResult Index()
        {
            var role = GetUserRole();
            ViewBag.CanManage = role == "Admin" || role == "Doctor";
            var patients = _db.Patients.ToList();
            var doctors = _db.Users.Where(u => u.Role == "Doctor").ToList();
            ViewBag.Doctors = doctors;
            return View(patients);
        }

        // Patient Details (with assigned doctor name)
        public IActionResult Details(int id)
        {
            var role = GetUserRole();
            ViewBag.CanManage = role == "Admin" || role == "Doctor";
            var patient = _db.Patients.FirstOrDefault(p => p.PatientId == id);
            if (patient == null) return NotFound();
            var doctor = patient.AssignedDoctor.HasValue
                ? _db.Users.FirstOrDefault(u => u.UserId == patient.AssignedDoctor.Value)
                : null;
            ViewBag.AssignedDoctorName = doctor?.Name ?? "";
            return View(patient);
        }

        // AJAX for patient & user info (for edit modal)
        [HttpGet]
        public IActionResult GetPatientUser(int patientId)
        {
            var role = GetUserRole();
            if (role != "Admin" && role != "Doctor") return Unauthorized();
            var patient = _db.Patients.FirstOrDefault(p => p.PatientId == patientId);
            if (patient == null) return Json(new { success = false });
            var user = _db.Users.FirstOrDefault(u => u.UserId == patient.UserId);
            return Json(new
            {
                success = true,
                patient,
                user
            });
        }

        // AJAX: create patient + user from modal
        [HttpPost]
        public IActionResult CreateModal(User user, Patient patient)
        {
            var role = GetUserRole();
            if (role != "Admin" && role != "Doctor") return Unauthorized();

            user.Role = "Patient";
            user.CreatedAt = DateTime.Now;
            _db.Users.Add(user);
            _db.SaveChanges();

            patient.UserId = user.UserId;
            patient.Name = user.Name;
            patient.CreatedAt = DateTime.Now;
            _db.Patients.Add(patient);
            _db.SaveChanges();

            return Json(new { success = true, message = "Patient created!" });
        }

        // AJAX: edit patient + user from modal
        [HttpPost]
        public IActionResult EditModal(User user, Patient patient)
        {
            var role = GetUserRole();
            if (role != "Admin" && role != "Doctor") return Unauthorized();

            var dbUser = _db.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (dbUser == null) return Json(new { success = false, message = "User not found." });
            dbUser.Name = user.Name;
            dbUser.Email = user.Email;
            dbUser.Username = user.Username;
            dbUser.Password = user.Password;
            dbUser.UpdatedAt = DateTime.Now;
            _db.Users.Update(dbUser);

            var dbPatient = _db.Patients.FirstOrDefault(p => p.PatientId == patient.PatientId);
            if (dbPatient == null) return Json(new { success = false, message = "Patient not found." });
            dbPatient.Name = user.Name;
            dbPatient.Age = patient.Age;
            dbPatient.Gender = patient.Gender;
            dbPatient.ContactInfo = patient.ContactInfo;
            dbPatient.EmergencyContact = patient.EmergencyContact;
            dbPatient.AdmissionDate = patient.AdmissionDate;
            dbPatient.RoomNumber = patient.RoomNumber;
            dbPatient.AssignedDoctor = patient.AssignedDoctor;
            dbPatient.CreatedAt = dbPatient.CreatedAt; // don't touch created
            _db.Patients.Update(dbPatient);
            _db.SaveChanges();

            return Json(new { success = true, message = "Patient updated!" });
        }

        // ===========================
        // == MEDICAL HISTORY TABLE ==
        // ===========================

        // List all (AJAX)
        [HttpGet]
        public IActionResult GetMedicalHistory(int patientId)
        {
            var role = GetUserRole();
            if (role != "Admin" && role != "Doctor") return Unauthorized();

            var history = _db.MedicalHistories
                .Where(m => m.PatientId == patientId)
                .OrderByDescending(m => m.StartDate)
                .Select(m => new
                {
                    m.HistoryId,
                    m.PatientId,
                    m.Diagnosis,
                    m.Description,
                    m.StartDate,
                    m.Notes,
                    m.CreatedAt
                })
                .ToList();
            return Json(history);
        }

        // Get single for edit
        [HttpGet]
        public IActionResult GetMedicalHistoryItem(int id)
        {
            var mh = _db.MedicalHistories.FirstOrDefault(m => m.HistoryId == id);
            if (mh == null) return Json(new { success = false });
            return Json(new
            {
                success = true,
                item = new
                {
                    mh.HistoryId,
                    mh.PatientId,
                    mh.Diagnosis,
                    mh.Description,
                    mh.StartDate,
                    mh.Notes,
                    mh.CreatedAt
                }
            });
        }

        // Save/Add/Edit
        [HttpPost]
        public IActionResult SaveMedicalHistory(
            [FromForm] int patientId,
            [FromForm] int? medicalHistoryId,
            [FromForm] string diagnosis,
            [FromForm] string description,
            [FromForm] DateTime startDate,
            [FromForm] string notes
        )
        {
            var role = GetUserRole();
            if (role != "Admin" && role != "Doctor") return Unauthorized();

            if (medicalHistoryId.HasValue && medicalHistoryId.Value > 0)
            {
                // Edit
                var mh = _db.MedicalHistories.FirstOrDefault(m => m.HistoryId == medicalHistoryId.Value);
                if (mh == null) return Json(new { success = false, message = "Medical history not found." });
                mh.Diagnosis = diagnosis;
                mh.Description = description;
                mh.StartDate = DateOnly.FromDateTime(startDate);
                mh.Notes = notes;
            }
            else
            {
                // Create
                _db.MedicalHistories.Add(new MedicalHistory
                {
                    PatientId = patientId,
                    Diagnosis = diagnosis,
                    Description = description,
                    StartDate = DateOnly.FromDateTime(startDate),
                    Notes = notes,
                    CreatedAt = DateTime.Now
                });
            }
            _db.SaveChanges();
            return Json(new { success = true });
        }
    }
}