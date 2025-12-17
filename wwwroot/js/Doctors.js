// wwwroot/js/Doctors.js
// Manage doctors: open add/edit modals and save/delete via AJAX
// Includes read-only guard so Patients (or mode=view) cannot modify data.

console.log("Doctors.js loaded");

(function () {
    // Determine read-only mode from wrapper data attribute or URL param
    function pageReadOnly() {
        try {
            const wrapper = document.querySelector('.doctors-page-wrapper');
            if (wrapper && wrapper.dataset.readonly === 'true') return true;
            const params = new URLSearchParams(window.location.search);
            if (params.get('mode') === 'view') return true;
        } catch (e) { /* ignore */ }
        return false;
    }

    const READ_ONLY = pageReadOnly();

    // Utility to show readonly notice
    function showReadOnlyNotice() {
        // small non-blocking notice
        const msg = 'Read-only: you do not have permission to modify doctors.';
        if (typeof toastr !== 'undefined') {
            toastr.info(msg);
        } else {
            // fallback
            alert(msg);
        }
    }

    // If read-only, hide any edit/delete buttons that slipped through
    document.addEventListener('DOMContentLoaded', function () {
        if (READ_ONLY) {
            document.querySelectorAll('.doctors-table .btn-warning, .doctors-table .btn-danger, .btn-add-doctor').forEach(el => {
                el.style.display = 'none';
            });
            // Also disable save button inside modal to be safe
            const saveBtn = document.getElementById('doctorModalSaveBtn');
            if (saveBtn) {
                saveBtn.dataset.readonly = 'true';
            }
        }
    });

    // Open add modal
    window.openCreateDoctorModal = function () {
        if (READ_ONLY) { showReadOnlyNotice(); return; }

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
    };

    // Open edit modal
    window.openEditDoctorModal = function (doctorId) {
        if (READ_ONLY) { showReadOnlyNotice(); return; }

        $('#doctorModal-error').addClass('d-none').text('');

        $.getJSON(`/Doctors/GetDoctorUser?doctorId=${doctorId}`, function (res) {
            if (!res || !res.success) {
                $('#doctorModal-error').removeClass('d-none').text(res?.message || 'Failed to load doctor.');
                return;
            }

            var user = res.user || {};
            var doctor = res.doctor || {};

            $('#doctorModalLabel').text('Edit Doctor');
            $('#doctorId').val(doctor.doctorId || '');
            $('#userId').val(user.userId || '');
            $('#doctorName').val(user.name || '');
            $('#doctorEmail').val(user.email || '');
            $('#doctorUsername').val(user.username || '');
            $('#doctorPassword').val(user.password || '');
            $('#doctorPhone').val(doctor.phone || '');
            $('#doctorSpeciality').val(doctor.speciality || '');

            $('#doctorModal-error').addClass('d-none').text('');
            $('#doctorModal').modal('show');

            $('#doctorModalSaveBtn').off('click').on('click', function (e) {
                e.preventDefault();
                saveDoctor('edit');
            });
        }).fail(function (xhr) {
            console.error("GET /Doctors/GetDoctorUser failed", xhr);
            $('#doctorModal-error').removeClass('d-none').text('Server error loading doctor.');
        });
    };

    // Save doctor (create/edit)
    window.saveDoctor = function (mode) {
        if (READ_ONLY) { showReadOnlyNotice(); return; }

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
                if (res && res.success) {
                    $('#doctorModal').modal('hide');
                    location.reload();
                } else {
                    $('#doctorModal-error').removeClass('d-none').text(res?.message || 'Error occurred.');
                }
            },
            error: function (err) {
                console.error("Save doctor failed", err);
                $('#doctorModal-error').removeClass('d-none').text('Server error.');
            }
        });
    };

    // Delete doctor
    window.deleteDoctor = function (doctorId) {
        if (READ_ONLY) { showReadOnlyNotice(); return; }

        if (!confirm('Are you sure you want to delete this doctor?')) return;

        $.ajax({
            url: '/Doctors/DeleteDoctor',
            method: 'POST',
            data: { doctorId: doctorId },
            success: function (res) {
                if (res && res.success) {
                    location.reload();
                } else {
                    alert(res?.message || 'Error occurred.');
                }
            },
            error: function (err) {
                console.error("Delete doctor failed", err);
                alert('Server error.');
            }
        });
    };

})();