var HomeController = function () {
    this.initialize = function () {
        loadData();
    }

    function loadData(from, to) {

        var today = new Date();
        var fromday = new Date(today.getTime() - (7 * 24 * 60 * 60 * 1000));

        var todd = today.getDate();
        var fromdd = fromday.getDate();
        var tomm = today.getMonth() + 1;
        var frommm = fromday.getMonth() + 1;
        var toyyyy = today.getFullYear();
        var fromyyyy = fromday.getFullYear();
        if (todd < 10) {
            todd = '0' + todd;
        }

        if (tomm < 10) {
            tomm = '0' + tomm;
        }
        today = tomm + '/' + todd + '/' + toyyyy;
        fromday = frommm + '/' + fromdd + '/' + fromyyyy;
        $.ajax({
            type: "GET",
            url: "/Admin/Home/GetRevenue",
            data: {
                fromDate: fromday,
                toDate: today
            },
            dataType: "json",
            beforeSend: function () {
                atom.startLoading();
            },
            success: function (response) {
                $('#totalUSer').text(response.TotalUser);
                $('#totalRevenue').text('$' + response.TotalRevenue);
                $('#totalProfit').text('$' + response.TotalProfit);
                $('#expenses').text('$' + response.Expense);
                $('#expenditure').text('$' + response.Expenditure);
                $('#sales').text(response.Sales);
                $('#reviews').text(response.Review);
                $('#visitors').text(response.Visittor);
                $('#demo-pie-1').attr('data-percent', response.SalePercent);
                $('#demo-pie-2').attr('data-percent', response.ReviewPercent);
                initChart(response.RevenueReports);
                atom.stopLoading();
            },
            error: function (status) {
                atom.notify('Has an error', 'error');
                atom.stopLoading();
            }
        });
    }


    function initChart(data) {
        var arrRevenue = [];
        var arrProfit = [];

        $.each(data, function (i, item) {
            var today = new Date(item.Date);

            var todd = today.getDate();
            var tomm = today.getMonth() + 1;
            var toyyyy = today.getFullYear();
            if (todd < 10) {
                todd = '0' + todd;
            }

            if (tomm < 10) {
                tomm = '0' + tomm;
            }
            today = tomm + '/' + todd + '/' + toyyyy;
            arrRevenue.push({
                X: today,
                Y: item.Revenue
            });
        });
        $.each(data, function (i, item) {
            var today = new Date(item.Date);

            var todd = today.getDate();
            var tomm = today.getMonth() + 1;
            var toyyyy = today.getFullYear();
            if (todd < 10) {
                todd = '0' + todd;
            }

            if (tomm < 10) {
                tomm = '0' + tomm;
            }
            today = tomm + '/' + todd + '/' + toyyyy;
            arrProfit.push({
                X: today,
                Y: item.Profit
            });
        });
        //[
        //    { X: "6:00", Y: 10.00 },
        //    { X: "7:00", Y: 20.00 },
        //    { X: "8:00", Y: 40.00 },
        //    { X: "9:00", Y: 34.00 },
        //    { X: "10:00", Y: 40.25 },
        //    { X: "11:00", Y: 28.56 },
        //    { X: "12:00", Y: 18.57 },
        //    { X: "13:00", Y: 34.00 },
        //    { X: "14:00", Y: 40.89 },
        //    { X: "15:00", Y: 12.57 },
        //    { X: "16:00", Y: 28.24 },
        //    { X: "17:00", Y: 18.00 }
        //]
        var graphdata1 = {
            linecolor: "#33E114",
            title: "Revenue",
            values: arrRevenue
        };
        var graphdata2 = {
            linecolor: "#F70404",
            title: "Profit",
            values: arrProfit
        };

        $("#Linegraph").SimpleChart({
            ChartType: "Line",
            toolwidth: "50",
            toolheight: "25",
            axiscolor: "#E6E6E6",
            textcolor: "#6E6E6E",
            showlegends: false,
            data: [graphdata1, graphdata2],
            legendsize: "140",
            legendposition: 'bottom',
            xaxislabel: 'DAY',
            title: 'Weekly Profit',
            yaxislabel: 'Profit in $'
        });

    }
}