$("#slider").excoloSlider({
    mouseNav: false,
    interval: 8000, // = 8 seconds
    playReverse: true
});

$(document).ready(function () {
    setTimeout(function () {
        $('#zoomPreview').zoom();
    }, 1500);
});


/*http://www.bootply.com/132400*/
$('.carousel[data-type="multi"] .item').each(function () {
    var next = $(this).next();
    if (!next.length) {
        next = $(this).siblings(':first');
    }
    next.children(':first-child').clone().appendTo($(this));

    for (var i = 0; i < 2; i++) {
        next = next.next();
        if (!next.length) {
            next = $(this).siblings(':first');
        }

        next.children(':first-child').clone().appendTo($(this));
    }
});

// change the viewport to disable zooming on mobile devices. (from old page)
$(function () {
    function adaptViewport() {
        viewport = document.querySelector("meta[name=viewport]");
        if (isMobile()) {
            viewport.setAttribute('content', 'width=device-width, maximum-scale=1, minimum-scale=0.8, user-scalable=0');
        } else {
            viewport.setAttribute('content', 'width=device-width, initial-scale=1, user-scalable=1');
        }
    }

    adaptViewport();

    $(window).resize(function () {
        adaptViewport();
    });
});

function isMobile() {
    return $("#search-mobile").css("visibility") === "visible";
}

/*GO BACK*/
$(".btn-back").on("click", function () {
    goBack();
});



function goBack() {
    var oldLocation = document.referrer;
    var currentLocation = document.location.href;


    if (oldLocation != currentLocation && oldLocation != "" && currentLocation.toLowerCase().indexOf('kontakt') == -1) {
        GoToLastWhichIsNotContactPage(oldLocation);
    }
}

$(function () {
    var CreatisBackButton = function (selector) {
        $(selector).on('click', $.proxy(this._clickHandler, this));
    };

    CreatisBackButton.prototype._clickHandler = function () {
        var oldLocation = document.referrer;
        var currentLocation = document.location.href;
        if (oldLocation != currentLocation && oldLocation != "") {
            GoToLastWhichIsNotContactPage(oldLocation);
        }
    };

    var responsiveButtons = new CreatisBackButton('.responsive-back-btn');
});

function GoToLastWhichIsNotContactPage(oldLocation) {
    var contactUrlContains = 'kontakt';
    if (oldLocation != null && oldLocation.toLowerCase().indexOf(contactUrlContains) > -1) {
        window.history.go(-2);
    } else {
        window.history.back();
    }
}

