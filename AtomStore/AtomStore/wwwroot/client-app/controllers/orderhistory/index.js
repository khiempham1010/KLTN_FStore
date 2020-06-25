var HistoryController = function () {
    this.initialize = function () {
        registerEvents();
    }
    function registerEvents() {
        $('body').on('click', '.btnCancel', function (e) {
            e.preventDefault();
            var id = parseInt($(this).data('id'));
            $.ajax({
                url: '/OrderHistory/CancelOrder',
                type: 'post',
                dataType: 'json',
                data: {
                    orderId: id
                },
                success: function () {
                    atom.notify('Order was canceled successful', 'success');
                    var a = $(this).parent();
                    a.parent().find('.tdStatus').find('.orderStatus').text("Canceled")
                }
            });
        });
    }
}