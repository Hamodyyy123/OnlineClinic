// wwwroot/js/appointment_management.js

// ====== GLOBALS & HELPERS ======

const ROLE = window.AppUserRole || "Admin";

function apiGet(url, params, cb) {
    $.getJSON(url, params || {}, cb)
        .fail(xhr => {
            console.error("GET failed", xhr);
            $("#appts-error").removeClass("d-none").text("Request failed.");
        });
}

function apiPost(url, data, cb) {
    $.post(url, data || {}, cb)
        .fail(xhr => {
            console.error("POST failed", xhr);
            $("#appt-error").removeClass("d-none").text("Request failed.");
        });
}

function formatDisplayDate(str) {
    if (!str) return "";
    const normalized = str.replace(" ", "T");
    const d = new Date(normalized);
    return isNaN(d.getTime()) ? str : d.toLocaleString();
}

function buildStatusPill(status) {
    const s = (status || "").toLowerCase();
    if (s === "completed") {
        return `<span class="appts-status-pill appts-status-completed">Completed</span>`;
    }
    if (s === "cancelled") {
        return `<span class="appts-status-pill appts-status-cancelled">Cancelled</span>`;
    }
    return `<span class="appts-status-pill appts-status-booked">Booked</span>`;
}

// Render main appointments table
function renderAppointmentsTable(appts) {
    const rows = (appts || []).map(a => {
        const displayStart = formatDisplayDate(a.startTime);
        const displayEnd = formatDisplayDate(a.endTime);

        const patientCell = ROLE === "Patient"
            ? ""
            : `<td>${a.patientName || ""}</td>`;

        // Patients cannot delete; we can also hide Edit if you want
        const actionsCell = ROLE === "Patient"
            ? `<td></td>`
            : `<td>
                    <button class="btn btn-warning btn-sm"
                            onclick="openEditAppointmentModal(${a.appointmentId})">
                        Edit
                    </button>
                    <button class="btn btn-danger btn-sm mt-1 mt-md-0"
                            onclick="deleteAppointment(${a.appointmentId}, '${a.startTime}')">
                        Delete
                    </button>
               </td>`;

        const statusPill = buildStatusPill(a.status);

        return `<tr>
            <td>${a.doctorName || ""}</td>
            ${patientCell}
            <td>${displayStart}</td>
            <td>${displayEnd}</td>
            <td>${statusPill}</td>
            <td>${a.notes || ""}</td>
            ${actionsCell}
        </tr>`;
    }).join("");

    $("#apptsTableBody").html(rows || `<tr><td colspan="7" class="text-center text-muted py-4">No appointments found.</td></tr>`);

    // Update small summary counters
    const total = (appts || []).length;
    const today = (appts || []).filter(a => (a.status || "").toLowerCase() === "booked").length;
    $("#apptsCountSummary").text(`${total} appointment${total === 1 ? "" : "s"} loaded`);
    $("#apptsTodayCount").text(today);
    $("#apptsUpcomingCount").text(Math.max(total - today, 0));
}

// ====== INITIALIZATION ======

$(function () {
    console.log("Appointments JS loaded, ROLE =", ROLE);

    // Patient role: hide patient filter + header + modal patient row + status selector
    if (ROLE === "Patient") {
        console.log("Applying patient UI restrictions");
        $(".filter-patient-wrapper").hide();
        $("#filterPatient").hide();
        $("#thPatient").hide();
        $(".appt-patient-row").hide();
        // hide status row in modal
        $("#apptStatus").closest(".col-md-6").hide();
    }

    loadDoctorsAndPatients();
    fetchAndRenderAppointments();

    $("#apptsFilterForm").on("submit", function (e) {
        e.preventDefault();
        fetchAndRenderAppointments();
    });

    $("#apptsRefreshBtn").on("click", function () {
        fetchAndRenderAppointments();
    });

    $("#apptAddBtn").on("click", function () {
        openCreateAppointmentModal();
    });
});

// ====== DATA LOADING ======

function fetchAndRenderAppointments() {
    $("#appts-error").addClass("d-none").text("");

    const params = {
        date: $("#filterDate").val(),
        doctorId: $("#filterDoctor").val(),
        patientId: ROLE === "Admin" ? $("#filterPatient").val() : null
    };
    apiGet("/Appointments/Fetch", params, renderAppointmentsTable);
}

function loadDoctorsAndPatients() {
    apiGet("/Appointments/DoctorsList", null, function (doctors) {
        let filterOpts = '<option value="">All Doctors</option>';
        let modalOpts = '<option value="">Select Doctor</option>';
        (doctors || []).forEach(d => {
            filterOpts += `<option value="${d.doctorId}">${d.name}</option>`;
            modalOpts += `<option value="${d.doctorId}">${d.name}</option>`;
        });
        $("#filterDoctor").html(filterOpts);
        $("#apptDoctor").html(modalOpts);
    });

    if (ROLE !== "Patient") {
        apiGet("/Appointments/PatientsList", null, function (patients) {
            let filterOpts = '<option value="">All Patients</option>';
            let modalOpts = '<option value="">Select Patient</option>';
            (patients || []).forEach(p => {
                filterOpts += `<option value="${p.patientId}">${p.name}</option>`;
                modalOpts += `<option value="${p.patientId}">${p.name}</option>`;
            });
            $("#filterPatient").html(filterOpts);
            $("#apptPatient").html(modalOpts);
        });
    } else {
        $("#filterPatient").empty().hide();
        $("#apptPatient").empty().hide();
    }
}