$(function () {
    $('#slide-panel .slide-header .btn-close, #slide-panel .slide-header').on('click', function (evt) {
        evt.preventDefault();
        onSlideUpdate(true);
        return false;
    });

    window.onSlideUpdate = function (change) {

        change = change || false;

        var panel = $('#slide-panel'),
            panelHeight = panel.height(),
            headerHeight = panel.find('.slide-header').height(),
            slideOffset = panelHeight - headerHeight,
            panelButton = panel.find('.btn-close'),
            btnOpenUrl = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAUBAMAAAB/pwA+AAAAGFBMVEUAAAD///8AAAAAAAAAAAAAAAAAAAAAAAB5sPD/AAAAB3RSTlMAABBgj5/PGcmfkAAAAEZJREFUeF6tzrERgCAMRuEfD3s9GMCCo2cDdQK69DSZIJnfO8VMkFd95cNuuXBDPBEmSX6uyn2SRpWPWY+Fr5fpBkrzebAeIA0QvfpaPugAAAAASUVORK5CYII=',
            btnCloseUrl = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABQAAAAUBAMAAAB/pwA+AAAAFVBMVEX///9ERERERERERERERERERERERETet/PbAAAABnRSTlMAEGCPn88iC3JZAAAAO0lEQVR4Xq3GOw0AIAwFwAdFAAlhR0JxwMTOUgWtfwskfBz0poOz2IHSTslyEMYxV1VcyWTgmYqPGN425hQEUv4NHHwAAAAASUVORK5CYII=';


        if (change === true) { //toggle
            if (panel.hasClass("visible")) {
                panel.removeClass('visible').animate({ 'margin-bottom': '-' + slideOffset + 'px' });
                panelButton.css('background-image', 'url(' + btnOpenUrl + ')');
            } else {
                panel.addClass('visible').animate({ 'margin-bottom': '0px' });
                panelButton.css('background-image', 'url(' + btnCloseUrl + ')');
            }
        } else if (change === false) { //reset, fix
            if (!panel.hasClass("visible")) {
                panel.animate({ 'margin-bottom': '-' + slideOffset + 'px' });

            } else {
                panel.animate({ 'margin-bottom': '0px' });
            }
        }
        else if (change === "show") {
            panel.addClass('visible').animate({ 'margin-bottom': '0px' });
            panelButton.css('background-image', 'url(' + btnCloseUrl + ')');
        }
        else if (change === "hide") {
            panel.removeClass('visible').animate({ 'margin-bottom': '-' + slideOffset + 'px' });
            panelButton.css('background-image', 'url(' + btnOpenUrl + ')');
        }
    };

    //var scrollButton = new ScrollToTop(40);

    //  Form validation

    //function revalidateControls(containerSelector) {
    //    console.log('Revalidate for "' + containerSelector + ' .form-group"');
    //    var result = false;
    //    $(containerSelector + ' .form-group').each(function (i, e) {
    //        console.log('Checking element #' + i);
    //        var self = $(e),
    //            validate = self.data('validate'),
    //            input = self.find('input,textarea');
    //        if (validate) {
    //            var valRequired = input.data('val-required'),
    //                valPattern = input.data('val-regex-pattern');
    //            var mySelf = $(this),
    //                error = false;
    //            if (valRequired) {
    //                console.log('[' + i + '] => Value required');
    //                if (mySelf.find('input,textarea').val() === '') {
    //                    console.log('[' + i + '] => Value not present');
    //                    error = true;
    //                }
    //            }
    //            if (valPattern) {
    //                console.log('[' + i + '] => Pattern match required');
    //                var regEx = new RegExp(valPattern),
    //                    value = mySelf.find('input,textarea').val();
    //                if (!regEx.test(value)) {
    //                    console.log('[' + i + '] => Pattern and value doesnt match');
    //                    error = true;
    //                }
    //            }
    //            result = error;
    //        }
    //    });

    //    if (result) {
    //        console.log('Revalidation finished: Locking button');
    //        $(containerSelector + '.btn-next').attr('disabled', 'disabled');
    //    } else {
    //        console.log('Revalidation finished: Unlock button');
    //        $(containerSelector + '.btn-next').removeAttr('disabled');
    //    }
    //}

    //function registerFormValidation(containerSelector) {
    //    console.log('Register validation for "' + containerSelector + ' .form-group"');
    //    $(containerSelector + ' .form-group').each(function (i, e) {
    //        var self = $(e),
    //            validate = self.data('validate'),
    //            input = self.find('input,textarea'),
    //            span = self.find('span');
    //        if (validate) {
    //            var valRequired = input.data('val-required'),
    //                valPattern = input.data('val-regex-pattern'),
    //                valPatternError = input.data('val-regex');
    //            input.on('keyup', function () {
    //                var mySelf = $(this),
    //                    error = false,
    //                    errorText = '';

    //                if (valRequired) {
    //                    console.log('[Update] => Value required');
    //                    if (mySelf.val() === '') {
    //                        console.log('[Update] => Value not present');
    //                        error = true;
    //                        errorText = valRequired;
    //                    }
    //                }

    //                if (valPattern) {
    //                    console.log('[Update] => Pattern match required');
    //                    var regEx = new RegExp(valPattern),
    //                        value = mySelf.val();
    //                    if (!regEx.test(value)) {
    //                        console.log('[Update] => Pattern and value doesnt match');
    //                        error = true;
    //                        errorText = valPatternError;
    //                    }
    //                }

    //                if (error) {
    //                    console.log('Revalidation finished: Locking button');
    //                    span.text(errorText);
    //                    span.css('display', 'block');
    //                } else {
    //                    console.log('Revalidation finished: Unlock button');
    //                    span.html('&nbsp;');
    //                    span.css('display', 'none');
    //                }
    //            });
    //        }
    //    });
    //setInterval(function () {
    //    revalidateControls(containerSelector);
    //}, 1000);
    //}

    //registerFormValidation('#contactForm');
    //registerFormValidation('#editionForm');
    //registerFormValidation('#editionsForm');
});


