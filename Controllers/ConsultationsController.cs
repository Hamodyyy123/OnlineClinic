using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OnlineClinic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace OnlineClinic.Controllers
{
    [Route("[controller]")]
    public class ConsultationsController : Controller
    {
        private readonly OnlineClinicContext _db;
        private readonly IMemoryCache _cache;

        public ConsultationsController(OnlineClinicContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        private string GetUserRole() =>
            User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

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

        private string ChatCacheKey(int consultationId) => $"ConsultationChat_{consultationId}";

        // ====== DTO for chat messages kept only in cache ======
        private class ChatMessageDto
        {
            public int Id { get; set; }
            public string SenderRole { get; set; }
            public int SenderUserId { get; set; }
            public string Text { get; set; }
            public DateTime SentAt { get; set; }
        }

        // ===================== VIEW =====================

        // GET /Consultations
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // ===================== API: list running appointments + consultation status =====================

        // GET /Consultations/GetRunningAppointments
        [HttpGet("GetRunningAppointments")]
        public IActionResult GetRunningAppointments()
        {
            var role = GetUserRole();
            var userId = GetUserId();
            var now = DateTime.Now;

            var q = _db.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .AsQueryable();

            // only within current time window and not cancelled
            q = q.Where(a =>
                a.StartTime <= now &&
                a.EndTime >= now &&
                a.Status != "Cancelled");

            if (role == "Doctor")
            {
                var doctorId = GetDoctorIdForUser(userId);
                if (doctorId == null) return Unauthorized();
                q = q.Where(a => a.DoctorId == doctorId);
            }
            else if (role == "Patient")
            {
                var patientId = GetPatientIdForUser(userId);
                if (patientId == null) return Unauthorized();
                q = q.Where(a => a.PatientId == patientId);
            }

            var basic = q
                .OrderBy(a => a.StartTime)
                .Select(a => new
                {
                    appointmentId = a.AppointmentId,
                    doctorName = a.Doctor.Name,
                    patientName = a.Patient.Name,
                    startTime = a.StartTime,
                    endTime = a.EndTime
                })
                .ToList();

            var apptIds = basic.Select(x => x.appointmentId).ToList();
            var consultationsByAppt = _db.Consultations
                .Where(c => apptIds.Contains(c.AppointmentId))
                .ToDictionary(c => c.AppointmentId, c => c.Status);

            var result = basic.Select(a => new
            {
                a.appointmentId,
                a.doctorName,
                a.patientName,
                a.startTime,
                a.endTime,
                status = consultationsByAppt.TryGetValue(a.appointmentId, out var s) ? s : "NotStarted"
            }).ToList();

            return Json(result);
        }

        // ===================== API: load/create consultation =====================

        // GET /Consultations/GetConsultationForAppointment?appointmentId=123
        [HttpGet("GetConsultationForAppointment")]
        public IActionResult GetConsultationForAppointment(int appointmentId)
        {
            var role = GetUserRole();
            var userId = GetUserId();

            var appt = _db.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefault(a => a.AppointmentId == appointmentId);

            if (appt == null)
                return NotFound(new { message = "Appointment not found." });

            var doctorIdForUser = GetDoctorIdForUser(userId);
            var patientIdForUser = GetPatientIdForUser(userId);

            var allowed =
                role == "Admin" ||
                (role == "Doctor" && appt.DoctorId == doctorIdForUser) ||
                (role == "Patient" && appt.PatientId == patientIdForUser);

            if (!allowed)
                return Unauthorized();

            var consultation = _db.Consultations
                .FirstOrDefault(c => c.AppointmentId == appointmentId);

            if (consultation == null)
            {
                consultation = new Consultation
                {
                    AppointmentId = appointmentId,
                    DoctorId = appt.DoctorId,
                    PatientId = appt.PatientId,
                    Status = "Open",
                    StartedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Consultations.Add(consultation);
                _db.SaveChanges();
            }

            var dto = new
            {
                consultationId = consultation.ConsultationId,
                appointmentId = appt.AppointmentId,
                doctorName = appt.Doctor.Name,
                patientName = appt.Patient.Name,
                startTime = appt.StartTime,
                endTime = appt.EndTime,
                chiefComplaint = consultation.ChiefComplaint,
                diagnosis = consultation.Diagnosis,
                medications = consultation.Medications,
                status = consultation.Status,
                currentUserRole = role
            };

            return Json(dto);
        }

        // ===================== API: patient problem =====================

        [HttpPost("SaveChiefComplaint")]
        public IActionResult SaveChiefComplaint(int consultationId, string chiefComplaint)
        {
            var role = GetUserRole();
            var userId = GetUserId();

            var c = _db.Consultations
                .FirstOrDefault(x => x.ConsultationId == consultationId);

            if (c == null)
                return Json(new { success = false, message = "Consultation not found." });

            var patientIdForUser = GetPatientIdForUser(userId);
            if (role != "Patient" || c.PatientId != patientIdForUser)
                return Unauthorized();

            c.ChiefComplaint = chiefComplaint;
            c.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();

            return Json(new { success = true });
        }

        // ===================== API: doctor notes =====================

        [HttpPost("SaveDiagnosisAndMedications")]
        public IActionResult SaveDiagnosisAndMedications(int consultationId, string diagnosis, string medications)
        {
            var role = GetUserRole();
            var userId = GetUserId();

            var c = _db.Consultations
                .FirstOrDefault(x => x.ConsultationId == consultationId);

            if (c == null)
                return Json(new { success = false, message = "Consultation not found." });

            var doctorIdForUser = GetDoctorIdForUser(userId);
            if (role != "Doctor" || c.DoctorId != doctorIdForUser)
                return Unauthorized();

            c.Diagnosis = diagnosis;
            c.Medications = medications;
            c.UpdatedAt = DateTime.UtcNow;
            _db.SaveChanges();

            return Json(new { success = true });
        }

        // ===================== API: end consultation =====================

        [HttpPost("EndConsultation")]
        public IActionResult EndConsultation(int consultationId)
        {
            var role = GetUserRole();
            var userId = GetUserId();

            var c = _db.Consultations
                .FirstOrDefault(x => x.ConsultationId == consultationId);

            if (c == null)
                return Json(new { success = false, message = "Consultation not found." });

            var doctorIdForUser = GetDoctorIdForUser(userId);
            if (role != "Doctor" || c.DoctorId != doctorIdForUser)
                return Unauthorized();

            c.Status = "Completed";
            c.EndedAt = DateTime.UtcNow;
            c.UpdatedAt = DateTime.UtcNow;

            // Also mark appointment as completed
            var appt = _db.Appointments.FirstOrDefault(a => a.AppointmentId == c.AppointmentId);
            if (appt != null)
            {
                appt.Status = "Completed";
            }

            _db.SaveChanges();

            // Remove chat from cache when consultation ends
            _cache.Remove(ChatCacheKey(consultationId));

            return Json(new { success = true });
        }

        // ===================== API: chat via cache =====================

        // GET /Consultations/GetMessages?consultationId=1&afterId=0
        [HttpGet("GetMessages")]
        public IActionResult GetMessages(int consultationId, int afterId = 0)
        {
            var role = GetUserRole();
            var userId = GetUserId();

            var c = _db.Consultations.FirstOrDefault(x => x.ConsultationId == consultationId);
            if (c == null)
                return NotFound(new { message = "Consultation not found." });

            var doctorIdForUser = GetDoctorIdForUser(userId);
            var patientIdForUser = GetPatientIdForUser(userId);
            var allowed =
                role == "Admin" ||
                (role == "Doctor" && c.DoctorId == doctorIdForUser) ||
                (role == "Patient" && c.PatientId == patientIdForUser);

            if (!allowed)
                return Unauthorized();

            var key = ChatCacheKey(consultationId);
            var list = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return new List<ChatMessageDto>();
            });

            var msgs = list
                .Where(m => m.Id > afterId)
                .OrderBy(m => m.Id)
                .Select(m => new
                {
                    id = m.Id,
                    senderRole = m.SenderRole,
                    senderUserId = m.SenderUserId,
                    text = m.Text,
                    sentAt = m.SentAt
                })
                .ToList();

            return Json(msgs);
        }

        // POST /Consultations/PostMessage
        [HttpPost("PostMessage")]
        public IActionResult PostMessage(int consultationId, string messageText)
        {
            var role = GetUserRole();
            var userId = GetUserId();

            if (string.IsNullOrWhiteSpace(messageText))
                return Json(new { success = false, message = "Empty message." });

            var c = _db.Consultations
                .FirstOrDefault(x => x.ConsultationId == consultationId);

            if (c == null)
                return Json(new { success = false, message = "Consultation not found." });

            var doctorIdForUser = GetDoctorIdForUser(userId);
            var patientIdForUser = GetPatientIdForUser(userId);
            var allowed =
                role == "Admin" ||
                (role == "Doctor" && c.DoctorId == doctorIdForUser) ||
                (role == "Patient" && c.PatientId == patientIdForUser);

            if (!allowed)
                return Unauthorized();

            var key = ChatCacheKey(consultationId);
            var list = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return new List<ChatMessageDto>();
            });

            var nextId = list.Count == 0 ? 1 : list.Max(m => m.Id) + 1;

            var msg = new ChatMessageDto
            {
                Id = nextId,
                SenderUserId = userId,
                SenderRole = role,
                Text = messageText,
                SentAt = DateTime.UtcNow
            };

            list.Add(msg);

            return Json(new
            {
                success = true,
                id = msg.Id,
                senderRole = msg.SenderRole,
                senderUserId = msg.SenderUserId,
                text = msg.Text,
                sentAt = msg.SentAt
            });
        }
    }
}