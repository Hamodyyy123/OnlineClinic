// ====== PATIENT ADD/EDIT MODALS FOR LIST ======
function openCreatePatientModal() {
    $('#patientModalLabel').text('Add New Patient');
    $('#modalPatientId').val('');
    $('#modalUserId').val('');
    $('#modal-error').addClass('d-none').text('');
    $('#userName').val('');
    $('#userEmail').val('');
    $('#userUsername').val('');
    $('#userPassword').val('');
    $('#patientAge').val('');
    $('#patientGender').val('');
    $('#patientContactInfo').val('');
    $('#patientEmergencyContact').val('');
    $('#patientRoomNumber').val('');
    $('#patientAdmissionDate').val('');
    $('#assignedDoctor').val('');
    $('#patientModal').modal('show');
    $('#modalSaveBtn').off('click').on('click', function (e) {
        e.preventDefault();
        savePatient('create');
    });
}

function openEditPatientModal(patientId) {
    $.getJSON(`/Patients/GetPatientUser?patientId=${patientId}`, function (res) {
        if (!res.success) {
            $('#modal-error').removeClass('d-none').text('Failed to load patient.');
            return;
        }
        var user = res.user;
        var patient = res.patient;
        $('#patientModalLabel').text('Edit Patient');
        $('#modalPatientId').val(patient.patientId);
        $('#modalUserId').val(user.userId);
        $('#modal-error').addClass('d-none').text('');
        $('#userName').val(user.name);
        $('#userEmail').val(user.email);
        $('#userUsername').val(user.username);
        $('#userPassword').val(user.password);
        $('#patientAge').val(patient.age);
        $('#patientGender').val(patient.gender);
        $('#patientContactInfo').val(patient.contactInfo);
        $('#patientEmergencyContact').val(patient.emergencyContact);
        $('#patientRoomNumber').val(patient.roomNumber);
        $('#patientAdmissionDate').val(patient.admissionDate ? patient.admissionDate : '');
        $('#assignedDoctor').val(patient.assignedDoctor);
        $('#patientModal').modal('show');
        $('#modalSaveBtn').off('click').on('click', function (e) {
            e.preventDefault();
            savePatient('edit');
        });
    });
}

function savePatient(mode) {
    $('#modal-error').addClass('d-none').text('');
    var payload = {
        UserId: $('#modalUserId').val(),
        Name: $('#userName').val(),
        Email: $('#userEmail').val(),
        Username: $('#userUsername').val(),
        Password: $('#userPassword').val(),
        PatientId: $('#modalPatientId').val(),
        Age: $('#patientAge').val(),
        Gender: $('#patientGender').val(),
        ContactInfo: $('#patientContactInfo').val(),
        EmergencyContact: $('#patientEmergencyContact').val(),
        RoomNumber: $('#patientRoomNumber').val(),
        AdmissionDate: $('#patientAdmissionDate').val(),
        AssignedDoctor: $('#assignedDoctor').val()
    };
    var user = {
        UserId: payload.UserId,
        Name: payload.Name,
        Email: payload.Email,
        Username: payload.Username,
        Password: payload.Password
    };
    var patient = {
        PatientId: payload.PatientId,
        UserId: payload.UserId,
        Name: payload.Name,
        Age: payload.Age,
        Gender: payload.Gender,
        ContactInfo: payload.ContactInfo,
        EmergencyContact: payload.EmergencyContact,
        RoomNumber: payload.RoomNumber,
        AdmissionDate: payload.AdmissionDate,
        AssignedDoctor: payload.AssignedDoctor
    };

    var url = mode === 'create' ? '/Patients/CreateModal' : '/Patients/EditModal';
    $.ajax({
        url: url,
        method: 'POST',
        data: { user: user, patient: patient },
        success: function (res) {
            if (res.success) {
                $('#patientModal').modal('hide');
                location.reload();
            } else {
                $('#modal-error').removeClass('d-none').text(res.message || 'Error occurred.');
            }
        },
        error: function (err) {
            $('#modal-error').removeClass('d-none').text('Server error.');
        }
    });
}

