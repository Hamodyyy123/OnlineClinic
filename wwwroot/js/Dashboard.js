// wwwroot/js/UserDashboard.js

document.addEventListener("DOMContentLoaded", function () {
    const role = window.currentUserRole;
    let apiEndpoint = "";

    if (role === "Doctor") apiEndpoint = "/Dashboard/GetDoctorData";
    else if (role === "Admin") apiEndpoint = "/Dashboard/GetAdminData";
    else apiEndpoint = "/Dashboard/GetPatientData";

    const wrapper = document.querySelector(".dashboard-page-wrapper");
    if (wrapper) wrapper.setAttribute("data-role", role || "");

    fetch(apiEndpoint)
        .then(r => r.json())
        .then(data => {
            updateHeroCounters(role, data);
            updateMiniChart(role, data);
            renderRoleDashboard(role, data);
        })
        .catch(err => showError(err));
});

function showError(err) {
    console.error(err);
    const el = document.getElementById("dashboard-widgets");
    if (el) {
        el.innerHTML =
            `<div class="alert alert-danger">Error loading dashboard data. Please try again later.</div>`;
    }
}

/* ===== Hero main counter ===== */

function updateHeroCounters(role, data) {
    const mainValue = document.getElementById("dash-main-value");
    const mainCaption = document.getElementById("dash-main-caption");
    if (!mainValue || !mainCaption) return;

    if (role === "Doctor") {
        const s = data.stats || {};
        mainValue.textContent = s.todaysAppointments ?? s.assignedPatients ?? "-";
        mainCaption.textContent = s.todaysAppointments != null
            ? "Appointments today"
            : "Assigned patients";
    } else if (role === "Patient") {
        const app = data.nextAppointment || {};
        if (app.date || app.Date) {
            mainValue.textContent = "1";
            mainCaption.textContent = "Upcoming appointment";
        } else {
            mainValue.textContent = "0";
            mainCaption.textContent = "No upcoming appointments";
        }
    } else if (role === "Admin") {
        const s = data.stats || {};
        mainValue.textContent = s.appointmentsToday ?? s.doctors ?? "-";
        mainCaption.textContent = s.appointmentsToday != null
            ? "Appointments today"
            : "Active doctors";
    } else {
        mainValue.textContent = "-";
        mainCaption.textContent = "Overview";
    }
}

/* ===== Mini donut chart for appointments ===== */

function updateMiniChart(role, data) {
    const center = document.getElementById("mini-donut-value");
    if (!center) return;

    let booked = 0, completed = 0, cancelled = 0;

    if (role === "Admin" || role === "Doctor") {
        const s = data.stats || {};
        booked = s.appointmentsBooked ?? 0;
        completed = s.appointmentsCompleted ?? 0;
        cancelled = s.appointmentsCancelled ?? 0;

        if (!booked && !completed && !cancelled && Array.isArray(data.appointmentsToday)) {
            data.appointmentsToday.forEach(a => {
                const status = (a.Status || a.status || "").toLowerCase();
                if (status === "completed") completed++;
                else if (status === "cancelled") cancelled++;
                else booked++;
            });
        }
    } else if (role === "Patient") {
        const app = data.nextAppointment || {};
        if (app.date || app.Date) booked = 1;
    }

    const total = booked + completed + cancelled;
    center.textContent = total;

    const donut = document.querySelector(".mini-donut-circle");
    if (donut) {
        donut.style.opacity = total ? "1" : "0.4";
    }
}

/* ===== Dashboard Rendering (common notifications) ===== */

