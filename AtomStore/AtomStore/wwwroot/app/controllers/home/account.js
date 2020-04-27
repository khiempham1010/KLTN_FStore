var Account = function () {
    var self = this;
    var parent = parent;
    this.initialize = function () {
        registerEvents();
    }
    function registerEvents(){
        $("#btnSaveAccount").on('click', function (e) {
            if ($('#frmMaintainance').valid()) {
                e.preventDefault();
                var FullName = $('#txtFullName').val();
                var Email = $('#txtEmail').val();
                var BirthDay = $('#txtBirthDay').val();
                var Address = $('#txtAddress').val();
                var Phone = $('#txtPhone').val();

                $.ajax({
                    url: '/admin/User/SaveUser',
                    type: 'POST',
                    data: {
                        FullName: FullName,
                        Email: Email,
                        BirthDay: BirthDay,
                        Address: Address,
                        PhoneNumber: Phone
                    },
                    dataType: 'json',
                    beforeSend: function () {
                        atom.startLoading();
                    },
                    success: function (response) {
                        atom.notify('Update Account successful', 'success');
                        $('#modal-account-manage').modal('hide');
                        atom.stopLoading();
                    },
                    error: function () {
                        atom.notify('Has an error in save product progress', 'error');
                        atom.stopLoading();
                    }
                });
            }

        });
    }
}
