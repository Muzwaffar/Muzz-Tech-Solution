import { Variable } from "/Scripts/Common/variable.js";
export class MapService {//handles all leaflet map and leaflet draw
    //Map With Draw Control
    static initMap() {
        var locationType = null;
        var featureLayer = null;
        let map;
        
        if (map != undefined) { map.remove(); } 
        if ($("#map").length > 0) {
           
            map = L.map('map', { drawControl: false });
            
            var osm = L.tileLayer('http://{s}.google.com/vt/lyrs=s,h&x={x}&y={y}&z={z}', {
                maxZoom: 20,
                subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
            })
            map.addLayer(osm);
            setInterval(function () {
                map.invalidateSize();
            }, 500);
            var geometryToShow = $("#GeomDataText").val();
            if (geometryToShow != "") {
                let parsedJson = JSON.parse(geometryToShow);
                featureLayer = L.geoJson(parsedJson);
                featureLayer.addTo(map);

                map.fitBounds(featureLayer.getBounds());
            } else {
                featureLayer = new L.FeatureGroup();
                map.locate({ setView: true, maxZoom: 16 })
            }

            var drawPluginOptions = {
                position: 'topright',
                draw: {
                    polyline: false,
                    polygon: {
                        allowIntersection: false, // Restricts shapes to simple polygons
                        drawError: {
                            color: '#e1e100', // Color the shape will turn when intersects
                            message: '<strong>Polygon draw does not allow intersections.<strong>' // Message that will show when intersect
                        },
                        shapeOptions: {
                            color: '#bada55'
                        }
                    },
                    circle: false,
                    circlemarker: false,
                    rectangle: {
                        shapeOptions: {
                            clickable: false
                        }
                    }
                },
                edit: {
                    featureGroup: featureLayer, //REQUIRED!!
                    remove: true,
                    edit: true
                }
            };
            // Initialise the draw control and pass it the FeatureGroup of editable layers
            var drawControl = new L.Control.Draw(drawPluginOptions);
            map.addControl(drawControl);

         
            map.on('draw:created', function (e) {
                var type = e.layerType, layer = e.layer;
                locationType = type;
                if (type != "marker") {
                    var latlon = layer.getBounds().getCenter();
                    $("#CenterPointLat").val(latlon.lat);
                    $("#CenterPointLong").val(latlon.lng);
                } else {
                    $("#CenterPointLat").val(layer.getLatLng().lat);
                    $("#CenterPointLong").val(layer.getLatLng().lng);
                }
                if (type == "rectangle" || type == "polygon") {
                    locationType = "Polygon";
                } else if (type == "marker") {
                    locationType = "Point"
                } else if (type == "circlemarker") {
                    locationType = "Circle"
                }
                $("#Type").val(locationType);
                var geojson = layer.toGeoJSON()
                $("#GeomDataText").val(JSON.stringify(geojson));
                featureLayer = layer;
                featureLayer.addTo(map);
            });


            map.on('draw:drawstart', function (e) {
                map.removeLayer(featureLayer);
                $("#GeomDataText").val("");
            })

            map.on('draw:edited', function (e) {
                var layers = e.layers;
                layers.eachLayer(function (layer) {
                    var type = layer.feature.geometry.type;
                    locationType = type;
                    if (type != "Point") {
                        var latlon = layer.getBounds().getCenter();
                        $("#CenterPointLat").val(latlon.lat);
                        $("#CenterPointLong").val(latlon.lng);
                    } else {
                        $("#CenterPointLat").val(layer.getLatLng().lat);
                        $("#CenterPointLong").val(layer.getLatLng().lng)
                    }
                    $("#Type").val(locationType);
                    var geojson = layer.toGeoJSON()
                    $("#GeomDataText").val(JSON.stringify(geojson));
                    featureLayer = layer;
                    featureLayer.addTo(map);
                });
            });
        }
    }

