using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineClinic.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace OnlineClinic.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly OnlineClinicContext _db;
        public AppointmentsController(OnlineClinicContext db) => _db = db;

        private string GetUserRole() => User.FindFirst(ClaimTypes.Role)?.Value ?? "";
        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        private int? GetDoctorIdForUser(int userId) =>
            _db.Doctors.Where(d => d.UserId == userId).Select(d => d.DoctorId).FirstOrDefault();

        private int? GetPatientIdForUser(int userId) =>
            _db.Patients.Where(p => p.UserId == userId).Select(p => p.PatientId).FirstOrDefault();

        private IQueryable<Appointment> GetBaseQuery() =>
            _db.Appointments.Include(a => a.Doctor).Include(a => a.Patient);

        private IQueryable<Appointment> ApplyRoleFilter(IQueryable<Appointment> q)
        {
            var role = GetUserRole();
            var userId = GetUserId();
            return role switch
            {
                "Admin" => q,
                "Doctor" => q.Where(a => a.DoctorId == GetDoctorIdForUser(userId)),
                "Patient" => q.Where(a => a.PatientId == GetPatientIdForUser(userId)),
                _ => q.Where(a => false)
            };
        }

        private object ToDto(Appointment a)
        {
            var role = GetUserRole();
            var patientName = role == "Patient" ? "" : a.Patient?.Name;

            return new
            {
                a.AppointmentId,
                a.DoctorId,
                doctorName = a.Doctor?.Name,
                a.PatientId,
                patientName,
                startTime = a.StartTime.ToString("yyyy-MM-dd HH:mm"),
                endTime = a.EndTime?.ToString("yyyy-MM-dd HH:mm") ?? "",
                a.Status,
                a.Notes
            };
        }

        private bool HasOverlap(int doctorId, int excludeId, DateTime start, DateTime end) =>
            _db.Appointments.Any(a =>
                a.DoctorId == doctorId &&
                a.AppointmentId != excludeId &&
                a.Status == "Booked" &&
                a.EndTime.HasValue &&
                (
                    (start >= a.StartTime && start < a.EndTime.Value) ||
                    (end > a.StartTime && end <= a.EndTime.Value) ||
                    (start <= a.StartTime && end >= a.EndTime.Value)
                )
            );

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Fetch(DateTime? date = null, int? doctorId = null, int? patientId = null)
        {
            var q = ApplyRoleFilter(GetBaseQuery());
            var role = GetUserRole();

            if (role == "Admin")
            {
                if (date.HasValue) q = q.Where(a => a.StartTime.Date == date.Value.Date);
                if (doctorId > 0) q = q.Where(a => a.DoctorId == doctorId);
                if (patientId > 0) q = q.Where(a => a.PatientId == patientId);
            }
            else if (role == "Doctor")
            {
                if (date.HasValue) q = q.Where(a => a.StartTime.Date == date.Value.Date);
                if (doctorId > 0) q = q.Where(a => a.DoctorId == doctorId);
            }

            var list = q.AsEnumerable().OrderBy(a => a.StartTime).Select(ToDto);
            return Json(list);
        }

        [HttpGet]
        public IActionResult GetAppointment(int id)
        {
            var appt = GetBaseQuery().FirstOrDefault(a => a.AppointmentId == id);
            if (appt == null) return Json(new { success = false });

            return Json(new
            {
                success = true,
                appt = new
                {
                    appt.AppointmentId,
                    appt.PatientId,
                    appt.DoctorId,
                    startTime = appt.StartTime.ToString("yyyy-MM-ddTHH:mm"),
                    endTime = appt.EndTime?.ToString("yyyy-MM-ddTHH:mm") ?? "",
                    appt.Status,
                    appt.Notes
                }
            });
        }

        [HttpPost]
        public IActionResult SaveAppointment(
            int? appointmentId, int doctorId, int patientId,
            DateTime startTime, DateTime endTime, string status, string notes)
        {
            var role = GetUserRole();
            var userId = GetUserId();

            // For patient role, override patientId and lock status to "Booked"
            if (role == "Patient")
            {
                var myPatientId = GetPatientIdForUser(userId);
                if (myPatientId == null)
                    return Unauthorized();

                patientId = myPatientId.Value;
                status = "Booked";
            }

            if (role == "Doctor" && GetDoctorIdForUser(userId) != doctorId)
                return Unauthorized();

            if (role != "Patient" && role != "Doctor" && role != "Admin")
                return Unauthorized();

            if (HasOverlap(doctorId, appointmentId ?? 0, startTime, endTime))
                return Json(new { success = false, message = "The doctor already has an appointment at that time." });

            Appointment appt;
            if (appointmentId is > 0)
            {
                appt = _db.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId.Value);
                if (appt == null) return Json(new { success = false, message = "Appointment not found." });

                // Extra safety: if patient, ensure they can only modify their own appointment
                if (role == "Patient")
                {
                    var myPatientId = GetPatientIdForUser(userId);
                    if (appt.PatientId != myPatientId)
                        return Unauthorized();
                }

                appt.UpdatedAt = DateTime.Now;
            }
            else
            {
                appt = new Appointment { CreatedAt = DateTime.Now };
                _db.Appointments.Add(appt);
            }

            appt.DoctorId = doctorId;
            appt.PatientId = patientId;
            appt.StartTime = startTime;
            appt.EndTime = endTime;
            appt.Status = status;
            appt.Notes = notes;

            _db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DeleteAppointment(int appointmentId)
        {
            var role = GetUserRole();
            var userId = GetUserId();
            var appt = _db.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            if (appt == null) return Json(new { success = false, message = "Appointment not found." });

            // Patients are not allowed to delete at all
            if (role == "Patient")
                return Unauthorized();

            var allowed =
                role == "Admin" ||
                (role == "Doctor" && appt.DoctorId == GetDoctorIdForUser(userId));

            if (!allowed) return Unauthorized();

            _db.Appointments.Remove(appt);
            _db.SaveChanges();
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult DoctorAppointments(int doctorId)
        {
            var appts = _db.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => new
                {
                    id = a.AppointmentId,
                    start = a.StartTime,
                    end = a.EndTime,
                    a.PatientId
                })
                .ToList();

            return Json(appts);
        }

        [HttpGet]
        public IActionResult DoctorsList() =>
            Json(_db.Doctors.Select(d => new { doctorId = d.DoctorId, name = d.Name }).ToList());

        [HttpGet]
        public IActionResult PatientsList() =>
            Json(_db.Patients.Select(p => new { patientId = p.PatientId, name = p.Name }).ToList());
    }
}