var CheckoutController = function () {

    this.initialize = function () {
        registerEvents();
    }

    function registerEvents() {
        $('.CustomerName').focusout(function () {
            if ($('.CustomerName').val() == "") {
                $('.CustomerName').css('border-color', '#ECA8A8');
            }
            else {
                $('.CustomerName').css('border-color', '');
            }
        });
        $('.CustomerPhone').focusout(function () {
            if ($('.CustomerPhone').val() == "") {
                $('.CustomerPhone').css('border-color', '#ECA8A8');
            }
            else {
                $('.CustomerPhone').css('border-color', '');
            }
        });
        $('.CustomerEmail').focusout(function () {
            if ($('.CustomerEmail').val() == "") {
                $('.CustomerEmail').css('border-color', '#ECA8A8');
            }
            else {
                $('.CustomerEmail').css('border-color', '');
            }
        });
        $('.CustomerAddress').focusout(function () {
            if ($('.CustomerAddress').val() == "") {
                $('.CustomerAddress').css('border-color', '#ECA8A8');
            }
            else {
                $('.CustomerAddress').css('border-color', '');
            }
        });
        $('#btnPaypal').on('click', function (e) {
            e.preventDefault();
            var customerName = $('.CustomerName').val();
            var customerPhone = $('.CustomerPhone').val();
            var customerEmail = $('.CustomerEmail').val();
            var customerAddress = $('.CustomerAddress').val();
            if (customerName == "" || customerPhone == "" || customerEmail == "" || customerAddress == "") {
                if (customerAddress == "") {
                    $('.CustomerAddress').focus();
                    $('.CustomerAddress').css('border-color', '#ECA8A8');
                }
                else {
                    $('.CustomerAddress').css('border-color', '');
                }
                if (customerEmail == "") {
                    $('.CustomerEmail').focus();
                    $('.CustomerEmail').css('border-color', '#ECA8A8');
                }
                else {
                    $('.CustomerEmail').css('border-color', '');
                }
                if (customerPhone == "") {
                    $('.CustomerPhone').focus();
                    $('.CustomerPhone').css('border-color', '#ECA8A8');
                }
                else {
                    $(".CustomerPhone").css('border-color', '');
                }
                if (customerName == "") {
                    $('.CustomerName').focus();
                    $('.CustomerName').css('border-color', '#ECA8A8');
                }
                else {
                    $('.CustomerName').css('border-color', '');
                }
            }
            else {
                var customerMessage = $('.CustomerMessage').val();
                var paymentMethod = 3;
                $.ajax({
                    url: '/Cart/CheckoutAjax',
                    type: 'post',
                    data: {
                        CustomerName: customerName,
                        CustomerPhone: customerPhone,
                        CustomerAddress: customerAddress,
                        CustomerEmail: customerEmail,
                        CustomerMessage: customerMessage,
                        PaymentMethod: paymentMethod
                    },
                    dataType: "json",
                    beforeSend: function () {
                        atom.startLoading();
                    },
                    success: function () {
                        atom.notify('successful.', 'success');
                        $('#frmCheckoutPaypal').submit();
                        atom.stopLoading();

                    },
                    error: function () {
                        atom.notify('failure', 'error');
                        atom.stopLoading();
                    }
                });
            }
           
            
        });
        $('#btnPlaceOrder').on('click', function () {
            var customerName = $('.CustomerName').val();
            var customerPhone = $('.CustomerPhone').val();
            var customerEmail = $('.CustomerEmail').val();
            var customerAddress = $('.CustomerAddress').val();
            if (customerName == "" || customerPhone == "" || customerEmail == "" || customerAddress == "") {
                if (customerAddress == "") {
                    $('.CustomerAddress').focus();
                    $('.CustomerAddress').css('border-color', '#ECA8A8');
                }
                else {
                    $('.CustomerAddress').css('border-color', '');
                }
                if (customerEmail == "") {
                    $('.CustomerEmail').focus();
                    $('.CustomerEmail').css('border-color', '#ECA8A8');
                }
                else {
                    $('.CustomerEmail').css('border-color', '');
                }
                if (customerPhone == "") {
                    $('.CustomerPhone').focus();
                    $('.CustomerPhone').css('border-color', '#ECA8A8');
                }
                else {
                    $(".CustomerPhone").css('border-color', '');
                }
                if (customerName == "") {
                    $('.CustomerName').focus();
                    $('.CustomerName').css('border-color', '#ECA8A8');
                }
                else {
                    $('.CustomerName').css('border-color', '');
                }
            }
            else {
                
                var customerMessage = $('.CustomerMessage').val();
                var paymentMethod = $("#paymentMethodul input[type='radio']:checked").val();
                if (paymentMethod == 3) {
                    $.ajax({
                        url: '/Cart/CheckoutAjax',
                        type: 'post',
                        data: {
                            CustomerName: customerName,
                            CustomerPhone: customerPhone,
                            CustomerAddress: customerAddress,
                            CustomerEmail: customerEmail,
                            CustomerMessage: customerMessage,
                            PaymentMethod: paymentMethod
                        },
                        dataType: "json",
                        beforeSend: function () {
                            atom.startLoading();
                        },
                        success: function () {
                            atom.notify('successful.', 'success');
                            $('#frmCheckoutPaypal').submit();
                            atom.stopLoading();

                        },
                        error: function () {
                            atom.notify('failure', 'error');
                            atom.stopLoading();
                        }
                    });
                }
                else {
                    $('#frmCheckout').submit();
                }
            }


        });
        
    }
}