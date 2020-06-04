var WishlistController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
        
    }

    function registerEvents() {
        $('body').on('click', '#btnAddToCart', function (e) {
            e.preventDefault();
            var id = parseInt($(this).data('Id'));
            $.ajax({
                url: '/Cart/AddToCart',
                type: 'post',
                dataType: 'json',
                data: {
                    productId: id,
                    quantity: 1
                },
                success: function () {
                    atom.notify('Product was added successful', 'success');
                    loadData()
                }
            });
        });
        $('body').on('click', '#btnRemove', function (e) {
            e.preventDefault();
            var id = parseInt($(this).data('id'));
            $.ajax({
                url: '/Wishlist/Remove',
                type: 'post',
                dataType: 'json',
                data: {
                    wishlistId : id
                },
                success: function () {
                    atom.notify('Product was added successful', 'success');
                    loadData()
                }
            })
        })
    }
    function loadData() {
        $.ajax({
            url: '/Wishlist/GetWishlist',
            type: 'GET',
            dataType: 'json',
            success: function (response) {
                var template = $('#template-wishlist').html();
                var render = "";
                $.each(response, function (i, item) {
                    render += Mustache.render(template,
                        {
                            Id: item.Id,
                            Url: '/' + item.Product.SeoAlias + "-p." + item.Product.Id + ".html",
                            ProductName: item.ProductName,
                            ProductName: item.ProductName,
                            Image: item.Product.Image,
                            Price: atom.formatNumber(item.Product.Price, 0)+".0",
                            ProductId: item.Product.Id
                        });
                });
                if (render !== "")
                    $('#table-wishlist-content').html(render);
                else {
                    $('.table-responsive').html('There are no items in your Wishlist.');
                }
            },
            error: function (e) {
                console.log(e);
                atom.notify('Has an error in progress', 'error');
            }
        });
        return false;
    }
}