function renderRoleDashboard(role, data) {
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

function renderNotificationsWidget(list) {
    const items = list.map(n => {
        if (typeof n === "string" && n.toLowerCase().includes("consultation is running now")) {
            return `<li><a href="/Consultations">A consultation is running now. Click to open.</a></li>`;
        }
        return `<li>${n}</li>`;
    }).join("");

    return `<div class="widget">
        <h5>Notifications</h5>
        <ul>${items}</ul>
    </div>`;
}

function renderAlertsWidget(list) {
    return `<div class="widget">
        <h5>Alerts</h5>
        <ul>${list.map(a => `<li>${a}</li>`).join('')}</ul>
    </div>`;
}

/* ===== Doctor dashboard with bar graph ===== */

function renderDoctorDashboard(d) {
    const upcomingList = Array.isArray(d.upcomingAppointments) ? d.upcomingAppointments : [];
    const scheduleList = Array.isArray(d.todaysSchedule) ? d.todaysSchedule : [];
    const stats = d.stats || {};

    const upcomingHtml = upcomingList.length
        ? upcomingList.map(a => {
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

    const bookedToday = stats.todaysAppointments ?? 0;
    const completedToday = stats.completedToday ?? 0;
    const totalDay = bookedToday + completedToday;
    const completedPct = totalDay ? Math.round((completedToday / totalDay) * 100) : 0;

    const widgets = `
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
          <h5>Workload Overview</h5>
          <ul class="graph-list">
            <li class="graph-item">
              <span class="graph-item-label">Completed Today</span>
              <div class="graph-bar">
                <div class="graph-bar-fill" style="width:${completedPct}%;"></div>
              </div>
              <span>${completedToday}/${totalDay || 0}</span>
            </li>
            <li class="graph-item">
              <span class="graph-item-label">Assigned Patients</span>
              <span class="stats">${stats.assignedPatients ?? "-"}</span>
            </li>
          </ul>
        </div>
        <div class="widget">
          <h5>Quick Links</h5>
          <ul>
            <li><a href="${d.links ? d.links.medicalHistory : "#"}">View Patients</a></li>
            <li><a href="${d.links ? d.links.consultations : "#"}">Consultations</a></li>
          </ul>
        </div>
      </div>
    </div>`;
    document.getElementById("dashboard-widgets").innerHTML = widgets;
}

/* ===== Patient dashboard ===== */

function renderPatientDashboard(d) {
    const app = d.nextAppointment || {};
    const doctor = d.assignedDoctor || {};

    const hasAppointment = !!app.date || !!app.Date;
    const date = app.date || app.Date || "";
    const span = app.span || app.Span || app.time || app.Time || "";
    const docName = app.doctor || app.Doctor || "";
    const desc = app.description || app.Description || "";

    let nextAppHtml = "No upcoming appointments.";
    if (hasAppointment) {
        nextAppHtml = `<b>${date} ${span}</b> with ${docName} ${desc ? `(${desc})` : ""}`;
    }

    const assignedDoctorName = doctor.name || doctor.Name || "No assigned doctor.";
    const stats = d.stats || {};

    const widgets = `
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
          <h5>Your Activity</h5>
          <ul class="graph-list">
            <li class="graph-item">
              <span class="graph-item-label">Completed Appointments</span>
              <span class="stats">${stats.completedAppointments ?? "-"}</span>
            </li>
            <li class="graph-item">
              <span class="graph-item-label">Open Requests</span>
              <span class="stats">${stats.openRequests ?? "-"}</span>
            </li>
          </ul>
        </div>
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

/* ===== Admin dashboard (fixed to handle camelCase) ===== */

function renderAdminDashboard(d) {
    const s = d.stats || {};
    const l = d.links || {};
    const apptsToday = Array.isArray(d.appointmentsToday) ? d.appointmentsToday : [];

    const apptRows = apptsToday.length
        ? apptsToday.map(a => {
            const startRaw = a.StartTime || a.startTime;
            const endRaw = a.EndTime || a.endTime;
            const start = startRaw ? new Date(startRaw).toLocaleTimeString() : "";
            const end = endRaw ? new Date(endRaw).toLocaleTimeString() : "";
            const doctorName = a.DoctorName || a.doctorName || "";
            const patientName = a.PatientName || a.patientName || "";
            const notes = a.Notes || a.notes || "";
            return `<tr>
                <td>${start} - ${end}</td>
                <td>${doctorName}</td>
                <td>${patientName}</td>
                <td>${notes}</td>
            </tr>`;
        }).join("")
        : `<tr><td colspan="4">No appointments today.</td></tr>`;

    const totalUsers = (s.doctors || 0) + (s.patients || 0);
    const doctorPct = totalUsers ? Math.round((s.doctors / totalUsers) * 100) : 0;
    const patientPct = totalUsers ? Math.round((s.patients / totalUsers) * 100) : 0;

    const widgets = `
    <div class="row">
      <div class="col-md-6">
        <div class="widget">
          <h5>Clinic Overview</h5>
          <ul class="graph-list">
            <li class="graph-item">
              <span class="graph-item-label">Doctors</span>
              <div class="graph-bar">
                <div class="graph-bar-fill" style="width:${doctorPct}%;"></div>
              </div>
              <span>${s.doctors ?? 0}</span>
            </li>
            <li class="graph-item">
              <span class="graph-item-label">Patients</span>
              <div class="graph-bar">
                <div class="graph-bar-fill" style="width:${patientPct}%;"></div>
              </div>
              <span>${s.patients ?? 0}</span>
            </li>
            <li class="graph-item">
              <span class="graph-item-label">Appointments Today</span>
              <span class="stats">${s.appointmentsToday ?? "-"}</span>
            </li>
            <li class="graph-item">
              <span class="graph-item-label">New Users This Month</span>
              <span class="stats">${s.newUsersThisMonth ?? "-"}</span>
            </li>
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