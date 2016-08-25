/*global console: false, creatis_carpenterStorage_replaceContentFromStorage: false */

$(function () {

    function initDesktop() {
        $("#accordion").accordion({
            collapsible: true,
            active: localStorage.selectedCarpenter ? false : true,
        });

        // Kontaktseite Suche
        $("#search-site-submit").click(function () {
            var searchQuery = $("#search-query-site").val();
            if (isValidPostal(searchQuery)) {    
                document.location = "/tischler?query=" + searchQuery;
            }
            return false;
        });

        $("#search-query-site").on('input', function () {
            var isValid = isValidPostal($("#search-query-site").val());
            $("#search-query-site").css('border-color', !isValid ? 'red' : '#d5d5d5');
        });


        $("#search-query").on('input', function () {
            var isValid = isValidPostal($("#search-query").val());
            $("#search-query").css('border-color', !isValid ? 'red' : '#d5d5d5');
        });

        //Prevent default on enter
        $('#search-query').keypress(function (event) {
            if (event.keyCode == 10 || event.keyCode == 13)
                event.preventDefault();

        });

        $("#accordion").removeClass('c-hidden');
        $("#search-flyout-submit").click(function () {
            var searchQuery = $("#search-query").val();
            if (searchQuery !== null && searchQuery !== "" && isValidPostal(searchQuery)) {
                document.location = "/tischler?query=" + searchQuery;
            }
            return false;
        });
        if (typeof (creatis_carpenterStorage_replaceContentFromStorage) !== "undefined") {
            creatis_carpenterStorage_replaceContentFromStorage();
        }
    }

    function initMobile() {
        //code taken from https://github.com/codrops/ButtonComponentMorph/blob/master/index.html

        var docElem = window.document.documentElement, didScroll, scrollPosition;

        // trick to prevent scrolling when opening/closing button
        function noScrollFn() {
            window.scrollTo(scrollPosition ? scrollPosition.x : 0, scrollPosition ? scrollPosition.y : 0);
        }

        function noScroll() {
            window.removeEventListener('scroll', scrollHandler);
            window.addEventListener('scroll', noScrollFn);
        }

        function scrollFn() {
            window.addEventListener('scroll', scrollHandler);
        }

        function canScroll() {
            window.removeEventListener('scroll', noScrollFn);
            scrollFn();
        }

        function scrollHandler() {
            if (!didScroll) {
                didScroll = true;
                setTimeout(function () { scrollPage(); }, 60);
            }
        };

        function scrollPage() {
            scrollPosition = { x: window.pageXOffset || docElem.scrollLeft, y: window.pageYOffset || docElem.scrollTop };
            didScroll = false;
        };

        scrollFn();


        // Mobile carpenter search button

        var mobileSearchButton = document.querySelector('#search-mobile .morph-button');

        $('#search-mobile .morph-button').click(function (ev) {
            if (ev.originalEvent) {
                ev.originalEvent.preventDefault();
            }
        });

        var mobileSearchMorphingButton = new UIMorphingButton(mobileSearchButton, {
            closeEl: '.icon-close',
            onBeforeOpen: function () {
                // don't allow to scroll
                noScroll();
            },
            onAfterOpen: function () {
                // can scroll again
                canScroll();

                $('.dialog-page').hide();
                $('.mobile-search-page-overview').show();
            },
            onBeforeClose: function () {
                // don't allow to scroll
                noScroll();
            },
            onAfterClose: function () {
                // can scroll again
                if (window.buyWithoutCarpenter && localStorage.selectedCarpenter !== null) {
                    $('#purchase-request-starter')[0].click();
                }
                else
                {
                    window.buyWithoutCarpenter = false;
                }
                canScroll();
            }
        });
    }

    (function startup() {
        if (isMobile()) {
            initMobile();
        } else {
            if ($("#accordion").length > 0) {
                initDesktop();
            }
        }
        //initDesktop();
        //initMobile();
        displaySelectedCarpenter();
    })();
});

function isMobile() {
    return $("#search-mobile").css("visibility") === "visible";
}

function displaySelectedCarpenter() {
    if (localStorage.selectedCarpenter) {
        $('#div_text').css('display', 'none');
        var data = JSON.parse(localStorage.getItem('selectedCarpenter'));
        var cId = data.id;
        setTimeout(function () {
            $('#' + cId + ' .btn-select').first().trigger('click');
        }, 50);
    }
}