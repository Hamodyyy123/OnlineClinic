using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // ============ ADMIN DASHBOARD ============

        [HttpGet]
        public IActionResult GetAdminData()
        {
            var today = DateTime.Today;
            var firstOfMonth = new DateTime(today.Year, today.Month, 1);

            int doctorCount = _db.Users.Count(u => u.Role == "Doctor");
            int patientCount = _db.Users.Count(u => u.Role == "Patient");

            // New users this month
            int newUsersThisMonth = _db.Users.Count(u => u.CreatedAt >= firstOfMonth);

            // All appointments for today (not just count)
            var appointmentsToday = _db.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.StartTime.Date == today)
                .OrderBy(a => a.StartTime)
                .Select(a => new
                {
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    DoctorName = a.Doctor.Name,
                    PatientName = a.Patient.Name,
                    Notes = a.Notes
                })
                .ToList();

            int appointmentsTodayCount = appointmentsToday.Count;

            // Highest working doctor: doctor with the most appointments (overall)
            var highestWorkingDoctor = _db.Appointments
                .GroupBy(a => a.DoctorId)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    DoctorId = g.Key,
                    AppointmentsCount = g.Count()
                })
                .FirstOrDefault();

            string topDoctorName = null;
            int topDoctorAppointments = 0;

            if (highestWorkingDoctor != null)
            {
                topDoctorName = _db.Doctors
                    .Where(d => d.DoctorId == highestWorkingDoctor.DoctorId)
                    .Select(d => d.Name)
                    .FirstOrDefault();
                topDoctorAppointments = highestWorkingDoctor.AppointmentsCount;
            }

            var stats = new
            {
                doctors = doctorCount,
                patients = patientCount,
                appointmentsToday = appointmentsTodayCount,
                newUsersThisMonth,
                highestWorkingDoctorName = topDoctorName,
                highestWorkingDoctorAppointments = topDoctorAppointments
            };

            var result = new
            {
                stats,
                appointmentsToday,
                links = new
                {
                    doctorsList = Url.Action("Index", "Doctors"),
                    patientsList = Url.Action("Index", "Patients"),
                    reports = Url.Action("Index", "Reports")
                }
            };
            return Json(result);
        }

        // ============ DOCTOR DASHBOARD ============

        [HttpGet]
        public IActionResult GetDoctorData()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Find doctor's DoctorId from Users.UserId
            var doctor = _db.Doctors.FirstOrDefault(d => d.UserId == userId);
            if (doctor == null)
            {
                return Json(new { });
            }
            int doctorId = doctor.DoctorId;

            // 2 upcoming appointments
            var upcomingAppointments = _db.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.StartTime >= DateTime.Now)
                .OrderBy(a => a.StartTime)
                .Take(2)
                .Select(a => new
                {
                    Time = a.StartTime.ToShortTimeString(),
                    Span = $"{a.StartTime:t} - {a.EndTime:t}",
                    Patient = a.Patient.Name,
                    Description = a.Notes
                }).ToList();

            // Today's schedule (with time span)
            var todaysSchedule = _db.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.StartTime.Date == DateTime.Today)
                .OrderBy(a => a.StartTime)
                .Select(a => new
                {
                    Time = a.StartTime.ToShortTimeString(),
                    Span = $"{a.StartTime:t} - {a.EndTime:t}",
                    Patient = a.Patient.Name,
                    Description = a.Notes
                }).ToList();

            // Notifications for doctor
            var notifList = _db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.NotificationId)
                .Select(n => n.Message)
                .Take(5)
                .ToList();

            // Show also if there is a consultation running now
            var now = DateTime.Now;
            var runningConsultations = _db.Consultations
                .Include(c => c.Appointment)
                .Where(c =>
                    c.DoctorId == doctorId &&
                    c.Status == "Open" &&
                    c.Appointment.StartTime <= now &&
                    c.Appointment.EndTime >= now)
                .Select(c => new
                {
                    c.ConsultationId,
                    c.AppointmentId
                })
                .ToList();

            if (runningConsultations.Any())
            {
                notifList.Insert(0, "A consultation is running now. Click to open.");
            }

            // Assigned Patients (patients whose AssignedDoctor == this doctor)
            int assignedPatients = _db.Patients.Count(p => p.AssignedDoctor == doctorId);

            var result = new
            {
                upcomingAppointments,
                todaysSchedule,
                notifications = notifList,
                stats = new { assignedPatients },
                links = new
                {
                    medicalHistory = Url.Action("Index", "Patients"),
                    consultations = Url.Action("Index", "Consultations")
                }
            };
            return Json(result);
        }

        // ============ PATIENT DASHBOARD ============

        [HttpGet]
        public IActionResult GetPatientData()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var patient = _db.Patients
                .Include(p => p.AssignedDoctorNavigation)
                .FirstOrDefault(p => p.UserId == userId);

            if (patient == null)
                return Json(new { });

            // Next appointment with time span
            var nextAppointment = _db.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patient.PatientId && a.StartTime >= DateTime.Now)
                .OrderBy(a => a.StartTime)
                .Select(a => new
                {
                    Date = a.StartTime.ToShortDateString(),
                    Time = a.StartTime.ToShortTimeString(),
                    Span = $"{a.StartTime:t} - {a.EndTime:t}",
                    Doctor = a.Doctor.Name,
                    Description = a.Notes
                })
                .FirstOrDefault();

            // Assigned doctor (from AssignedDoctorNavigation)
            var assignedDoctor = patient.AssignedDoctorNavigation != null
                ? new { Name = patient.AssignedDoctorNavigation.Name }
                : null;

            // Notifications
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
                    requestAppointment = Url.Action("Index", "Appointments"),
                    medicalHistory = Url.Action("Details", "Patients", new { id = patient.PatientId })
                }
            };
            return Json(result);
        }
    }
}