// wwwroot/js/UserDashboard.js

document.addEventListener("DOMContentLoaded", function () {
    let role = window.currentUserRole;
    let apiEndpoint = "";

    if (role === "Doctor") apiEndpoint = "/Dashboard/GetDoctorData";
    else if (role === "Admin") apiEndpoint = "/Dashboard/GetAdminData";
    else apiEndpoint = "/Dashboard/GetPatientData";

    fetch(apiEndpoint)
        .then(r => r.json())
        .then(data => renderRoleDashboard(role, data))
        .catch(err => showError(err));
});

function showError(err) {
    console.error(err);
    document.getElementById("dashboard-widgets").innerHTML =
        `<div class="alert alert-danger">Error loading dashboard data. Please try again later.</div>`;
}

// --------- Dashboard Rendering ---------

function renderRoleDashboard(role, data) {
    // Notifications (common) – doctor/patient
    if (data.notifications && data.notifications.length) {
        document.getElementById("notifications-widget").innerHTML =
            renderNotificationsWidget(data.notifications);
    } else if (data.alerts && data.alerts.length) {
        document.getElementById("notifications-widget").innerHTML =
            renderAlertsWidget(data.alerts);
    } else {
        document.getElementById("notifications-widget").innerHTML = "";
    }

    if (role === "Doctor") {
        renderDoctorDashboard(data);
    } else if (role === "Patient") {
        renderPatientDashboard(data);
    } else if (role === "Admin") {
        renderAdminDashboard(data);
    }
}

// -------- WIDGET BUILDERS --------

function renderNotificationsWidget(list) {
    // Special "consultation running now" message => clickable
    const items = list.map(n => {
        if (typeof n === "string" && n.toLowerCase().includes("consultation is running now")) {
            return `<li><a href="/Consultations">A consultation is running now. Click to open.</a></li>`;
        }
        return `<li>${n}</li>`;
    }).join("");

    return `<div class="widget notifications">
        <h5>Notifications</h5>
        <ul>${items}</ul>
    </div>`;
}

function renderAlertsWidget(list) {
    return `<div class="widget alerts">
        <h5>Alerts</h5>
        <ul>${list.map(a => `<li>${a}</li>`).join('')}</ul>
    </div>`;
}

// ------------ DOCTOR DASH -------------
function renderDoctorDashboard(d) {
    const upcomingList = Array.isArray(d.upcomingAppointments) ? d.upcomingAppointments : [];
    const scheduleList = Array.isArray(d.todaysSchedule) ? d.todaysSchedule : [];

    const upcomingHtml = upcomingList.length
        ? upcomingList.map(a => {
            // Match server JSON: { time, span, patient, description }
            const span = a.span || a.time || "";
            const patient = a.patient || "";
            const desc = a.description ? ` (${a.description})` : "";
            return `<li><b>${span}</b> - ${patient}${desc}</li>`;
        }).join("")
        : "<li>No upcoming appointments.</li>";

    const scheduleHtml = scheduleList.length
        ? scheduleList.map(a => {
            const span = a.span || a.time || "";
            const patient = a.patient || "";
            const desc = a.description ? ` (${a.description})` : "";
            return `<li><b>${span}</b> - ${patient}${desc}</li>`;
        }).join("")
        : "<li>No appointments today.</li>";

    let widgets = `
    <div class="row">
      <div class="col-md-6">
        <div class="widget">
          <h5>Upcoming Appointments</h5>
          <ul>${upcomingHtml}</ul>
        </div>
        <div class="widget">
          <h5>Today's Schedule</h5>
          <ul>${scheduleHtml}</ul>
        </div>
      </div>
      <div class="col-md-6">
        <div class="widget">
          <h5>Assigned Patients</h5>
          <div class="stats">${d.stats ? d.stats.assignedPatients : "-"}</div>
        </div>
        <div class="widget">
          <h5>Quick Links</h5>
          <ul>
            <li><a href="${d.links ? d.links.medicalHistory : "#"}">View Patients</a></li>
            <li><a href="${d.links ? d.links.consultations : "#"}">Consultations</a></li>
          </ul>
        </div>
      </div>
    </div>
    `;
    document.getElementById("dashboard-widgets").innerHTML = widgets;
}

