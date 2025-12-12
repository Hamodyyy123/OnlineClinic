// wwwroot/js/Consultations.js

function showPageError(msg) {
    $("#consultation-error").text(msg).removeClass("d-none");
}
function clearPageError() {
    $("#consultation-error").text("").addClass("d-none");
}

function showModalError(msg) {
    $("#consultation-modal-error").text(msg).removeClass("d-none");
}
function clearModalError() {
    $("#consultation-modal-error").text("").addClass("d-none");
}

function apiGet(url, data, cb) {
    $.getJSON(url, data || {}, cb)
        .fail(function (xhr) {
            console.error("GET failed", xhr);
            showPageError("Request failed.");
        });
}

function apiPost(url, data, cb, onModal) {
    $.post(url, data || {}, cb)
        .fail(function (xhr) {
            console.error("POST failed", xhr);
            if (onModal) showModalError("Request failed.");
            else showPageError("Request failed.");
        });
}

$(function () {
    clearPageError();
    loadRunningAppointments();
});

function loadRunningAppointments() {
    apiGet("/Consultations/GetRunningAppointments", null, function (data) {
        if (!Array.isArray(data)) {
            showPageError("Failed to load running appointments.");
            return;
        }

        const rows = data.map(a => {
            const s = a.startTime ? new Date(a.startTime) : null;
            const e = a.endTime ? new Date(a.endTime) : null;
            const start = s ? s.toLocaleString() : "";
            const end = e ? e.toLocaleString() : "";

            let actionCell = "";
            if (a.status === "Completed") {
                actionCell = `<span class="badge bg-success">Completed</span>`;
            } else {
                actionCell = `<button class="btn btn-primary btn-sm" onclick="joinChat(${a.appointmentId})">
                                Join Chat
                              </button>`;
            }

            return `<tr>
                <td>${a.doctorName || ""}</td>
                <td>${a.patientName || ""}</td>
                <td>${start}</td>
                <td>${end}</td>
                <td>${a.status || ""}</td>
                <td>${actionCell}</td>
            </tr>`;
        }).join("");

        $("#runningAppointmentsBody").html(rows || "<tr><td colspan='6'>No running appointments.</td></tr>");
    });
}

let consultationModalInstance = null;
let lastMessageId = 0;
let pollingTimer = null;

function joinChat(appointmentId) {
    clearModalError();

    apiGet("/Consultations/GetConsultationForAppointment", { appointmentId: appointmentId }, function (data) {
        if (!data || data.message) {
            showModalError(data?.message || "Failed to load consultation.");
            return;
        }

        initializeConsultationModal(data);

        const modalEl = document.getElementById("consultationModal");
        consultationModalInstance = new bootstrap.Modal(modalEl);
        consultationModalInstance.show();

        lastMessageId = 0;
        $("#m-chatMessages").html("<em>Loading messages...</em>");
        startPollingMessages();
    });
}

function initializeConsultationModal(data) {
    clearModalError();

    $("#m-consultationId").val(data.consultationId);
    $("#m-currentUserRole").val(data.currentUserRole || "");

    $("#m-doctorName").text(data.doctorName || "");
    $("#m-patientName").text(data.patientName || "");

    const s = data.startTime ? new Date(data.startTime) : null;
    const e = data.endTime ? new Date(data.endTime) : null;
    $("#m-startTime").text(s ? s.toLocaleString() : "");
    $("#m-endTime").text(e ? e.toLocaleString() : "");
    $("#m-status").text(data.status || "");

    const role = data.currentUserRole || "";
    const complaint = data.chiefComplaint || "";
    const diag = data.diagnosis || "";
    const meds = data.medications || "";

    $("#m-problem-edit").addClass("d-none");
    $("#m-problem-view").addClass("d-none");
    $("#m-doctor-notes-card").addClass("d-none");
    $("#m-chiefComplaintMsg").addClass("d-none");
    $("#m-doctorSaveMsg").addClass("d-none");

    if (role === "Patient") {
        $("#m-problem-edit").removeClass("d-none");
        $("#m-problem-view").removeClass("d-none");
        $("#m-chiefComplaintInput").val(complaint);
        $("#m-chiefComplaintText").text(complaint);

        $("#m-saveChiefComplaintBtn").off("click").on("click", function () {
            saveChiefComplaint();
        });
    } else {
        $("#m-problem-view").removeClass("d-none");
        $("#m-chiefComplaintText").text(complaint);
    }

    if (role === "Doctor") {
        $("#m-doctor-notes-card").removeClass("d-none");
        $("#m-diagnosisInput").val(diag);
        $("#m-medicationsInput").val(meds);

        $("#m-saveDiagnosisBtn").off("click").on("click", function () {
            saveDiagnosisAndMedications();
        });

        $("#m-endConsultationBtn").off("click").on("click", function () {
            endConsultation();
        });
    }

    $("#m-chatSendBtn").off("click").on("click", function () {
        sendChatMessage();
    });

    $("#m-chatInput").off("keypress").on("keypress", function (e) {
        if (e.which === 13) {
            e.preventDefault();
            sendChatMessage();
        }
    });

    $("#consultationModal").off("hidden.bs.modal").on("hidden.bs.modal", function () {
        stopPollingMessages();
    });
}

