

function core_stringToUrl(s) {
    s = s.split(" ").join("-");
    s = s.split("ö").join("oe");
    s = s.split("ä").join("ae");
    s = s.split("ü").join("ue");
    return s.replace(/[^A-Za-z\-!?]/g, '');
}

function writeUrlToAttr(id, attr, str) {
    writeToAttr(id, attr, core_stringToUrl(str));
}
function writeToAttr(id, attr, str) {
    if (attr == 'innerHtml') {
        $('#' + id).html(str);
    }
    $('#' + id).attr(attr, str);
}

function countWords(str) {
    var matches = str.match(/[A-Za-z0-9\-_]+/g);
    return matches ? matches.length : 0;
}
function countKeywords(str) {
    return str.split(',').length;
}


var GoogleMapsAPI = {
    loaded: false,
    loading: false,
    mapInitializes: [],
    maps: [],

    initialize(func) {
        if (this.loaded) {
            func();
        }
        this.mapInitializes.push(func);
        if (!this.loading) {
            this.loading = true;
            this.init();
        }
    },

    googleAPICallback: function () {
        GoogleMapsAPI.loaded = true;
        GoogleMapsAPI.mapInitializes.forEach(function (item, id) {
            GoogleMapsAPI.maps.push(item());
        });
    },
    init: function () {
        if (!this.loaded) {
            var script = document.createElement("script");
            script.src = "https://maps.googleapis.com/maps/api/js?key=AIzaSyBszxMTbF0c_LBw21HKocOFatQKdRaZ8n4&callback=GoogleMapsAPI.googleAPICallback";
            document.getElementsByTagName("head")[0].appendChild(script);
        }
    }


}