$('#searchBox').autocomplete({
    source: 'Search/Autocomplete'
});

var showHeaderElements = function () {

    $('.pageHeaderShowOnly').each(function () {
        var element = $(this);

        var showed = infCon3.FirstItemVisible() && $(window).scrollTop() <= 10;
        if (showed) {
            element.addClass("showed");
        } else {
            element.removeClass("showed");
        }
    });

}

$(document).ready(function () {
    infCon3.CustomAnimations.Animations = [
        {
            selector: ".widget-grid .three-column .col-md-4 .icon",
            animations: [{ dir: "top", ani: "fadeIn" }, { dir: "bottom", ani: "fadeIn" }],
            preHidden: true
        },
        {
            selector: ".widget-grid .three-column .col-md-4 .image",
            animations: [{ dir: "top", ani: "fadeIn" }, { dir: "bottom", ani: "fadeIn" }],
            preHidden: true
        },
        {
            selector: ".cm-widget.widget-grid .twocol-left .col-md-5 .icon",
            animations: [{ dir: "top", ani: "fadeIn" }, { dir: "bottom", ani: "fadeIn" }],
            preHidden: true
        },
        {
            selector: ".cm-widget.widget-grid .twocol-left .col-md-5 .image",
            animations: [{ dir: "top", ani: "fadeIn" }, { dir: "bottom", ani: "fadeIn" }],
            preHidden: true
        },
        {
            selector: ".cm-widget.widget-grid .twocol-right .col-md-5 .icon",
            animations: [{ dir: "top", ani: "fadeIn" }, { dir: "bottom", ani: "fadeIn" }],
            preHidden: true
        },
        {
            selector: ".cm-widget.widget-grid .twocol-right .col-md-5 .image",
            animations: [{ dir: "top", ani: "fadeIn" }, { dir: "bottom", ani: "fadeIn" }],
            preHidden: true
        }
    ];

    infCon3.ShowWarningsAndInfos = false;

    infCon3.init();

    $('[data-toggle=collapse-next]').click(function (e) {
        e.preventDefault();
        var $target = $(this).parents('.panel').find('.panel-collapse');
        $(this).toggleClass('show');
        $target.collapse('toggle');
    });

    $('.geoswitched').click(function (e) {
        e.preventDefault();
        $(this).toggleClass('switched');
        $('#Lünen').toggle();
        $('#Dortmund').toggle();
    });

    infCon3.OnPartialLoaded.push(function (element, success) {
        if (!success) {
            return;
        }
        element.find('[data-toggle=collapse-next]').click(function (e) {
            e.preventDefault();
            var $target = $(this).parents('.panel').find('.panel-collapse');
            $(this).toggleClass('show');
            $target.collapse('toggle');
        });

        element.find('.geoswitched').click(function (e) {
            e.preventDefault();
            $(this).toggleClass('switched');
            $('#Lünen').toggle();
            $('#Dortmund').toggle();
        });

        element.find('.widget-maps').click(function (e) {
            $(this).find('iframe').css('pointer-events', 'all');
        }).mouseleave(function (e) {
            $(this).find('iframe').css('pointer-events', 'none');
        });
    });

    $(window).scroll(function () {
        navigation();
        footer();

        function navigation() {
            ////// CONFIG 
            var MARGIN = 50;

            var breakePointTop = 0;
            if (infCon3.Partials.Items[0].Visible) {
                breakePointTop = infCon3.Partials.Items[0].Element.offset().top + infCon3.Partials.Items[0].Element.outerHeight();
            }
            var breakePointBottom = $(document).height();
            if (infCon3.Partials.Items[infCon3.Partials.Items.length - 1].Visible) {
                breakePointBottom = infCon3.Partials.Items[infCon3.Partials.Items.length - 1].Element.offset().top;
            }

            var navigation = $("nav#navigation");

            var viewCenter = $(window).scrollTop() + $(window).outerHeight() / 2;
            var navTop = viewCenter - navigation.outerHeight() / 2;
            var navBottom = viewCenter + navigation.outerHeight() / 2;

            var navOffsetTop = breakePointTop + MARGIN - navTop;
            var navOffsetBottom = -1 * (breakePointBottom - MARGIN - navBottom);

            if (navOffsetBottom > 0 && navOffsetTop > 0) {
                infCon3.log("displayed area for navigation is to small.", "warning");
                return;
            }
            if (navOffsetBottom > 0) {
                navigation.css("margin-top", (-1 * navOffsetBottom));
                return;
            }
            if (navOffsetTop > 0) {
                navigation.css("margin-top", navOffsetTop);
                return;
            }
            navigation.css("margin-top", 0);
        }

        function footer() {
            var foot = $('#dynamic-footer');
            if (foot == undefined || foot == null || foot.length <= 0) {
                return;
            }
            if (infCon3.LastItemVisible()) {
                foot.css('visibility', 'visible');
            } else {
                foot.css('visibility', 'hidden');
            }
        }
    });


    //$('.pageHeaderShowOnly').each(function () {
    //    var element = $(this);
    //    function show(element) {
    //        var showed = infCon3.FirstItemVisible() && $(window).scrollTop() <= 10;
    //        if (showed) {
    //            element.addClass("showed");
    //        } else {
    //            element.removeClass("showed");
    //        }
    //    }
    //    show(element);
    //    $(window).scroll(function () {
    //        show(element);
    //    });
    //});

    $(window).scroll(function () {
        showHeaderElements();
    });

    showHeaderElements();


    $('.header-logo').click(function () {
        if (infCon3.Partials.Items.length <= 0) {
            return;
        }
        infCon3.NavigationPartials.navigateTo(infCon3.Partials.Items[0]);
    });
});
