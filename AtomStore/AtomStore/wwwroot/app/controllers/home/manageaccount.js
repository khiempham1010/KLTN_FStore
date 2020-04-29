var manageaccount = function () {
    this.initialize = function () {
        registerEvents();
        loadAccount();
    }

    function registerEvents() {
        $('#btnAccount').on('click', function () {
            loadAccount();
            $('#modal-account-manage').modal('show');
        });
        //$("#btnSaveAccount").on('click', function (e) {
        //    if ($('#frmMaintainance').valid()) {
        //        e.preventDefault();
        //        var FullName = $('#txtFullName').val();
        //        var Email = $('#txtEmail');
        //        var BirthDay = $('#txtBirthDay').val();
        //        var Address = $('#txtAddress').val();
        //        var Phone = $('#txtPhone').val();

        //        $.ajax({
        //            url: '/admin/User/SaveUser',
        //            data: {
        //                FullName: FullName,
        //                Email: Email,
        //                BirthDay: BirthDay,
        //                Address: Address,
        //                Phone, Phone
        //            },
        //            beforeSend: function () {
        //                atom.startLoading();
        //            },
        //            type: 'post',
        //            dataType: 'json',
        //            success: function (response) {
        //                atom.notify('Update Account successful', 'success');
        //                $('#modal-account-manage').modal('hide');
        //                resetFormMaintainance();

        //                atom.stopLoading();
        //            },
        //            error: function () {
        //                atom.notify('Has an error in save product progress', 'error');
        //                atom.stopLoading();
        //            }
        //        });
        //    }

        //});
    }
    function loadAccount() {
        $.ajax({
            url: '/admin/User/GetUser',
            type: 'get',
            dataType: 'json',
            beforeSend: function () {
                atom.startLoading();
            },
            success: function (response) {
                var data = response;
                $('#txtFullName').val(data.FullName);
                $('#txtEmail').val(data.Email);
                $('#txtBirthDay').val(atom.dateFormatJson2(data.BirthDay));
                $('#txtAddress').val(data.Address);
                $('#txtPhone').val(data.PhoneNumber);

            },
            error: function (status) {
                atom.notify('Has an error in progress', 'error');
                atom.stopLoading();
            }
        });
    }
}