    //Map With Pin Control
    static initPinMap() {
        var locationType = null;
        var featureLayer = null;
        let map;

        if (map != undefined) { map.remove(); }
        if ($("#map").length > 0) {
            $(document).on('click', function (e) {
                // Check if the clicked element is outside the autocomplete input
                if (!$(e.target).closest('#Address').length) {
                    enableScrolling(); // Enable scrolling if clicked outside
                }
            });

            // Map tile layers 
            const googleHybrid = L.tileLayer('http://{s}.google.com/vt?lyrs=s,h&x={x}&y={y}&z={z}', { maxZoom: 20, subdomains: ['mt0', 'mt1', 'mt2', 'mt3'] });
            const googleStreets = L.tileLayer('http://{s}.google.com/vt?lyrs=m&x={x}&y={y}&z={z}', { maxZoom: 20, subdomains: ['mt0', 'mt1', 'mt2', 'mt3'] });
            // init map 
            map = L.map('map', {
                center: [-20.3484, 57.5522],
                zoom: 9,
                gestureHandling: true,
                layers: [googleHybrid]
            });
            //init base maps
            const baseMaps = {
                "Google Hybrid": googleHybrid,
                "Google Streets": googleStreets
            };
            //add control layer for base maps
            L.control.layers(baseMaps).addTo(map);

            //parse geometry and add to map if exist
            const geometryToShow = $("#GeomDataText").val();
            if (geometryToShow) {
                const parsedJson = JSON.parse(geometryToShow);
                featureLayer = L.geoJson(parsedJson).addTo(map);
                map.fitBounds(featureLayer.getBounds());
            } else {
                featureLayer = new L.FeatureGroup();
            }

            $('#Address').autocomplete({
                minChars: 3,
                deferRequestBy: 250,
                serviceUrl: new Variable().LocationIQURL + 'autocomplete',
                paramName: 'q',
                params: {
                    key: new Variable().LocationIQAccessToken,
                    format: "json",
                    limit: 5,
                    countrycodes: "mu"
                },
                ajaxSettings: {
                    dataType: 'json'
                },
                formatResult: function (suggestion, currentValue, index) {
                    disableScrolling();
                    return `
                        <div class="autocomplete-suggestion-name">${highlight(suggestion.data.display_place, currentValue)}</div>
                        <div class="autocomplete-suggestion-address">${highlight(suggestion.data.display_address, currentValue)}</div>
                    `;
                },
                transformResult: function (response) {
                    return {
                        suggestions: response.map(dataItem => ({
                            value: dataItem.display_name,
                            data: dataItem
                        }))
                    };
                },
                onSelect: function (suggestion) {
                    enableScrolling();
                    displaySelectedAddress(suggestion.data);
                }
            });
            
            // Suppress autocomplete dropdown when the marker is manually placed
            $('#Address').on('focus', function () {
                if ($(this).data('autocomplete-suppress')) {
                    $(this).data('autocomplete-suppress', false); // Reset suppress flag
                    return false; // Prevent the dropdown
                }
            });

            function disableScrolling() {
                // Disable mouse wheel scrolling
                document.addEventListener("wheel", preventDefaultWheel, { passive: false });

                // Disable keyboard scrolling
                document.addEventListener("keydown", preventDefaultKeydown);
            }

            function preventDefaultWheel(e) {
                e.preventDefault(); // Prevent mouse wheel scrolling
            }

            function preventDefaultKeydown(e) {
                const keys = [ 33, 34, 35, 36, 37, 38, 39, 40]; // Spacebar and arrow keys
                if (keys.includes(e.keyCode)) {
                    e.preventDefault(); // Prevent keyboard scrolling
                }
            }

            function enableScrolling() {
                // Re-enable mouse wheel scrolling (remove the listener)
                document.removeEventListener("wheel", preventDefaultWheel, { passive: false });

                // Re-enable keyboard scrolling (remove the listener)
                document.removeEventListener("keydown", preventDefaultKeydown);
            }

            // Function to display selected address details
            function displaySelectedAddress(data) {
                $("#Address").val(data.display_name)
                $("#Latitude").val(data.lat);
                $("#Longitude").val(data.lon);

                let {
                    name = "",
                    house_number = "",
                    road = "",
                    neighbourhood = "",
                    suburb = "",
                    island = "",
                    city = "",
                    county = "",
                    state = "",
                    state_code = "",
                    postcode = "",
                    country = "",
                    country_code = "",
                    quarter = "",
                    town = "",
                    village = ""
                } = data.address || {}; // Use `|| {}` in case `data.address` itself is undefined


                var street = [road, quarter, name].filter(Boolean).join(", ");
                var locality = [suburb, county, city, town, village].filter(Boolean).join(", ");
                var district = state;



                map.removeLayer(featureLayer);
                $("#GeomDataText").val("");
                $("#Type").val("Point");

                // Create GeoJSON for the selected point
                var geojson = {
                    "type": "Feature",
                    "geometry": {
                        "type": "Point",
                        "coordinates": [data.lon, data.lat]
                    },
                    "properties": {
                        "name": data.display_name,
                        "address": data.address,
                        "formatted_address": `${street}, ${locality}, ${district}, ${country}`
                    }
                };

                $("#Street").val(street)
                $("#Locality").val(locality)
                $("#District").val(district)
                // Store GeoJSON data in the GeomDataText field
                $("#GeomDataText").val(JSON.stringify(geojson)); // Set the GeoJSON as a string in the hidden field

                // Optionally, add the GeoJSON feature to the map (you can replace `layer` if needed)
                featureLayer = L.geoJSON(geojson); // Create a Leaflet GeoJSON layer from the GeoJSON data
                featureLayer.addTo(map); // Add the new GeoJSON layer to the map
                map.setView([data.lat, data.lon], 15); 

                $("#Address").val(data.display_name).change();
            }

            // Helper function to highlight matching text
            function highlight(text, focus) {
                var regex = new RegExp('(' + escapeRegExp(focus) + ')', 'gi');
                return text.replace(regex, '<strong>$1</strong>');
            }

            // Helper function to escape special characters in regex
            function escapeRegExp(str) {
                return str.replace(/[-[\]/{}()*+?.\\^$|]/g, '\\$&');
            }
            

            setInterval(e=> {
                map.invalidateSize();
            }, 500);
            var drawPluginOptions = {
                position: 'topright',
                draw: {
                    polyline: false,
                    polygon: false,
                    circle: false,
                    circlemarker: false,
                    rectangle: false
                },
                edit: {
                    featureGroup: featureLayer, //REQUIRED!!
                    remove: false,
                    edit: false
                }
            };
            // Initialise the draw control and pass it the FeatureGroup of editable layers
            var drawControl = new L.Control.Draw(drawPluginOptions);
            map.addControl(drawControl);


            map.on('draw:created', function (e) {

                var type = e.layerType, layer = e.layer;
                locationType = type;
                var lat = layer.getLatLng().lat;
                var lon = layer.getLatLng().lng
                $("#Latitude").val(lat);
                $("#Longitude").val(lon);
                if (type == "marker") {
                    locationType = "Point"
                }
                $("#Type").val(locationType);
                var geojson = layer.toGeoJSON()
                $("#GeomDataText").val(JSON.stringify(geojson));
                featureLayer = layer;
                featureLayer.addTo(map);

                $.get(new Variable().LocationIQURL + new Variable().LReverseGeooding + '&lat=' + lat + '&lon=' + lon + '&format=json', function (data) {
                    let {
                        name = "",
                        house_number = "",
                        road = "",
                        neighbourhood = "",
                        suburb = "",
                        island = "",
                        city = "",
                        county = "",
                        state = "",
                        state_code = "",
                        postcode = "",
                        country = "",
                        country_code = "",
                        quarter = "",
                        town = "",
                        village = ""
                    } = data.address || {}; // Use `|| {}` in case `data.address` itself is undefined


                    var fullAddress = data.display_name;
                    var street = [road, quarter].filter(Boolean).join(", ");
                    var locality = [suburb, county, city, town, village].filter(Boolean).join(", ");
                    var district = state;

                    $("#Street").val(street)
                    $("#Locality").val(locality)
                    $("#District").val(district)

                    // Set the Address field without triggering the autocomplete dropdown
                    $("#Address")
                        .val(fullAddress)
                        .data('autocomplete-suppress', true) // Custom flag to suppress autocomplete
                        .change();

                    $.ajax({
                        url: new Variable().LocationIQURL + 'autocomplete',
                        data: {
                            key: new Variable().LocationIQAccessToken,
                            q: fullAddress,
                            format: "json",
                            limit: 10,
                            countrycodes: "mu"
                        },
                        dataType: 'json',
                        success: function (response) {
                            if (response.length > 0) {
                                // Automatically select the first suggestion
                                displaySelectedAddress(response[0]);
                            }
                        },
                        error: function (xhr, status, error) {
                            console.error("Error fetching autocomplete suggestions:", error);
                        }
                    });

                })
            });


            map.on('draw:drawstart', function (e) {
                map.removeLayer(featureLayer);
                $("#GeomDataText").val("");
            })

        }
    }
}
window.MapService = new MapService();