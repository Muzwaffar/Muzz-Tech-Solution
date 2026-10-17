export class DateRangeService {
    static dateRangePicker(Id) {
        var currentDate = moment().startOf('day');
        var firstDay = moment(currentDate).subtract(6, 'days');
        var lastDay = moment(currentDate);

        $('#DateRangePicker').daterangepicker({         
            "autoApply": true,
            ranges: {
                'Today': [moment(), moment()],
                'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                'This Month': [moment().startOf('month'), moment().endOf('month')],
                'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
            },
            "startDate": firstDay,
            "endDate": lastDay
        }, function (start, end, label) {}
        );
    }

    static formatReturnDate() {
        //get datepicker value
        var dateRange = $('#DateRangePicker').val();
        var dateRangeT = dateRange.split(" ");
        //format Date
        var From = DateRangeService.formatDate(dateRangeT[0]);
        var To = DateRangeService.formatDate(dateRangeT[2]);
        //dto to send date from and To
        var range = {
            From: moment(From, "DD-MM-YYYY").add(4, 'hours'),
            To: moment(To, "DD-MM-YYYY").add(4, 'hours')
        };

        return range;
    }

    static formatDefaultReturnDate() {
        //dto to send date from and To
        var range = {
            From: moment(moment(), "DD-MM-YYYY").add(4, 'hours'),
            To: moment(moment().subtract(6, 'days'), "DD-MM-YYYY").add(4, 'hours')
        };

        return range;
    }
    

    static formatDate(dat) {
        const [month, day, year] = dat.split('/');
        const date = new Date(+year, +month - 1, +day); 8
        return date;
    }
}