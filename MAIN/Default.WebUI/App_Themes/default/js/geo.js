$(function() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function(pos) {
            var coordinate = pos.coords.latitude + ',' + pos.coords.longitude;
            //$.ajax('/ajax/savelocation/' + encodeURIComponent(coordinate));
        }, function(error) {
            switch (error.code) {
                case error.PERMISSION_DENIED:
                    console.log('');
                    break;
                case error.POSITION_UNAVAILABLE:
                    // No position available
                    break;
                case error.TIMEOUT:
                    // Request timed out
                    break;
                case error.UNKNOWN_ERROR:
                    // Unknown error
                    break;
            }
        });
    } else {
        // GEOLOCATION not supported.
    }
});