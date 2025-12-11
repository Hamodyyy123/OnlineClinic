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
    document.getElementById("dashboard-widgets").innerHTML =
        `<div class="alert alert-danger">Error loading dashboard data. Please try again later.</div>`;
}

// --------- Dashboard Rendering ---------

function renderRoleDashboard(role, data) {
    // Notifications (common)
    if (data.notifications) {
        document.getElementById("notifications-widget").innerHTML =
            renderNotificationsWidget(data.notifications);
    } else if (data.alerts) {
        document.getElementById("notifications-widget").innerHTML =
            renderAlertsWidget(data.alerts);
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
    return `<div class="widget notifications">
        <h5>Notifications</h5>
        <ul>${list.map(n => `<li>${n}</li>`).join('')}</ul>
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
    let widgets = `
    <div class="row">
      <div class="col-md-4">
        <div class="widget">
          <h5>Upcoming Appointments</h5>
          <ul>${(d.upcomingAppointments || []).map(a => `<li><b>${a.Time}</b> - ${a.Patient} (${a.Description || ""})</li>`).join('')}</ul>
        </div>
        <div class="widget">
          <h5>Today's Schedule</h5>
          <ul>${(d.todaysSchedule || []).map(a => `<li><b>${a.Time}</b> - ${a.Patient} (${a.Description || ""})</li>`).join('')}</ul>
        </div>
      </div>
      <div class="col-md-4">
        <div class="widget">
          <h5>Assigned Patients</h5>
          <div class="stats">${d.stats ? d.stats.assignedPatients : "-"}</div>
        </div>
        <div class="widget">
          <h5>Quick Links</h5>
          <ul>
            <li><a href="${d.links ? d.links.medicalHistory : "#"}">View Medical History</a></li>
            <li><a href="${d.links ? d.links.prescriptions : "#"}">Prescriptions</a></li>
          </ul>
        </div>
      </div>
    </div>
    `;
    document.getElementById("dashboard-widgets").innerHTML = widgets;
}

// ------------ PATIENT DASH -------------
function renderPatientDashboard(d) {
    let app = d.nextAppointment || {};
    let doctor = d.assignedDoctor || {};
    let widgets = `
    <div class="row">
      <div class="col-md-6">
        <div class="widget">
          <h5>Next Appointment</h5>
          <div><b>${app.Date || ""} ${app.Time || ""}</b> with ${app.Doctor || ""} (${app.Description || ""})</div>
        </div>
        <div class="widget">
          <h5>Assigned Doctor</h5>
          <div>${doctor.Name || ""}</div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="widget">
          <h5>Quick Links</h5>
          <ul>
            <li><a href="${d.links ? d.links.requestAppointment : "#"}">Request Appointment</a></li>
            <li><a href="${d.links ? d.links.medicalHistory : "#"}">View Medical History</a></li>
            <li><a href="${d.links ? d.links.prescriptions : "#"}">Prescriptions</a></li>
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
    let widgets = `
    <div class="row">
      <div class="col-md-6">
        <div class="widget">
          <h5>Statistics</h5>
          <ul>
            <li>Doctors: <b>${s.doctors}</b></li>
            <li>Patients: <b>${s.patients}</b></li>
            <li>Appointments Today: <b>${s.appointmentsToday}</b></li>
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
    </div>`;
    document.getElementById("dashboard-widgets").innerHTML = widgets;
}