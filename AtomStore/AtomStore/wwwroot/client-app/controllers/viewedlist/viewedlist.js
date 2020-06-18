var ViewedlistController = function () {
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
        $('body').on('click', '.btnAddToWishlist', function (e) {
            e.preventDefault();
            var id = parseInt($(this).data('id'));
            $.ajax({
                url: '/Product/AddWishlist',
                type: 'post',
                dataType: 'json',
                data: {
                    productId: id
                },
                success: function () {
                    atom.notify('Product was added successful', 'success');
                },
                error: function () {
                    atom.notify('Log in to add product', 'error');
                    atom.stopLoading();
                }
            });
        });
    }

    function loadData() {
        $.ajax({
            url: '/Viewedlist/GetAllPaging',
            type: 'GET',
            data: {
                page: atom.configs.pageIndex
            },
            dataType: 'json',
            success: function (response) {
                var template = $('#template-viewedlist').html();
                var render = "";
                $.each(response, function (i, product) {
                    $.each(product, function (i, item) {
                        render += Mustache.render(template,
                            {
                                Id: item.Id,
                                Url: '/' + item.Product.SeoAlias + "-p." + item.Product.Id + ".html",
                                ProductName: item.ProductName,
                                Image: item.Product.Image,
                                Price: atom.formatNumber(item.Product.Price, 0) + ".0",
                                PromotionPrice: item.Product.PromotionPrice == null ? '<p class="special-price"> <span class="price-label">Special Price</span> <span class="price">' + "$" + atom.formatNumber(item.Product.Price, 0) + ".0" + '</span> </p>' :
                                    '<p class="special-price"> <span class="price-label">Special Price</span> <span class="price">' + "$" + atom.formatNumber(item.Product.PromotionPrice, 0) + ".0" + '</span> </p>' +
                                    '<p class= "old-price" > <span class="price-label">Regular Price:</span> <span class="price">' + "$" + atom.formatNumber(item.Product.Price, 0) + ".0" + '</span> </p>' +
                                    '<p class="special-price1"> <span class="price-label">% discount</span> <span class="price">' + Math.round(100 - ((item.Product.PromotionPrice / item.Product.Price) * 100)) + "%" + '</span> </p>',
                                ProductId: item.Product.Id
                            });
                    });
                });
                if (render !== "")
                    $('.products-grid').html(render);
                else {
                    $('.product-item').html('There are no items in your viewed list.');
                }
                wrapPaging(response.RowCount, function () {
                    loadData()
                });
            },
            //error: function (e) {
            //    console.log(e);
            //    atom.notify('Has an error in progress', 'error');
            //}
        });
        return false;
    }
    function wrapPaging(RecordCount, callBack) {
        var totalSize = Math.ceil(RecordCount / atom.configs.pageSize);
        if ($('#paginationUL a').length === 0) {
            $('#paginationUL').empty();
            $('#paginationUL').removeData("twbs-pagination");
            $('#paginationUL').unbind("page");
        }
        $('.paginationUL').twbsPagination({
            totalPages: totalSize,
            visiblePages: 7,
            first: '',
            prev: '<',
            next: '>',
            last: '',
            onPageClick: function (event, p) {
                atom.configs.pageIndex = p;
                setTimeout(callBack(), 200);
            }
        });
    }
}