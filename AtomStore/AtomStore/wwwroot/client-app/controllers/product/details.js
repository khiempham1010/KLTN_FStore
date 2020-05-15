var ProductDetailController = function () {
    this.initialize = function () {
        registerEvents();
    }

    function registerEvents() {
        $('#btnAddToCart').on('click', function (e) {
            e.preventDefault();
            var id = parseInt($(this).data('id'));
            var colorId = parseInt($('#ddlColorId').val());
            var sizeId = parseInt($('#ddlSizeId').val());
            $.ajax({
                url: '/Cart/AddToCart',
                type: 'post',
                dataType: 'json',
                data: {
                    productId: id,
                    quantity: parseInt($('#txtQuantity').val()),
                    color: colorId,
                    size: sizeId
                },
                success: function () {
                    atom.notify('Product was added successful', 'success');
                    loadHeaderCart();
                }
            });
        });
        $('#btnAddToWishlist').on('click', function (e) {
            e.preventDefault();
            var id = parseInt($(this).data('id'));
            var corlor = $('#btnAddToWishlist').css("color");
            if (corlor=="rgb(51, 51, 51)") {
                $.ajax({
                    url: '/Product/AddWishlist',
                    type: 'post',
                    dataType: 'json',
                    data: {
                        productId: id
                    },
                    success: function () {
                        atom.notify('Product was added successful', 'success');
                        loadHeaderCart();
                        $('#btnAddToWishlist').css('color', 'red');
                    },
                    error: function () {
                        atom.notify('Log in to add product', 'error');
                        atom.stopLoading();
                    }
                });
            }
            else {
                $.ajax({
                    url: '/Product/AddWishlist',
                    type: 'post',
                    dataType: 'json',
                    data: {
                        productId: id
                    },
                    success: function () {
                        atom.notify('Product was added successful', 'success');
                        loadHeaderCart();
                        $('#btnAddToWishlist').css('color', 'black');
                    },
                    error: function () {
                        atom.notify('Log in to add product', 'error');
                        atom.stopLoading();
                    }
                });
            }
        });
    }

    function loadHeaderCart() {
        $("#headerCart").load("/AjaxContent/HeaderCart");
    }
}