function renderChatMessages(messages, append) {
    const container = $("#m-chatMessages");
    if (!append) container.empty();

    if (!messages || messages.length === 0) {
        if (!append) container.html("<em>No messages yet.</em>");
        return;
    }

    if (!append && container.text().trim() === "No messages yet.") {
        container.empty();
    }

    messages.forEach(m => {
        const sent = m.sentAt ? new Date(m.sentAt).toLocaleTimeString() : "";
        const row = `<div><strong>${m.senderRole}:</strong> ${m.text} <small class="text-muted">${sent}</small></div>`;
        container.append(row);
        lastMessageId = Math.max(lastMessageId, m.id);
    });

    container.scrollTop(container[0].scrollHeight);
}

function startPollingMessages() {
    stopPollingMessages();

    const consultationId = $("#m-consultationId").val();
    if (!consultationId) return;

    function poll() {
        apiGet("/Consultations/GetMessages",
            { consultationId: consultationId, afterId: lastMessageId },
            function (msgs) {
                if (Array.isArray(msgs) && msgs.length > 0) {
                    renderChatMessages(msgs, true);
                }
            });

        pollingTimer = setTimeout(poll, 2000);
    }

    poll();
}

function stopPollingMessages() {
    if (pollingTimer) {
        clearTimeout(pollingTimer);
        pollingTimer = null;
    }
}

function sendChatMessage() {
    const text = $("#m-chatInput").val().trim();
    if (!text) return;

    const consultationId = $("#m-consultationId").val();

    apiPost("/Consultations/PostMessage",
        { consultationId: consultationId, messageText: text },
        function (res) {
            if (res && res.success) {
                $("#m-chatInput").val("");
                renderChatMessages([{
                    id: res.id,
                    senderRole: res.senderRole,
                    senderUserId: res.senderUserId,
                    text: res.text,
                    sentAt: res.sentAt
                }], true);
            } else {
                showModalError(res?.message || "Failed to send message.");
            }
        },
        true);
}

function saveChiefComplaint() {
    clearModalError();
    const consultationId = $("#m-consultationId").val();
    const txt = $("#m-chiefComplaintInput").val().trim();

    if (!txt) {
        alert("Please describe your problem.");
        return;
    }

    apiPost("/Consultations/SaveChiefComplaint",
        { consultationId: consultationId, chiefComplaint: txt },
        function (res) {
            if (res && res.success) {
                $("#m-chiefComplaintText").text(txt);
                $("#m-chiefComplaintMsg").removeClass("d-none");
                setTimeout(() => $("#m-chiefComplaintMsg").addClass("d-none"), 1500);
            } else {
                showModalError(res?.message || "Failed to save complaint.");
            }
        },
        true);
}

function saveDiagnosisAndMedications() {
    clearModalError();
    const consultationId = $("#m-consultationId").val();
    const diag = $("#m-diagnosisInput").val().trim();
    const meds = $("#m-medicationsInput").val().trim();

    apiPost("/Consultations/SaveDiagnosisAndMedications",
        { consultationId: consultationId, diagnosis: diag, medications: meds },
        function (res) {
            if (res && res.success) {
                $("#m-doctorSaveMsg").removeClass("d-none");
                setTimeout(() => $("#m-doctorSaveMsg").addClass("d-none"), 1500);
            } else {
                showModalError(res?.message || "Failed to save diagnosis/medications.");
            }
        },
        true);
}

function endConsultation() {
    clearModalError();
    const consultationId = $("#m-consultationId").val();
    if (!confirm("End this consultation?")) return;

    apiPost("/Consultations/EndConsultation",
        { consultationId: consultationId },
        function (res) {
            if (res && res.success) {
                $("#m-status").text("Completed");
                alert("Consultation ended.");
                if (consultationModalInstance) {
                    consultationModalInstance.hide();
                }
                stopPollingMessages();
                loadRunningAppointments();
            } else {
                showModalError(res?.message || "Failed to end consultation.");
            }
        },
        true);
}