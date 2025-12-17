// wwwroot/js/Profile.js

function pfShowError(msg) {
    $("#profile-error").text(msg).removeClass("d-none");
    setTimeout(() => $("#profile-error").addClass("d-none"), 4000);
}

function pfClearError() {
    $("#profile-error").text("").addClass("d-none");
}

function pfShowSuccess(msg) {
    $("#profile-success").text(msg).removeClass("d-none");
    setTimeout(() => $("#profile-success").addClass("d-none"), 2000);
}

function pfGet(url, data, cb) {
    $.getJSON(url, data || {}, cb)
        .fail(function (xhr) {
            console.error("GET failed", xhr);
            pfShowError("Request failed.");
        });
}

function pfPost(url, data, cb) {
    $.post(url, data || {}, cb)
        .fail(function (xhr) {
            console.error("POST failed", xhr);
            pfShowError("Request failed.");
        });
}

$(function () {
    pfClearError();
    loadProfile();

    $("#pf-saveProfileBtn").on("click", function () {
        saveBasicProfile();
    });

    $("#pf-savePatientBtn").on("click", function () {
        savePatientDetails();
    });

    $("#pf-saveDoctorBtn").on("click", function () {
        saveDoctorDetails();
    });
});

function loadProfile() {
    pfGet("/Profile/GetProfile", null, function (data) {
        if (!data || !data.user) {
            pfShowError(data?.message || "Failed to load profile.");
            return;
        }

        const user = data.user;
        $("#pf-role").val(user.role || "");

        // Update header display
        $("#profile-display-name").text(user.fullName || "User");
        $("#pf-role-text").text(user.role || "User");

        // Basic user info
        $("#pf-fullName").val(user.fullName || "");
        $("#pf-email").val(user.email || "");

        // Patient section
        if (user.isPatient && data.patient) {
            $("#pf-patient-section").removeClass("d-none");

            const p = data.patient;
            $("#pf-age").val(p.age != null ? p.age : "");
            $("#pf-gender").val(p.gender || "");
            $("#pf-contactInfo").val(p.contactInfo || "");

            if (Array.isArray(data.medicalHistories)) {
                $("#pf-medicalHistories-card").removeClass("d-none");
                const rows = data.medicalHistories.map(h => {
                    const created = h.createdAt ? new Date(h.createdAt).toLocaleString() : "";
                    const updated = h.updatedAt ? new Date(h.updatedAt).toLocaleString() : "";
                    return `<tr>
                        <td>${h.condition || ""}</td>
                        <td>${h.description || ""}</td>
                        <td>${created}</td>
                        <td>${updated}</td>
                    </tr>`;
                }).join("");
                $("#pf-medicalHistoriesBody").html(rows || "<tr><td colspan='4' style='text-align: center; padding: var(--space-8); color: var(--text-tertiary);'>No medical history records found.</td></tr>");
            }
        }

        // Doctor section
        if (user.isDoctor && data.doctor) {
            $("#pf-doctor-section").removeClass("d-none");
            const d = data.doctor;
            $("#pf-specialty").val(d.specialty || "");
            $("#pf-doctorPhone").val(d.phoneNumber || "");
        }
    });
}

function saveBasicProfile() {
    pfClearError();
    const fullName = $("#pf-fullName").val();
    const email = $("#pf-email").val();

    pfPost("/Profile/UpdateProfile",
        { fullName, email },
        function (res) {
            if (res && res.success) {
                pfShowSuccess("Profile saved successfully!");
                // Update header display
                $("#profile-display-name").text(fullName || "User");
            } else {
                pfShowError(res?.message || "Failed to save profile.");
            }
        });
}

function savePatientDetails() {
    pfClearError();
    const age = $("#pf-age").val();
    const gender = $("#pf-gender").val();
    const contactInfo = $("#pf-contactInfo").val();

    pfPost("/Profile/UpdatePatientDetails",
        { age, gender, contactInfo },
        function (res) {
            if (res && res.success) {
                pfShowSuccess("Patient details saved successfully!");
            } else {
                pfShowError(res?.message || "Failed to save patient details.");
            }
        });
}

function saveDoctorDetails() {
    pfClearError();
    const specialty = $("#pf-specialty").val();
    const phoneNumber = $("#pf-doctorPhone").val();

    pfPost("/Profile/UpdateDoctorDetails",
        { specialty, phoneNumber },
        function (res) {
            if (res && res.success) {
                pfShowSuccess("Doctor details saved successfully!");
            } else {
                pfShowError(res?.message || "Failed to save doctor details.");
            }
        });
}