// ====== MEDICAL HISTORY MODALS FOR DETAILS VIEW ======
function showMedicalHistory(patientId) {
    $('#medicalHistoryModalBody').html('<div class="text-center py-4">Loading...</div>');
    $('#medicalHistoryModal').modal('show');
    fetch(`/Patients/GetMedicalHistory?patientId=${patientId}`)
        .then(resp => resp.json())
        .then(data => {
            let canManage = window.canManage;
            let rows = (data || []).map(h => `
                <tr>
                    <td>${h.startDate ? new Date(h.startDate).toLocaleDateString() : ''}</td>
                    <td>${h.diagnosis || ''}</td>
                    <td>${h.description || ''}</td>
                    <td>${h.notes || ''}</td>
                    ${canManage
                    ? `<td>
                            <button class="btn btn-sm btn-warning" onclick="openEditMedicalHistory(${h.historyId}, ${patientId})">Edit</button>
                           </td>`
                    : '<td></td>'
                }
                </tr>
            `).join('');
            let addBtn = canManage ? `<button class="btn btn-success mb-2" onclick="openEditMedicalHistory(0, ${patientId})">Add Medical History</button>` : '';
            $('#medicalHistoryModalBody').html(`
                ${addBtn}
                <table class="table table-bordered">
                    <thead>
                        <tr><th>Date</th><th>Diagnosis</th><th>Description</th><th>Notes</th><th></th></tr>
                    </thead>
                    <tbody>${rows}</tbody>
                </table>
            `);
        })
        .catch(err => {
            $('#medicalHistoryModalBody').html('<div class="text-danger">Failed to load medical history.</div>');
        });
}

// Open Medical History Edit/Add modal
function openEditMedicalHistory(historyId, patientId) {
    $('#mh-error').addClass('d-none').text('');
    $('#mh-form')[0].reset();
    $('#mh-id').val('');
    $('#mh-patient-id').val(patientId);
    if (historyId && historyId > 0) {
        // Edit
        $('#medicalHistoryEditModalLabel').text('Edit Medical History');
        fetch(`/Patients/GetMedicalHistoryItem?id=${historyId}`)
            .then(resp => resp.json())
            .then(res => {
                if (!res.success) return;
                $('#mh-id').val(res.item.historyId);
                $('#mh-date').val(res.item.startDate ? res.item.startDate.split('T')[0] : '');
                $('#mh-diagnosis').val(res.item.diagnosis || '');
                $('#mh-description').val(res.item.description || '');
                $('#mh-notes').val(res.item.notes || '');
                $('#medicalHistoryEditModal').modal('show');
            });
    } else {
        // Add
        $('#medicalHistoryEditModalLabel').text('Add Medical History');
        $('#mh-id').val('');
        $('#mh-date').val('');
        $('#mh-diagnosis').val('');
        $('#mh-description').val('');
        $('#mh-notes').val('');
        $('#medicalHistoryEditModal').modal('show');
    }
}

// Medical History Add/Edit submit
$(function () {
    $('#mh-form').submit(function (e) {
        e.preventDefault();
        $('#mh-error').addClass('d-none').text('');
        let payload = {
            patientId: $('#mh-patient-id').val(),
            medicalHistoryId: $('#mh-id').val(),
            diagnosis: $('#mh-diagnosis').val(),
            description: $('#mh-description').val(),
            startDate: $('#mh-date').val(),
            notes: $('#mh-notes').val()
        };
        $.post('/Patients/SaveMedicalHistory', payload, function (res) {
            if (res.success) {
                $('#medicalHistoryEditModal').modal('hide');
                // Refresh history in parent modal
                showMedicalHistory(payload.patientId);
            } else {
                $('#mh-error').removeClass('d-none').text(res.message || 'Failed to save.');
            }
        });
    });
});