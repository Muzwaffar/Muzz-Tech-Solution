import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { Variable } from "/Scripts/Common/variable.js";

export class ScheduleCalendar {//class that handles screen
    event = []
    constructor() {
        this.renderCalendar();
    }

    //Calendar Rendering
    renderCalendar() {
        ApiService.callCustom(ApiType.POST, new Variable().getCalendarAPI, "", (data) => {
            if (data) {
                if (data.data.length > 0) {
                    for (let [index, datum] of data.data.entries()) {
                        datum.url = '#partialSchedule'
                        datum.className = 'calendarEvent' + index;
                        $('<style>.calendarEvent' + index + ' { background-color: ' + datum.backgroundcolor + '; border: 0px; }</style>').appendTo('body');
                        $('<style>.calendarEvent' + index + ':hover { text-decoration: none; color: black; }</style>').appendTo('body');
                        this.event.push(datum);
                    }

                    var calendarEl = document.getElementById('calendar');
                    var calendar = new FullCalendar.Calendar(calendarEl, {
                        initialView: 'dayGridMonth',
                        initialDate: Date.now(),
                        headerToolbar: {
                            left: 'prev,next today',
                            center: 'title',
                            right: 'dayGridMonth,timeGridWeek,timeGridDay'
                        },


                        events: this.event,
                        dayMaxEvents: 4,
                        eventClick: function (info) {
                            $(".content").html("")
                            if (info.event.id == 0) { info.event.id = "" };
                            ApiService.callCustom(ApiType.POST, new Variable().getCalendarById, info.event.id, (data) => {
                                if (data) {
                                    //set partial view content
                                    $(".content").html(data)
                                    ScheduleCalendarPopup.getRelationById(info.event.id)
                                }
                            })

                        }
                    });
                    calendar.render();

                }
                else {
                    var calendarEl = document.getElementById('calendar');
                    var calendar = new FullCalendar.Calendar(calendarEl, {
                        initialView: 'dayGridMonth',
                        initialDate: Date.now(),
                        headerToolbar: {
                            left: 'prev,next today',
                            center: 'title',
                            right: 'dayGridMonth,timeGridWeek,timeGridDay'
                        },

                    });
                    calendar.render();
                }

            }
        })

        
    }

}
window.ScheduleCalendar = new ScheduleCalendar();