// ====== MODAL HANDLING ======

function openCreateAppointmentModal() {
    $("#apptModalLabel").text("Book/Add Appointment");
    $("#appt-error").addClass("d-none").text("");
    $("#apptId").val("");
    $("#apptDoctor").val("");
    $("#apptPatient").val("");
    $("#apptStart").val("");
    $("#apptEnd").val("");
    $("#apptStatus").val("Booked");
    $("#apptNotes").val("");
    $("#doctorSlots").html("");

    if (ROLE === "Patient") {
        $(".appt-patient-row").hide();
        $("#apptStatus").closest(".col-md-6").hide();
    } else {
        $(".appt-patient-row").show();
        $("#apptStatus").closest(".col-md-6").show();
    }

    $("#apptModal").modal("show");

    $("#apptSaveBtn").off("click").on("click", function (e) {
        e.preventDefault();
        saveAppointment();
    });

    if (ROLE === "Patient") {
        $("#apptDoctor").off("change").on("change", function () {
            showDoctorSlots($(this).val());
        });
    } else {
        $("#apptDoctor").off("change").on("change", function () {
            showDoctorSlots($(this).val());
        });
    }
}

function openEditAppointmentModal(id) {
    apiGet("/Appointments/GetAppointment", { id }, function (res) {
        if (!res || !res.success) {
            $("#appt-error").removeClass("d-none").text(res?.message || "Failed to load appointment.");
            return;
        }
        const a = res.appt;
        $("#apptModalLabel").text("Edit Appointment");
        $("#apptId").val(a.appointmentId);
        $("#apptDoctor").val(a.doctorId);
        $("#apptStart").val(a.startTime);
        $("#apptEnd").val(a.endTime);
        $("#apptStatus").val(a.status);
        $("#apptNotes").val(a.notes || "");
        $("#doctorSlots").html("");

        if (ROLE === "Patient") {
            $(".appt-patient-row").hide();
            $("#apptStatus").closest(".col-md-6").hide();
        } else {
            $("#apptPatient").val(a.patientId);
            $(".appt-patient-row").show();
            $("#apptStatus").closest(".col-md-6").show();
        }

        $("#apptModal").modal("show");

        $("#apptSaveBtn").off("click").on("click", function (e) {
            e.preventDefault();
            saveAppointment();
        });
    });
}

// ====== SAVE / DELETE ======

function saveAppointment() {
    $("#appt-error").addClass("d-none").text("");

    const data = {
        appointmentId: $("#apptId").val(),
        doctorId: $("#apptDoctor").val(),
        // patientId is ignored for patient role (server uses logged-in patient)
        patientId: ROLE === "Patient" ? 0 : $("#apptPatient").val(),
        startTime: $("#apptStart").val(),
        endTime: $("#apptEnd").val(),
        // for patient role, backend forces status to "Booked" anyway
        status: $("#apptStatus").val(),
        notes: $("#apptNotes").val()
    };

    apiPost("/Appointments/SaveAppointment", data, function (res) {
        if (res && res.success) {
            $("#apptModal").modal("hide");
            fetchAndRenderAppointments();
        } else {
            $("#appt-error").removeClass("d-none").text(res?.message || "Failed to save appointment.");
        }
    });
}

function deleteAppointment(id, apptStart) {
    if (ROLE === "Patient") {
        alert("You are not allowed to delete appointments.");
        return;
    }
    if (!confirm("Delete this appointment?")) return;

    apiPost("/Appointments/DeleteAppointment", { appointmentId: id }, function (res) {
        if (res && res.success) {
            fetchAndRenderAppointments();
        } else {
            alert(res?.message || "Failed to delete appointment.");
        }
    });
}

// ====== DOCTOR SLOTS (per-doctor in modal) ======

function showDoctorSlots(doctorId) {
    $("#doctorSlots").html("Loading...");
    if (!doctorId) {
        $("#doctorSlots").html("");
        return;
    }

    apiGet("/Appointments/DoctorAppointments", { doctorId }, function (data) {
        if (!data || !data.length) {
            $("#doctorSlots").html("<span class=\"text-success\">All times are available.</span>");
            return;
        }
        const rows = data.map(a => {
            const s = new Date(a.start);
            const e = a.end ? new Date(a.end) : null;
            return `<li>${s.toLocaleString()} - ${e ? e.toLocaleString() : ""}</li>`;
        }).join("");
        $("#doctorSlots").html("<b>Doctor's Appointments:</b><ul>" + rows + "</ul>");
    });
}