// ------------ PATIENT DASH -------------
function renderPatientDashboard(d) {
    const app = d.nextAppointment || {};
    const doctor = d.assignedDoctor || {};

    const hasAppointment = !!app.date || !!app.Date; // handle camelCase or PascalCase from server
    const date = app.date || app.Date || "";
    const span = app.span || app.Span || app.time || app.Time || "";
    const docName = app.doctor || app.Doctor || "";
    const desc = app.description || app.Description || "";

    let nextAppHtml = "No upcoming appointments.";
    if (hasAppointment) {
        nextAppHtml = `<b>${date} ${span}</b> with ${docName} ${desc ? `(${desc})` : ""}`;
    }

    const assignedDoctorName = doctor.name || doctor.Name || "No assigned doctor.";

    let widgets = `
    <div class="row">
      <div class="col-md-6">
        <div class="widget">
          <h5>Next Appointment</h5>
          <div>${nextAppHtml}</div>
        </div>
        <div class="widget">
          <h5>Assigned Doctor</h5>
          <div>${assignedDoctorName}</div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="widget">
          <h5>Quick Links</h5>
          <ul>
            <li><a href="${d.links ? d.links.requestAppointment : "#"}">Request Appointment</a></li>
            <li><a href="${d.links ? d.links.medicalHistory : "#"}">View Medical History</a></li>
          </ul>
        </div>
      </div>
    </div>`;
    document.getElementById("dashboard-widgets").innerHTML = widgets;
}

// ------------- ADMIN DASH --------------
function renderAdminDashboard(d) {
    let s = d.stats || {};
    let l = d.links || {};
    let apptsToday = Array.isArray(d.appointmentsToday) ? d.appointmentsToday : [];

    let apptRows = apptsToday.length
        ? apptsToday.map(a => {
            const start = a.StartTime ? new Date(a.StartTime).toLocaleTimeString() : "";
            const end = a.EndTime ? new Date(a.EndTime).toLocaleTimeString() : "";
            return `<tr>
                <td>${start} - ${end}</td>
                <td>${a.DoctorName || ""}</td>
                <td>${a.PatientName || ""}</td>
                <td>${a.Notes || ""}</td>
            </tr>`;
        }).join("")
        : `<tr><td colspan="4">No appointments today.</td></tr>`;

    let topDoctorText = s.highestWorkingDoctorName
        ? `${s.highestWorkingDoctorName} (${s.highestWorkingDoctorAppointments} appointments total)`
        : "No data.";

    let widgets = `
    <div class="row">
      <div class="col-md-6">
        <div class="widget">
          <h5>Statistics</h5>
          <ul>
            <li>Doctors: <b>${s.doctors ?? "-"}</b></li>
            <li>Patients: <b>${s.patients ?? "-"}</b></li>
            <li>Appointments Today: <b>${s.appointmentsToday ?? "-"}</b></li>
            <li>New Users This Month: <b>${s.newUsersThisMonth ?? "-"}</b></li>
            <li>Highest Working Doctor: <b>${topDoctorText}</b></li>
          </ul>
        </div>
        <div class="widget">
          <h5>Quick Links</h5>
          <ul>
            <li><a href="${l.doctorsList || "#"}">View Doctors</a></li>
            <li><a href="${l.patientsList || "#"}">View Patients</a></li>
            <li><a href="${l.reports || "#"}">Reports</a></li>
          </ul>
        </div>
      </div>
      <div class="col-md-6">
        <div class="widget">
          <h5>Today's Appointments</h5>
          <table class="table table-sm">
            <thead>
              <tr>
                <th>Time</th>
                <th>Doctor</th>
                <th>Patient</th>
                <th>Notes</th>
              </tr>
            </thead>
            <tbody>
              ${apptRows}
            </tbody>
          </table>
        </div>
      </div>
    </div>`;
    document.getElementById("dashboard-widgets").innerHTML = widgets;
}