import { ApiService, ApiType } from "/Scripts/Common/apiService.js";
import { DefaultService } from "/Scripts/Common/DefaultService.js";

export class Incident {//class that handles screen
    userToken = null;
    oneTimeToken = null;

    map = null;
    markerGroup = null;
    interval = null;
    test = [];
    testMap = null;
    constructor() {
        $(document).ready(() => {
           // this.intervalData();
            this.getMapData();
            
        });
    }

    intervalData() {
        this.interval =  setInterval("Incident.getMapData()", 15000);
    }


    getMapData() {
        if (this.map != null) {
            this.map.remove();
            this.map = null;
        }
        this.getServiceRequestMapData();

        ApiService.callCustom(ApiType.POST, variable.fleetParam, "", (datax) => {
            if (datax) {
                let LiveData = [];
                let ZoneData = [];
                this.getMap()
                $.ajax({
                    type: 'POST',
                    dataType: "JSON",
                    url: datax.data.fleetDataURL + "login",
                    data: JSON.parse(datax.data.fleetLoginPayload),
                    success: (data) => {
                        this.userToken = data.data.user.userToken;
                        this.oneTimeToken = data.oneTimeToken;
                        $.ajax({
                            type: 'POST',
                            dataType: "JSON",
                            url: datax.data.fleetDataURL + "Live",
                            data: JSON.parse(datax.data.fleetMapPayload),
                            headers: {
                                'One_time_token': this.oneTimeToken,
                                'User_token': this.userToken,
                            },
                            context: this,
                            success: (dataLive) => {
                                LiveData = dataLive.data.liveData;
                                this.getLiveData(LiveData)
                            }
                        });

                        $.ajax({
                            type: 'GET',
                            dataType: "JSON",
                            url: datax.data.fleetDataURL + "getzones",
                            headers: {
                                'One_time_token': this.oneTimeToken,
                                'User_token': this.userToken,
                            },
                            context: this,
                            success: (dataZone) => {
                                ZoneData = dataZone.data.lZone;
                                this.getZones(ZoneData)
                            }
                        });
                    }

                });

            }
        })
    }

