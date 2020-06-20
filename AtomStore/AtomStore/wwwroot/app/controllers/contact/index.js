var FeedbackController = function () {
    this.initialize = function () {
        loadData();
        registerEvents();
    }
    function registerEvents() {
        $('#txt-search-keyword').keypress(function (e) {
            if (e.which === 13) {
                e.preventDefault();
                loadData();
            }
        });
        $("#btn-search").on('click', function () {
            loadData();
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
                type: "POST",
                url: "/Admin/Contact/UpdateStatus",
                data: { id: that },
                beforeSend: function () {
                    atom.startLoading();
                },
                success: function (response) {
                    loadData();
                    atom.stopLoading();

                },
                error: function (e) {
                    atom.notify('Has an error in progress', 'error');
                    atom.stopLoading();
                }
            });
        });
    };
    function loadData(isPageChanged) {
        $.ajax({
            type: "GET",
            url: "/admin/Contact/GetAllPaging",
            data: {
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
                            Id: item.Id,
                            Name: item.Name,
                            Email: item.Email,
                            Message: item.Message,
                            DateCreated: atom.dateTimeFormatJson(item.DateCreated),
                            Status: item.Status == 1 ? 'Replied' : 'Not reply',
                            Class: item.Status == 1 ? 'fa-check' : 'fa-remove',
                            Color: item.Status == 1 ? '#28a745' :'#dc3545'
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