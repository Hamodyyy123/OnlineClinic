// ====== DOCTOR LIST/MANAGE MODALS (Admin only) ======

// Open add modal
function openCreateDoctorModal() {
    $('#doctorModalLabel').text('Add New Doctor');
    $('#doctorModal-error').addClass('d-none').text('');
    $('#doctorId').val('');
    $('#userId').val('');
    $('#doctorName').val('');
    $('#doctorEmail').val('');
    $('#doctorUsername').val('');
    $('#doctorPassword').val('');
    $('#doctorPhone').val('');
    $('#doctorSpeciality').val('');
    $('#doctorModal').modal('show');
    $('#doctorModalSaveBtn').off('click').on('click', function (e) {
        e.preventDefault();
        saveDoctor('create');
    });
}

// Open edit modal
function openEditDoctorModal(doctorId) {
    $.getJSON(`/Doctors/GetDoctorUser?doctorId=${doctorId}`, function (res) {
        if (!res.success) {
            $('#doctorModal-error').removeClass('d-none').text('Failed to load doctor.');
            return;
        }
        var user = res.user;
        var doctor = res.doctor;
        $('#doctorModalLabel').text('Edit Doctor');
        $('#doctorId').val(doctor.doctorId);
        $('#userId').val(user.userId);
        $('#doctorName').val(user.name);
        $('#doctorEmail').val(user.email);
        $('#doctorUsername').val(user.username);
        $('#doctorPassword').val(user.password);
        $('#doctorPhone').val(doctor.phone);
        $('#doctorSpeciality').val(doctor.speciality);
        $('#doctorModal-error').addClass('d-none').text('');
        $('#doctorModal').modal('show');
        $('#doctorModalSaveBtn').off('click').on('click', function (e) {
            e.preventDefault();
            saveDoctor('edit');
        });
    });
}

// Save doctor (create/edit)
function saveDoctor(mode) {
    $('#doctorModal-error').addClass('d-none').text('');
    var doctorId = $('#doctorId').val();
    var userId = $('#userId').val();
    var name = $('#doctorName').val();
    var email = $('#doctorEmail').val();
    var username = $('#doctorUsername').val();
    var password = $('#doctorPassword').val();
    var phone = $('#doctorPhone').val();
    var speciality = $('#doctorSpeciality').val();

    var url, data;
    if (mode === 'create') {
        url = '/Doctors/CreateDoctorModal';
        data = { name, email, username, password, phone, speciality };
    } else {
        url = '/Doctors/EditDoctorModal';
        data = { doctorId, userId, name, email, username, password, phone, speciality };
    }

    $.ajax({
        url: url,
        method: 'POST',
        data: data,
        success: function (res) {
            if (res.success) {
                $('#doctorModal').modal('hide');
                location.reload();
            } else {
                $('#doctorModal-error').removeClass('d-none').text(res.message || 'Error occurred.');
            }
        },
        error: function (err) {
            $('#doctorModal-error').removeClass('d-none').text('Server error.');
        }
    });
}

// Delete doctor
function deleteDoctor(doctorId) {
    if (!confirm('Are you sure you want to delete this doctor?')) return;
    $.ajax({
        url: '/Doctors/DeleteDoctor',
        method: 'POST',
        data: { doctorId: doctorId },
        success: function (res) {
            if (res.success) {
                location.reload();
            } else {
                alert(res.message || 'Error occurred.');
            }
        },
        error: function (err) {
            alert('Server error.');
        }
    });
}