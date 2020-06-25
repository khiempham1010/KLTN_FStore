var OrderController = function () {
    var cachedObj = {
        products: [],
        colors: [],
        sizes: [],
        paymentMethods: [],
        orderStatuses: []
    }
    this.initialize = function () {
        $.when(loadOrderStatus(),
            loadPaymentMethod(),
            loadColors(),
            loadSizes(),
            loadProducts())
            .done(function () {
                loadData();
            });

        registerEvents();
    }

    function registerEvents() {
        $('#txtFromDate, #txtToDate').datepicker({
            autoclose: true,
            format: 'dd/mm/yyyy'
        });
        //Init validation
        $('#frmMaintainance').validate({
            errorClass: 'red',
            ignore: [],
            lang: 'vi',
            rules: {
                txtCustomerName: { required: true },
                txtCustomerAddress: { required: true },
                txtCustomerPhone: { required: true },
                ddlOrderStatus: { required: true }
            }
        });
        $('#txt-search-keyword').keypress(function (e) {
            if (e.which === 13) {
                e.preventDefault();
                loadData();
            }
        });
        $("#btn-search").on('click', function () {
            loadData();
        });

        $("#btn-create").on('click', function () {
            resetFormMaintainance();
            $('#modal-detail').modal('show');
        });
        $("#ddl-show-page").on('change', function () {
            atom.configs.pageSize = $(this).val();
            atom.configs.pageIndex = 1;
            loadData(true);
        });

        $('body').on('click', '.btn-view', function (e) {
            e.preventDefault();
            var that = $(this).data('id');
            $.ajax({
                type: "GET",
                url: "/Admin/Order/GetById",
                data: { id: that },
                beforeSend: function () {
                    atom.startLoading();
                },
                success: function (response) {
                    var data = response;
                    $('#hidId').val(data.Id);
                    $('#txtCustomerName').val(data.CustomerName);

                    $('#txtCustomerAddress').val(data.CustomerAddress);
                    $('#txtCustomerPhone').val(data.CustomerPhone);
                    $('#txtCustomerEmail').val(data.CustomerEmail);
                    $('#txtCustomerMessage').val(data.CustomerMessage);
                    $('#ddlPaymentMethod').val(data.PaymentMethod);
                    $('#ddlCustomerId').val(data.CustomerId);
                    $('#ddlOrderStatus').val(data.OrderStatus);
                    $('#lbldatecreated').val(data.DateCreated)

                    var orderDetails = data.OrderDetails;
                    if (data.OrderDetails != null && data.OrderDetails.length > 0) {
                        var render = '';
                        var templateDetails = $('#template-table-order-details').html();

                        $.each(orderDetails, function (i, item) {
                            var products = getProductOptions(item.ProductId);
                            var colors = getColorOptions(item.ColorId);
                            var sizes = getSizeOptions(item.SizeId);

                            render += Mustache.render(templateDetails,
                                {
                                    Id: item.Id,
                                    Products: products,
                                    Colors: colors,
                                    Sizes: sizes,
                                    Quantity: item.Quantity
                                });
                        });
                        $('#tbl-order-details').html(render);
                    }
                    $('#modal-detail').modal('show');
                    atom.stopLoading();

                },
                error: function (e) {
                    atom.notify('Has an error in progress', 'error');
                    atom.stopLoading();
                }
            });
        });
        $('#btnConfirm').on('click', function (e) {
            if ($('#frmMaintainance').valid()) {
                var id = $('#hidId').val();
                var customerName = $('#txtCustomerName').val();
                var customerAddress = $('#txtCustomerAddress').val();
                var customerId = $('#ddlCustomerId').val();
                var customerPhone = $('#txtCustomerPhone').val();
                var customerMessage = $('#txtCustomerMessage').val();
                var paymentMethod = $('#ddlPaymentMethod').val();
                var orderStatus = $('#ddlOrderStatus').val();
                //order detail

                var orderDetails = [];
                $.each($('#tbl-order-details tr'), function (i, item) {
                    orderDetails.push({
                        Id: $(item).data('id'),
                        ProductId: $(item).find('select.ddlProductId').first().val(),
                        Quantity: $(item).find('input.txtQuantity').first().val(),
                        ColorId: $(item).find('select.ddlColorId').first().val(),
                        SizeId: $(item).find('select.ddlSizeId').first().val(),
                        OrderId: id
                    });
                });

                $.ajax({
                    type: "POST",
                    url: "/Admin/Order/SendMail",
                    data: {
                        Id: id,
                        OrderStatus: 1,
                        CustomerAddress: customerAddress,
                        CustomerId: customerId,
                        CustomerMessage: customerMessage,
                        CustomerPhone: customerPhone,
                        CustomerName: customerName,
                        PaymentMethod: paymentMethod,
                        Status: 1,
                        OrderDetails: orderDetails
                    },
                    dataType: "json",
                    beforeSend: function () {
                        atom.startLoading();
                    },
                    success: function (response) {
                        atom.notify('Save order successful', 'success');
                        $('#modal-detail').modal('hide');
                        resetFormMaintainance();

                        atom.stopLoading();
                        loadData(true);
                    },
                    error: function () {
                        atom.notify('Has an error in progress', 'error');
                        atom.stopLoading();
                    }
                });
                return false;
            }

        });
        $('#btnCompleted').on('click', function (e) {
            if ($('#frmMaintainance').valid()) {
                var id = $('#hidId').val();
                $.ajax({
                    type: "POST",
                    url: "/Admin/Order/ChangeOrderStatus",
                    data: {
                        Id: id,
                        OrderStatus: 4
                    },
                    dataType: "json",
                    beforeSend: function () {
                        atom.startLoading();
                    },
                    success: function (response) {
                        atom.notify('Save order successful', 'success');
                        $('#modal-detail').modal('hide');
                        resetFormMaintainance();

                        atom.stopLoading();
                        loadData(true);
                    },
                    error: function () {
                        atom.notify('Has an error in progress', 'error');
                        atom.stopLoading();
                    }
                });
                return false;
            }

        });

        $('#btnCancelOrder').on('click', function (e) {
            if ($('#frmMaintainance').valid()) {
                var id = $('#hidId').val();

                $.ajax({
                    type: "POST",
                    url: "/Admin/Order/ChangeOrderStatus",
                    data: {
                        Id: id,
                        OrderStatus: 3
                    },
                    dataType: "json",
                    beforeSend: function () {
                        atom.startLoading();
                    },
                    success: function (response) {
                        atom.notify('Save order successful', 'success');
                        $('#modal-detail').modal('hide');
                        resetFormMaintainance();

                        atom.stopLoading();
                        loadData(true);
                    },
                    error: function () {
                        atom.notify('Has an error in progress', 'error');
                        atom.stopLoading();
                    }
                });
                return false;
            }

        });
        $('#btnReturned').on('click', function (e) {
            if ($('#frmMaintainance').valid()) {
                var id = $('#hidId').val();

                $.ajax({
                    type: "POST",
                    url: "/Admin/Order/ChangeOrderStatus",
                    data: {
                        Id: id,
                        OrderStatus: 2
                    },
                    dataType: "json",
                    beforeSend: function () {
                        atom.startLoading();
                    },
                    success: function (response) {
                        atom.notify('Save order successful', 'success');
                        $('#modal-detail').modal('hide');
                        resetFormMaintainance();

                        atom.stopLoading();
                        loadData(true);
                    },
                    error: function () {
                        atom.notify('Has an error in progress', 'error');
                        atom.stopLoading();
                    }
                });
                return false;
            }

        });

        $('#btnSave').on('click', function (e) {
            if ($('#frmMaintainance').valid()) {
                e.preventDefault();
                var id = $('#hidId').val();
                var customerName = $('#txtCustomerName').val();
                var customerAddress = $('#txtCustomerAddress').val();
                var customerId = $('#ddlCustomerId').val();
                var customerPhone = $('#txtCustomerPhone').val();
                var customerEmail = $('#txtCustomerEmail').val();
                var customerMessage = $('#txtCustomerMessage').val();
                var paymentMethod = $('#ddlPaymentMethod').val();
                var orderStatus = $('#ddlOrderStatus').val();
                var dateCreated = $('#lbldatecreated').val();
                if (dateCreated == "")
                    dateCreated = UniqueDateTime();
                //order detail

                var orderDetails = [];
                $.each($('#tbl-order-details tr'), function (i, item) {
                    orderDetails.push({
                        Id: $(item).data('id'),
                        ProductId: $(item).find('select.ddlProductId').first().val(),
                        Quantity: $(item).find('input.txtQuantity').first().val(),
                        ColorId: $(item).find('select.ddlColorId').first().val(),
                        SizeId: $(item).find('select.ddlSizeId').first().val(),
                        OrderId: id
                    });
                });

                $.ajax({
                    type: "POST",
                    url: "/Admin/Order/SaveEntity",
                    data: {
                        Id: id,
                        OrderStatus: orderStatus,
                        CustomerAddress: customerAddress,
                        CustomerId: customerId,
                        CustomerMessage: customerMessage,
                        CustomerPhone: customerPhone,
                        CustomerName: customerName,
                        customerEmail: customerEmail,
                        PaymentMethod: paymentMethod,
                        Status: 1,
                        OrderDetails: orderDetails,
                        DateCreated: dateCreated

                    },
                    dataType: "json",
                    beforeSend: function () {
                        atom.startLoading();
                    },
                    success: function (response) {
                        atom.notify('Save order successful', 'success');
                        $('#modal-detail').modal('hide');
                        resetFormMaintainance();

                        atom.stopLoading();
                        loadData(true);
                    },
                    error: function () {
                        atom.notify('Has an error in progress', 'error');
                        atom.stopLoading();
                    }
                });
                return false;
            }

        });

        $('#btnAddDetail').on('click', function () {
            var template = $('#template-table-order-details').html();
            var products = getProductOptions(null);
            var colors = getColorOptions(null);
            var sizes = getSizeOptions(null);
            var render = Mustache.render(template,
                {
                    Id: 0,
                    Products: products,
                    Colors: colors,
                    Sizes: sizes,
                    Quantity: 0,
                    Total: 0
                });
            $('#tbl-order-details').append(render);
        });

        $('body').on('click', '.btn-delete-detail', function () {
            $(this).parent().parent().remove();
        });

        $("#btnExport").on('click', function () {
            var that = $('#hidId').val();
            $.ajax({
                type: "POST",
                url: "/Admin/Order/ExportExcel",
                data: { orderId: that },
                beforeSend: function () {
                    atom.startLoading();
                },
                success: function (response) {
                    window.location.href = response;

                    atom.stopLoading();

                }
            });
        });
    };

    function loadOrderStatus() {
        return $.ajax({
            type: "GET",
            url: "/admin/order/GetOrderStatus",
            dataType: "json",
            success: function (response) {
                cachedObj.orderStatuses = response;
                var render = "";
                $.each(response, function (i, item) {
                    render += "<option value='" + item.Value + "'>" + item.Name + "</option>";
                });
                $('#ddlOrderStatus').html(render);
            }
        });
    }

    function loadPaymentMethod() {
        return $.ajax({
            type: "GET",
            url: "/admin/order/GetPaymentMethod",
            dataType: "json",
            success: function (response) {
                cachedObj.paymentMethods = response;
                var render = "";
                $.each(response, function (i, item) {
                    render += "<option value='" + item.Value + "'>" + item.Name + "</option>";
                });
                $('#ddlPaymentMethod').html(render);
            }
        });
    }

    function loadProducts() {
        return $.ajax({
            type: "GET",
            url: "/Admin/Product/GetAll",
            dataType: "json",
            success: function (response) {
                cachedObj.products = response;
            },
            error: function () {
                atom.notify('Has an error in progress', 'error');
            }
        });
    }

    function loadColors() {
        return $.ajax({
            type: "GET",
            url: "/Admin/Order/GetColors",
            dataType: "json",
            success: function (response) {
                cachedObj.colors = response;
            },
            error: function () {
                atom.notify('Has an error in progress', 'error');
            }
        });
    }

    function loadSizes() {
        return $.ajax({
            type: "GET",
            url: "/Admin/Order/GetSizes",
            dataType: "json",
            success: function (response) {
                cachedObj.sizes = response;
            },
            error: function () {
                atom.notify('Has an error in progress', 'error');
            }
        });
    }

    function getProductOptions(selectedId) {
        var products = "<select class='form-control ddlProductId'>";
        $.each(cachedObj.products, function (i, product) {
            if (selectedId === product.Id)
                products += '<option value="' + product.Id + '" selected="select">' + product.Name + '</option>';
            else
                products += '<option value="' + product.Id + '">' + product.Name + '</option>';
        });
        products += "</select>";
        return products;
    }

    function getColorOptions(selectedId) {
        var colors = "<select class='form-control ddlColorId'>";
        $.each(cachedObj.colors, function (i, color) {
            if (selectedId === color.Id)
                colors += '<option value="' + color.Id + '" selected="select">' + color.Name + '</option>';
            else
                colors += '<option value="' + color.Id + '">' + color.Name + '</option>';
        });
        colors += "</select>";
        return colors;
    }

    function getSizeOptions(selectedId) {
        var sizes = "<select class='form-control ddlSizeId'>";
        $.each(cachedObj.sizes, function (i, size) {
            if (selectedId === size.Id)
                sizes += '<option value="' + size.Id + '" selected="select">' + size.Name + '</option>';
            else
                sizes += '<option value="' + size.Id + '">' + size.Name + '</option>';
        });
        sizes += "</select>";
        return sizes;
    }

    function resetFormMaintainance() {
        $('#hidId').val(0);
        $('#txtCustomerName').val('');

        $('#txtCustomerAddress').val('');
        $('#txtCustomerPhone').val('');
        $('#txtCustomerMessage').val('');
        $('#ddlPaymentMethod').val('');
        $('#ddlCustomerId').val('');
        $('#ddlOrderStatus').val('');
        $('#tbl-order-details').html('');
    }

    function loadData(isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/admin/order/GetAllPaging",
            data: {
                startDate: $('#txtFromDate').val(),
                endDate: $('#txtToDate').val(),
                keyword: $('#txtSearchKeyword').val(),
                page: atom.configs.pageIndex,
                pageSize: atom.configs.pageSize
            },
            dataType: "json",
            beforeSend: function () {
                atom.startLoading();
            },
            success: function (response) {
                var template = $('#table-template').html();
                var render = "";
                if (response.RowCount > 0) {
                    $.each(response.Results, function (i, item) {
                        render += Mustache.render(template, {
                            CustomerName: item.CustomerName,
                            Id: item.Id,
                            PaymentMethod: getPaymentMethodName(item.PaymentMethod),
                            DateCreated: atom.dateTimeFormatJson(item.DateCreated),
                            OrderStatus: getOrderStatusName(item.OrderStatus)
                        });
                    });
                    $("#lbl-total-records").text(response.RowCount);
                    if (render != undefined) {
                        $('#tbl-content').html(render);

                    }
                    wrapPaging(response.RowCount, function () {
                        loadData();
                    }, isPageChanged);


                }
                else {
                    $("#lbl-total-records").text('0');
                    $('#tbl-content').html('');
                }
                atom.stopLoading();
            },
            error: function (status) {
                console.log(status);
            }
        });
    };
    function getPaymentMethodName(paymentMethod) {
        var method = $.grep(cachedObj.paymentMethods, function (element, index) {
            return element.Value == paymentMethod;
        });
        if (method.length > 0)
            return method[0].Name;
        else return '';
    }
    function getOrderStatusName(status) {
        var status = $.grep(cachedObj.orderStatuses, function (element, index) {
            return element.Value == status;
        });
        if (status.length > 0)
            return status[0].Name;
        else return '';
    }
    function wrapPaging(recordCount, callBack, changePageSize) {
        var totalsize = Math.ceil(recordCount / atom.configs.pageSize);
        //Unbind pagination if it existed or click change pagesize
        if ($('#paginationUL a').length === 0 || changePageSize === true) {
            $('#paginationUL').empty();
            $('#paginationUL').removeData("twbs-pagination");
            $('#paginationUL').unbind("page");
        }
        //Bind Pagination Event
        $('#paginationUL').twbsPagination({
            totalPages: totalsize,
            visiblePages: 7,
            first: 'first',
            prev: 'prev',
            next: 'next',
            last: 'last',
            onPageClick: function (event, p) {
                atom.configs.pageIndex = p;
                setTimeout(callBack(), 200);
            }
        });
    }
}