import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Variable } from "/Scripts/Common/variable.js";
import { DatatableService } from "/Scripts/Common/DatatableService.js";
import { ActivityModule } from "/Scripts/Common/Enums.js";

export class Activities {//class that handles screen
    isFirstLoading = true;
    root = null;
    root2 = null;
    constructor() {

        $(document).ready(() => {
            //Column list   to build table (predefined columns)
            //data: object key
            //title: column title to show

            var ColumnList = [

                { data: 'activityTypeName', title: 'Activity Type' },
                { data: 'startDate', title: 'Start Date' },
                { data: 'endDate', title: 'End Date' },
                { data: 'status', title: 'Status', },
                { data: 'remarks', title: 'Remarks' },
                { data: 'assignedTo', title: 'Assigned To' },
                { data: 'createdBy', title: 'Created By' }
            ];

            //(Table ID, Partial View ID, Button to open popup , url api to get list, column list (From above), class name where action function is defined)
            DatatableService.initializeDataGridActivities('ActivitiesTable', 'partial', 'Activities.openPopupAddEdit(0)', ColumnList, 'Activities');

            this.filterDatatable();
        });
    }
    
    //filterDatatable Function
    filterDatatable() {
        if (!this.isFirstLoading) {
            //get datepicker value
            var dateRange = $('#DateRangePicker').val();
            var dateRangeT = dateRange.split(" ");
            //format date
            var From = this.formatDate(dateRangeT[0]);
            var To = this.formatDate(dateRangeT[2]);
            //dto to send data from and to
            var range = {
                From: moment(From, "DD-MM-YYYY").add(4, 'hours'),
                To: moment(To, "DD-MM-YYYY").add(4, 'hours'),
                ActivityModule: ActivityModule.General
            };
            //Calling API with Range
            ApiService.callCustom(ApiType.POST, variable.getAllActivies, range, (data) => {
                if (data) {
                    DatatableService.updateDataGrid('ActivitiesTable', data.data);
                }
            });

            //Calling API with Range
            ApiService.callCustom(ApiType.POST, variable.activitiesData, range, (data) => {
                if (data) {
                    this.clearChart();
                    //update datagrid
                    this.getChartData(data.data);
                    this.getPieChart(data.data);
                    setTimeout(e => {
                        this.getURLparameters();
                    }, 100)


                }
            });
        } else {
            var firstDay = moment(new Date()).subtract('7', 'days').format("MM/DD/yyyy");
            var lastDay = moment(new Date()).format("MM/DD/yyyy");
            var date = firstDay + " - " + lastDay;
            $('#DateRangePicker').val(date)
            this.isFirstLoading = false;
        }

    }

    //format date
    formatDate(dat) {
        const [month, day, year] = dat.split('/');
        const date = new Date(+year, +month - 1, +day);
        return date;
    }

    //funtion to check whether this page is being redirected
    getURLparameters() {
        const queryString = window.location.search;
        //url Parameters
        const urlParams = new URLSearchParams(queryString);
        // if url contains dto, switch to edit mode
        if (urlParams.has('dto')) {
            //get Id from URL
            this.Id = parseInt(urlParams.get('dto'))
            window.location = window.location + "partial"
            //Get inherited data when loading the page
            this.openPopupAddEdit(this.Id);
        };
    }

    //open view
    openPopupAddEdit(Id) {
        //empty partial view content 
        $(".content").html("")
        //call api to get partial view
        if (Id == 0) { Id = "" };
        ApiService.callCustom(ApiType.POST, new Variable().getByIdActivities, Id, (data) => {
            if (data) {
                //set partial view content
                $(".content").html(data)
            }
        })
    }

    //Clear chart Data upon selection of dates
    clearChart() {
        if (am5.registry.rootElements.length > 0) {
            am5.registry.rootElements[0].dispose()
        }
        am5.registry.rootElements.forEach(root => {
            root.container.children.clear();
            root.dispose()
        })
    }

    //render pie chart data
    getPieChart(dat) {
        if (dat.pieData.length > 0) {
            // Create root element
            var root = am5.Root.new("activity-chart-by-status");
            // Set themes
            // https://www.amcharts.com/docs/v5/concepts/themes/
            root.setThemes([
                am5themes_Animated.new(root)
            ]);

            // Create chart
            // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/
            var chart = root.container.children.push(
                am5percent.PieChart.new(root, {
                    endAngle: 270
                })
            );

            // Create series
            // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Series
            var series = chart.series.push(
                am5percent.PieSeries.new(root, {
                    valueField: "value",
                    categoryField: "key",
                    endAngle: 270
                })
            );

            series.states.create("hidden", {
                endAngle: -90
            });

            // Set data
            // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Setting_data
            var data = dat.pieData;

            series.data.setAll(data);

            series.appear(1000, 100);
        }
    }

    //render bar chart data
    getChartData(dat) {
        if (dat.byItems.length > 0) {
            // Create root element
            var root = am5.Root.new("activity-chart-by-type");

            // Set themes
            root.setThemes([
                am5themes_Animated.new(root)
            ]);

            var chart = root.container.children.push(am5xy.XYChart.new(root, {
                panX: true,
                panY: true,
                wheelX: "panX",
                wheelY: "zoomX",
                pinchZoomX: true
            }));

            // Add cursor
            // https://www.amcharts.com/docs/v5/charts/xy-chart/cursor/
            var cursor = chart.set("cursor", am5xy.XYCursor.new(root, {}));
            cursor.lineY.set("visible", false);

            // Create axes
            // https://www.amcharts.com/docs/v5/charts/xy-chart/axes/
            var xRenderer = am5xy.AxisRendererX.new(root, {
                minGridDistance: 30
            });

            var xAxis = chart.xAxes.push(am5xy.CategoryAxis.new(root, {
                maxDeviation: 0.3,
                categoryField: "key",
                renderer: xRenderer,
                tooltip: am5.Tooltip.new(root, {})
            }));

            // Enable label wrapping
            xRenderer.labels.template.setAll({
                oversizedBehavior: "wrap",
                textAlign: "center"
            });

            // Set up automatic width calculation using an adapter
            xRenderer.labels.template.adapters.add("width", function (width, target) {
                var x0 = xAxis.getDataItemCoordinateY(xAxis.dataItems[0], "category", 0);
                var x1 = xAxis.getDataItemCoordinateY(xAxis.dataItems[0], "category", 1);
                target.set("maxWidth", x1 - x0)
                return x1 - x0;
            });

            var yAxis = chart.yAxes.push(am5xy.ValueAxis.new(root, {
                maxDeviation: 0.3,
                renderer: am5xy.AxisRendererY.new(root, {})
            }));


            // Create series
            // https://www.amcharts.com/docs/v5/charts/xy-chart/series/
            var series = chart.series.push(am5xy.ColumnSeries.new(root, {
                name: "Series 1",
                xAxis: xAxis,
                yAxis: yAxis,
                valueYField: "value",
                sequencedInterpolation: true,
                categoryXField: "key",
                tooltip: am5.Tooltip.new(root, {
                    labelText: "{valueY}"
                })
            }));


            // Set data
            var data = dat.byItems

            xAxis.data.setAll(data);
            series.data.setAll(data);

            xAxis.get("renderer").labels.template.setAll({
                tooltipText: "a {keyFull}",
                interactive: true
            })


            // Make stuff animate on load
            // https://www.amcharts.com/docs/v5/concepts/animations/
            series.appear(1000);
            chart.appear(1000, 100);


        }
    }

}
window.Activities = new Activities();