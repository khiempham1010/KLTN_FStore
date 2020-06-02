var CheckoutController = function () {

    this.initialize = function () {
        registerEvents();
    }

    function registerEvents() {
        $('#btnCheckout').on('click', function (e) {
            e.preventDefault();
            var customerName = $('.CustomerName').val();
            var customerPhone = $('.CustomerPhone').val();
            var customerEmail = $('.CustomerEmail').val();
            var customerAddress = $('.CustomerAddress').val();
            var customerMessage = $('.CustomerMessage').val();
            var paymentMethod = $("#paymentMethodul input[type='radio']:checked").val();
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
            
        });
        
    }
}