//console.log('asfasfasfsafasf');

//$('#searchBox').autocomplete({
//    source: '@Url.Action("Autocomplete", "Search")'
//});


$(document).ready(function () {
    $('#searchBox').autocomplete({
        source: 'Search/Autocomplete'
    });
})


$(function () {
    $('#widgetList').jscroll();
});

var widgeteditdyn = function (event) {
    event.preventDefault();
    var target;

    if ($(event.target).is("button")) {
        target = $(event.target);
    } else {
        target = $(event.target).parent();
    }

    if(parent.$(".isinadmineditframe").length == 1){
        parent.location = target.attr("href").replace("widgeteditdyn", "widgetedit");
    }
    else {
        widgetEditModal(target.attr("href"));
    }
};

var widgetdeletedyn = function (event, name, id) {
    event.preventDefault();
    var target;

    if ($(event.target).is("button")) {
        target = $(event.target);
    } else {
        target = $(event.target).parent();
    }

    widgetDeleteModal(target.attr("href"), name, id);
};

var widgetmovefirst = function (event, id) {
    event.preventDefault();
    var target;

    if ($(event.target).is("button")) {
        target = $(event.target);
    } else {
        target = $(event.target).parent();
    }

    $.ajax({
        url: target.attr("href"),
        type: 'get',
        success: function () {
            $("[modelid=" + id + "]").prependTo(".inf-con-root");
        }
    });
};

var widgetmovelast = function (event, id) {
    event.preventDefault();
    var target;

    if ($(event.target).is("button")) {
        target = $(event.target);
    } else {
        target = $(event.target).parent();
    }

    $.ajax({
        url: target.attr("href"),
        type: 'get',
        success: function () {
            $("[modelid=" + id + "]").appendTo(".inf-con-root");
        }
    });
};

var widgetmoveup = function (event, id) {
    event.preventDefault();
    var target;

    if ($(event.target).is("button")) {
        target = $(event.target);
    } else {
        target = $(event.target).parent();
    }

    $.ajax({
        url: target.attr("href"),
        type: 'get',
        success: function () {
            $("[modelid=" + id + "]").insertBefore($("[modelid=" + id + "]").prev());
        }
    });
};

var widgetmovedown = function (event, id) {
    event.preventDefault();
    var target;

    if ($(event.target).is("button")) {
        target = $(event.target);
    } else {
        target = $(event.target).parent();
    }

    $.ajax({
        url: target.attr("href"),
        type: 'get',
        success: function () {
            $("[modelid=" + id + "]").insertAfter($("[modelid=" + id + "]").next());
        }
    });
};

var executeLinkFunction = function (event) {
    event.preventDefault();

    var target;

    if ($(event.target).is("button")) {
        target = $(event.target);
    } else {
        target = $(event.target).parent();
    }

    target.load(target.attr("href"), function () {
        location.reload();
    });
};

var ShowShare = function (elem) {
    $(elem).siblings().removeClass('hidden');
    $(elem).addClass('hidden');
}

function UnCrypt(s) {
    var n = 0;
    var r = "";
    for (var i = 0; i < s.length; i++) {
        n = s.charCodeAt(i);
        if (n >= 8364) {
            n = 128;
        }
        r += String.fromCharCode(n - 1);
    }
    return r;
}

function linkTo_UnCrypt(s) {
    location.href = UnCrypt(s);
}