    getMap() {

        var googleHybrid = L.tileLayer('http://{s}.google.com/vt?lyrs=s,h&x={x}&y={y}&z={z}', {
            maxZoom: 20,
            subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
        });

        var googleStreets = L.tileLayer('http://{s}.google.com/vt?lyrs=m&x={x}&y={y}&z={z}', {
            maxZoom: 20,
            subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
        });

        var openstreet = L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 20
        });


        this.map = L.map('map-long', {
            center: [-20.3484, 57.5522],
            zoom: 10,
            layers: [openstreet]
        });
        var baseMaps = {
            "Google Hybrid": googleHybrid,
            "Google Streets": googleStreets,
            "Open Street Map": openstreet
        };

        var layerControl = L.control.layers(baseMaps).addTo(this.map);

        if (this.map != undefined) {

            this.map.invalidateSize();
        }
    }

    getLiveData(data) {
        this.markerGroup = L.featureGroup().addTo(this.map);
        for (var datum of data) {
            var iconUrl = '../assets/Modules/Incident/MapIcon/marker-red.png';
            if (datum.activity == "Driving" || datum.activity == "Idling") {
                iconUrl = '../assets/Modules/Incident/MapIcon/marker-green.png';
            }

            var lastFuel = "No Fuel Data";
            if (datum.lastFueldata != null) {
                lastFuel = datum.lastFueldata.convertedValue + " " + datum.lastFueldata.uom;
            }
            var marker = L.marker([datum.latitude, datum.longitude], {
                icon: new L.icon({
                    iconUrl: iconUrl,

                    iconSize: [38, 38], // size of the icon
                    shadowSize: [50, 64], // size of the shadow
                    iconAnchor: [22, 37], // point of the icon which will correspond to marker's location
                    shadowAnchor: [4, 62],  // the same for the shadow
                    popupAnchor: [-3, -16] // point from which the popup should open relative to the iconAnchor
                })
            }).bindTooltip("<b>" + datum.vehicle.description + "</b><br><b> Status: </b>" + datum.activity  + "</b><br><b> Speed: </b>" + datum.speed + " km/h" + "<br><b> Address: </b>" + datum.address + "<br><b> Fuel: </b>" + lastFuel, { permanent: false, direction: 'top' })
            this.markerGroup.addLayer(marker);
        }
    }

    getZones(data) {
        data.forEach(z => {
            var geomData = this.fromGeoJSON(z.GeomData);
            var geojsonFeature = {
                "type": "Feature",
                "geometry": {
                    "type": geomData.jsonData.type,
                    "coordinates": [geomData.jsonData.coordinates]

                },
                "properties": {
                    "popupContent": z.description
                },
            };
            var zoneStyle = {
                "color": '#f96736',
                "opacity": 0.8
            };
            var jsonEditLayer = L.geoJSON(geojsonFeature, { style: zoneStyle, pmIgnore: false, onEachFeature: this.onEachFeature }).addTo(this.map);

        });
    }

    getServiceRequestMapData(type = "") {
        var url = null;
        if (type == "") {
            url = variable.incidentList;
        } else {
            url = variable.getIncidentByTypeName;
        }
        ApiService.callCustom(ApiType.POST, url, type, (data) => {
            if (data.data) {
                $("#list-group").html("");
                let container = $("#list-group");

                this.test.forEach(test => this.testMap.removeLayer(test))
                if (data.data.length > 0) {
                    this.testMap = L.featureGroup().addTo(this.map);
                    for (let datum of data.data) {
                        var classColor = "";
                        var icon = "";
                        var markericon = ""
                        if (datum.type == "Non Fire Incident") {
                            classColor = "list-group-item-warning";
                            icon = "fa-solid fa-car-burst";
                            markericon = "../assets/Modules/Incident/gif/Accident.gif";
                        } else if (datum.type == "Fire Incident") {
                            classColor = "list-group-item-danger";
                            icon = "fa-solid fa-fire";
                            markericon = "../assets/Modules/Incident/gif/Fire.gif";
                        } else if (datum.type == "Non Incident") {
                            classColor = "list-group-item-secondary";
                            icon = "fa-solid fa-kit-medical";
                            markericon = "../assets/Modules/Incident/gif/Flooding.gif";
                        } else {
                            markericon = "../assets/Modules/Incident/gif/Rescue.gif";
                        }

                        let con =
                            DefaultService.createElement("a", { class: "list-group-item list-group-item-action " + classColor, onclick: "Incident.showIncident('" + datum.coordinates + "')" }, [
                                DefaultService.createElement("div", { class: "d-flex w-100 justify-content-between" }, [
                                    DefaultService.createElement("span", { class: "mb-1 f-weight-bold span-medium" }, [
                                        DefaultService.createElement("i", { class: icon + " me-2" }, [], {}),
                                    ], { innerHTML: datum.incidentNo }),
                                    DefaultService.createElement("small", {}, [], { innerHTML: datum.timelapse })
                                ]),
                                DefaultService.createElement("div", {}, [
                                    DefaultService.createElement("span", { class: "mb-1 f-weight-bold" }, [], { innerHTML: "Type : " }),
                                    DefaultService.createElement("span", { class: "mb-1" }, [], { innerHTML: datum.type }),
                                ]),
                                DefaultService.createElement("div", {}, [
                                    DefaultService.createElement("span", { class: "mb-1 f-weight-bold" }, [], { innerHTML: "SubType : " }),
                                    DefaultService.createElement("span", { class: "mb-1" }, [], { innerHTML: datum.subType }),
                                ]),
                                DefaultService.createElement("div", {}, [
                                    DefaultService.createElement("span", { class: "mb-1 f-weight-bold" }, [], { innerHTML: "Incident Code : " }),
                                    DefaultService.createElement("span", { class: "mb-1" }, [], { innerHTML: datum.incidentCode }),
                                ]),
                                DefaultService.createElement("div", {}, [
                                    DefaultService.createElement("span", { class: "mb-1 f-weight-bold" }, [], { innerHTML: "Nature of Incident : " }),
                                    DefaultService.createElement("span", { class: "mb-1" }, [], { innerHTML: datum.natureOfIncident }),
                                ]),
                                DefaultService.createElement("div", {}, [
                                    DefaultService.createElement("span", { class: "mb-1 f-weight-bold" }, [], { innerHTML: "Address : " }),
                                    DefaultService.createElement("span", { class: "mb-1" }, [], { innerHTML: datum.location ?? "No Location" }),
                                ]),
                                DefaultService.createElement("div", { class: "d-flex justify-content-between align-items-center" }, [
                                    DefaultService.createElement("div", {}, [
                                        DefaultService.createElement("span", { class: "mb-1 f-weight-bold" }, [], { innerHTML: "Station : " }),
                                        DefaultService.createElement("span", { class: "mb-1" }, [], { innerHTML: datum.station }),
                                    ]),
                                    DefaultService.createElement("a", { class: "link-primary", onclick: "Incident.view(" + datum.id + ")" }, [
                                        DefaultService.createElement("span", {}, [], { innerHTML: "View " }),
                                        DefaultService.createElement("i", { class: "fa fa-arrow-right" }, [], {})
                                    ])
                                ])
                            ]);
                        // Attach event listener to the "View" link
                        con.querySelector('.link-primary').addEventListener('click', function (event) {
                            event.preventDefault(); // Prevent default action of the link
                            event.stopPropagation(); // Stop event propagation to parent elements
                        });
                        container.append(con)
                        if (datum.coordinates != ",") {
                            var latlng = datum.coordinates.replace(" ", "").split(",");
                            var marker = L.marker([latlng[0], latlng[1]], {
                                icon: new L.icon({
                                    iconUrl: markericon,

                                    iconSize: [34, 34], // size of the icon
                                    shadowSize: [50, 64], // size of the shadow
                                    iconAnchor: [22, 13], // point of the icon which will correspond to marker's location
                                    shadowAnchor: [4, 62],  // the same for the shadow
                                    popupAnchor: [-3, -76] // point from which the popup should open relative to the iconAnchor
                                })
                            }).bindTooltip("<b>" + datum.type + "</b><br> " + datum.location, { permanent: false, direction: 'top' })
                            this.test.push(marker);
                            this.testMap.addLayer(marker);
                        }
                    }
                } else {
                    let con = DefaultService.createElement("span", { class: "mb-1 f-weight-bold" }, [], { innerHTML: "No Call Ins Recorded" })
                    container.append(con)
                    this.test.forEach(test => this.testMap.removeLayer(test))
                    
                }
                
               
            }
        })


    }
    view(id) {
        window.location.href = "/IncidentHandling/AddEdit?Id=" + id + "&View=1";
    }
    showIncident(coords) {
        var googleHybrid = L.tileLayer('http://{s}.google.com/vt?lyrs=s,h&x={x}&y={y}&z={z}', {
            maxZoom: 20,
            subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
        });
        var latlng = coords.replace(" ", "").split(",");
        if (coords != ",") {
            this.map.flyTo([latlng[0], latlng[1]], 18);

        }
    }

    maponEachFeature() {

    }

    onEachFeature(feature, layer) {
        // does this feature have a property named popupContent?
        if (feature.properties && feature.properties.popupContent) {
            layer.bindPopup(feature.properties.popupContent);
        }
    }

    fromGeoJSON(input) {
        //console.log("fromGeoJSON", input);
        var layer;
        var layerType;
        var jsonData;
        var geometryCollectionArray = [];

        if (input.includes("GEOMETRYCOLLECTION")) {
            input = input.replace("GEOMETRYCOLLECTION(", "").replace("");
            geometryCollectionArray = input.split(")),");
            for (var i = 0; i < geometryCollectionArray.length; i++) {
                if (i < geometryCollectionArray.length - 1) {
                    geometryCollectionArray[i] += "))";
                }
            }
        } else {
            geometryCollectionArray.push(input);
        }

        for (var i = 0; i < geometryCollectionArray.length; i++) {

            input = geometryCollectionArray[i];
            input = input.replace("undefined", "");
            if (input.includes("POINT")) {
                input = input.replace("POINT (", "").replace(")", "").replace("POINT(", "");
                let points = input.split(" "); //console.log(points);
                layer = L.marker([+points[0], +points[1]]);
                layerType = "marker";
                jsonData = { type: "Point", coordinates: [+points[1], +points[0]] };
            } else if (input.includes("MULTIPOLYGON")) {
                input = input.replace("MULTIPOLYGON (((", "").replace(")))", "").replace("MULTIPOLYGON(((", "");
                let points = [];
                if (input.includes(", ")) {
                    points = input.split(", ");
                } else {
                    points = input.split(",");
                }
                var values = [];
                points.forEach(e => {
                    e = e.replace("((", "").replace("))", "");
                    var innerPoints = e.split(" ");
                    values.push([+innerPoints[0], +innerPoints[1]]);
                });
                //console.log("MULTIPOLYGON VALUES ", values, "");
                layer = L.polygon(values);
                layerType = "Polygon";
                jsonData = { type: "Polygon", coordinates: values };
            } else if (input.includes("POLYGON")) {
                //console.log("INPUT INCLUES POYLYGON", input);
                input = input.replace("POLYGON ((", "").replace("))", "").replace("POLYGON((", "");
                var points = [];
                if (input.includes(", ")) {
                    points = input.split(", ");
                } else {
                    points = input.split(",");
                }
                var values = [];
                points.forEach(e => {
                    var innerPoints = e.split(" ");
                    values.push([+innerPoints[0], +innerPoints[1]]);
                });
                //console.log("POLYGON VALUES ", values, input);
                layer = L.polygon(values);
                layerType = "polygon";
                jsonData = { type: "Polygon", coordinates: values };
            } else if (input.includes("MULTILINESTRING")) {
                //input = input.replace("MULTILINESTRING((", "").replace("))", "").replace("MULTILINESTRING ((", "");
                //var points: string[] = [];
                //if (input.includes(" ")) {
                //    points = input.split(", ");
                //} else {
                //    points = input.split(",");
                //}
                //var values = [];
                //points.forEach(e => {
                //    var innerPoints = e.split(" ");
                //    console.log("innerPoints", innerPoints);
                //    values.push([+innerPoints[1], +innerPoints[0]]);
                //});
                //console.log("MULTILINESTRING VALUES", values);

                //layer = L.polyline(values);
                //layerType = "polyline";
                //jsonData = { type: "PolyLine", coordinates: values };
                break;
            } else if (input.includes("LINESTRING")) {
                input = input.replace("LINESTRING(", "").replace(")", "").replace("LINESTRING (", "");
                //console.log(input);
                var points = [];
                if (input.includes(" ")) {
                    points = input.split(", ");
                } else {
                    points = input.split(",");
                }
                var values = [];
                points.forEach(e => {
                    var innerPoints = e.split(" ");
                    values.push([+innerPoints[0], +innerPoints[1]]);
                });
                //console.log("LINESTRING VALUES ", values, "");
                layer = L.polyline(values);
                layerType = "polyline";
                jsonData = { type: "PolyLine", coordinates: values };
            }
        }


        return { layer: layer, layerType: layerType, jsonData: jsonData };
    }


    //Enlarge / Minimize map
    enlargeManimizeMap() {
        var buttonCustom = document.getElementById("enlarge-minimize");
        var isMinimized = buttonCustom.innerHTML.includes('Enlarge')
        if (isMinimized) {
            buttonCustom.innerHTML = "Minimize Map";
            var div = document.getElementById("map-graph");
            var map = document.getElementById("Plan-vs-NoPlan-Amount");
            map.style.height = '80vh';
        } else {
            buttonCustom.innerHTML = "Enlarge Map";
            var popup = document.getElementById("map-graph");
            var map = document.getElementById("Plan-vs-NoPlan-Amount");
            map.style.height = '40vh';
        }
        this.scrollToBottom();
        this.map.invalidateSize();
    }

    minimizeMap() {
        var buttonCustom = document.getElementById("enlarge-minimize");
        if (buttonCustom != undefined) {
            buttonCustom.innerHTML = "Enlarge Map";
        }
        var div = document.getElementById("map-graph");
        d
        var map = document.getElementById("Plan-vs-NoPlan-Amount");
        map.style.height = '40vh';
        this.scrollToBottom();
        this.map.invalidateSize();
    }

    zoomToMarker() {
        if (this.markerGroup != null && this.map != undefined) {
            this.map.fitBounds(this.markerGroup.getBounds());
        }
    }

    scrollToBottom() {
        const element = document.getElementById("main-main");
        element.scrollIntoView({ behavior: "smooth", block: "end" });
    }

    fetchData() {
        toastr.info("Fetching Data ...")
        this.getMapData();
    }
    filterListbyType(type) {
        ApiService.callCustom(ApiType.POST, variable.getListAsset, type, (data) => {
            if (data.data) {
                DatatableService.updateDataGrid('AssetTable', completedList)
            }
        })
    }

}
window.Incident = new Incident();