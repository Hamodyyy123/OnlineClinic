using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineClinic.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace OnlineClinic.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly OnlineClinicContext _db;

        public DashboardController(OnlineClinicContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            string userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Patient";
            ViewBag.UserRole = userRole;
            return View();
        }

        [HttpGet]
        public IActionResult GetAdminData()
        {
            int doctorCount = _db.Users.Count(u => u.Role == "Doctor");
            int patientCount = _db.Users.Count(u => u.Role == "Patient");
            int appointmentsToday = _db.Appointments.Count(a => a.StartTime.Date == DateTime.Today);

            var alerts = new[]
            {
                "2 licenses expire this month.",
                "New user registration pending approval."
            };

            var result = new
            {
                stats = new { doctors = doctorCount, patients = patientCount, appointmentsToday },
                alerts,
                links = new
                {
                    doctorsList = Url.Action("Index", "Doctors"),
                    patientsList = Url.Action("Index", "Patients"),
                    reports = Url.Action("Index", "Reports")
                }
            };
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetDoctorData()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // 2 upcoming appointments
            var upcomingAppointments = _db.Appointments
                .Where(a => a.DoctorId == userId && a.StartTime >= DateTime.Now)
                .OrderBy(a => a.StartTime)
                .Take(2)
                .Select(a => new
                {
                    Time = a.StartTime.ToShortTimeString(),
                    Patient = _db.Patients.Where(p => p.PatientId == a.PatientId).Select(p => p.Name).FirstOrDefault(),
                    Description = a.Notes
                }).ToList();

            // Today's schedule
            var todaysSchedule = _db.Appointments
                .Where(a => a.DoctorId == userId && a.StartTime.Date == DateTime.Today)
                .OrderBy(a => a.StartTime)
                .Select(a => new
                {
                    Time = a.StartTime.ToShortTimeString(),
                    Patient = _db.Patients.Where(p => p.PatientId == a.PatientId).Select(p => p.Name).FirstOrDefault(),
                    Description = a.Notes
                }).ToList();

            // Notifications for doctor
            var notifList = _db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.NotificationId)
                .Select(n => n.Message)
                .Take(5)
                .ToList();

            // Assigned Patients
            int assignedPatients = _db.Patients.Count(p => p.UserId == userId);

            var result = new
            {
                upcomingAppointments,
                todaysSchedule,
                notifications = notifList,
                stats = new { assignedPatients },
                links = new
                {
                    medicalHistory = Url.Action("Index", "Patients"),
                    prescriptions = Url.Action("Index", "MedicalNotes")
                }
            };
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetPatientData()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var patient = _db.Patients.FirstOrDefault(p => p.UserId == userId);

            var nextAppointment = _db.Appointments
                .Where(a => a.PatientId == patient.PatientId && a.StartTime >= DateTime.Now)
                .OrderBy(a => a.StartTime)
                .Select(a => new
                {
                    Date = a.StartTime.ToShortDateString(),
                    Time = a.StartTime.ToShortTimeString(),
                    Doctor = _db.Users.Where(u => u.UserId == a.DoctorId).Select(u => u.Name).FirstOrDefault(),
                    Description = a.Notes
                })
                .FirstOrDefault();

            var assignedDoctor = _db.Users
                .Where(u => u.Role == "Doctor" && u.UserId == patient.UserId)
                .Select(u => new { Name = u.Name })
                .FirstOrDefault();

            var notifs = _db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.NotificationId)
                .Select(n => n.Message)
                .Take(5)
                .ToList();

            var result = new
            {
                nextAppointment,
                assignedDoctor,
                notifications = notifs,
                links = new
                {
                    requestAppointment = "#", // Add real link for request feature
                    medicalHistory = Url.Action("History", "MedicalHistory"),
                    prescriptions = Url.Action("Index", "MedicalNotes")
                }
            };
            return Json(result);
        }
    }
}