var infinitecontent = {
    init: function () {
        infinitecontent.alignNavigation();

        var hash = (location.pathname + "/").replace(/[/]+/g, "/");
        hash += location.search;


        $(window).scroll(infinitecontent.scrollHandler);
        if (hash.length > 1) {
            var e = $("section[id='" + hash + "'].inf-con-elem[inf-con-state=loaded]");
            infinitecontent.scrollToElement(e, function () {
                var h = e.prop("scrollHeight");
                infinitecontent.loadArea(e, h, $(window).height(), false, function (contentHeight) {
                    contentHeight = Math.min(contentHeight, $(window).height() - 1);
                    infinitecontent.loadArea(e, contentHeight, $(window).height(), true);
                });
            });
        }
        else {
            infinitecontent.loadElement($(".inf-con-elem[inf-con-state=notloaded]").first());
        }
        infinitecontent.scrollHandler();

    },
    loadArea: function (element, contentHeight, maxHeight, up, _callback) {
        if (contentHeight > maxHeight) {
            if (_callback != null) {
                _callback(contentHeight);
            }
            return;
        }
        if (up) {
            var prev = element.prev(".inf-con-elem[inf-con-state=notloaded]");
            if (prev.length <= 0) {
                if (_callback != null) {
                    _callback(contentHeight);
                }
                return;
            }
            $(".inf-con-loading").addClass("active").removeClass("bottom");
            prev.hide();

            infinitecontent.loadElement(prev, function (x) {
                x.show();
                $(window).scrollTop($(window).scrollTop() + x.prop("scrollHeight"));
                $(".inf-con-loading").removeClass("active");
                infinitecontent.loadArea(prev, contentHeight + x.prop("scrollHeight"), maxHeight, true, _callback);
            });
        } else {

            var next = element.next(".inf-con-elem[inf-con-state=notloaded]");
            if (next.length <= 0) {
                if (_callback != null) {
                    _callback(contentHeight);
                }
                return;
            }
            next.hide();
            $(".inf-con-loading").addClass("active").removeClass("bottom");
            infinitecontent.loadElement(next, function (x) {
                x.show();
                $(".inf-con-loading").removeClass("active");
                infinitecontent.loadArea(next, contentHeight + x.prop("scrollHeight"), maxHeight, false, _callback);
            });
        }
    },

    scrollToElement: function (element, callback) {
        if (element.length <= 0) {
            return;
        }
        var offset = element.offset().top;
        var scrollpos = 0;

        if (element.prop("scrollHeight") < $(window).height()) {
            scrollpos = offset - ($(window).height() - element.prop("scrollHeight")) / 2;
        }
        else {
            scrollpos = offset;
        }

        if (scrollpos > 0) {
            $("body").animate({
                scrollTop: scrollpos
            }, "slow").promise().done(function () {
                if (callback != null) {
                    callback(element);
                }
            });
        }
        else {
            if (callback != null) {
                callback(element);
            }
        }
    },
    loadElementArray: function (elements, peritemcallback, loadstartedcallback, errorcallback, callback) {
        var pending = [];

        for (var i = 0; i < elements.length; i++) {
            var element = $(elements[i]);
            pending.push(element.attr("id"));

            infinitecontent.loadElement(element, function (x) {
                pending.splice(pending.indexOf(x.attr("id")), 1);

                if (peritemcallback != null) {
                    peritemcallback(x);
                }

                if (callback != null && pending.length == 0) {
                    callback();
                }
            }, loadstartedcallback, errorcallback);
        }
    },
    loadElement: function (element, callback, loadstartedcallback, errorcallback) {
        if (element.length > 0) {
            element.attr("inf-con-state", "loading");

            if (loadstartedcallback != null) {
                loadstartedcallback(element);
            }

            element.load(element.attr("inf-con-href"), null, function (e, status) {
                if (status == "success") {
                    element.attr("inf-con-state", "loaded");

                    /*Add event listeners for collapse buttons*/
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

                    infinitecontent.alignNavigation();

                    element.find('a.inf-con-nav-a').click(function (e) {

                        infinitecontent.clickHandler(e);
                    });

                    element.waitForImages(function () {
                        if (callback != null) {
                            callback(element);
                        }
                    });
                }
                else {
                    element.attr("inf-con-state", "error");

                    if (errorcallback != null) {
                        errorcallback();
                    }
                }
            });
        } else {
            if (errorcallback != null) {
                errorcallback();
            }
        }
    },
    isLoaded: function (elementid) {
        return $("section[id='" + elementid + "'].inf-con-elem[inf-con-state=loaded]").length > 0;
    },
    isLoading: function (elementid) {
        return $("section[id='" + elementid + "'].inf-con-elem[inf-con-state=loading]").length > 0;
    },
    prevscrollpos: 0,
    preloaddistance: 500,
    scrollHandler: function () {
        var newpos = $(window).scrollTop();

        /*Position navigation*/
        if (newpos < infinitecontent.cacheddata.navTopOffset) {
            $("nav#navigation").css("margin-top", infinitecontent.cacheddata.navTopOffset - newpos);
        } else if (newpos + $(window).height() > $(document).height() - infinitecontent.cacheddata.navBottomOffset) {
            $("nav#navigation").css("margin-top", -(newpos + $(window).height() - ($(document).height() - infinitecontent.cacheddata.navBottomOffset)));
        } else {
            $("nav#navigation").css("margin-top", 0);
        }

        var wait = false;

        /*Load elements on scroll*/
        if (!infinitecontent.blocked && infinitecontent.current.length > 0) {
            if ($(".inf-con-elem[inf-con-state=loaded]").length) {
                if (newpos > infinitecontent.prevscrollpos) {

                    if (($(document).height() - infinitecontent.cacheddata.navBottomOffset) - (newpos + $(window).height()) <= 200) {

                        var next = infinitecontent.current.nextAll("[inf-con-state=notloaded]");

                        if (next.length > 0) {
                            if (next.length > 1) {
                                next = [next[0], next[1]];

                                infinitecontent.loadElementArray(next, function (e) {
                                    var height = e.height();

                                    e.height(0).animate({ height: height }, 2000, function () {
                                        e.removeAttr("style height");
                                    });
                                });
                            }
                            else {
                                infinitecontent.loadElement(next, function (e) {
                                    var height = e.height();

                                    e.height(0).animate({ height: height }, 2000, function () {
                                        e.removeAttr("style height");
                                    });
                                });
                            }
                        }
                    }
                    else {
                        var next = infinitecontent.current.next("[inf-con-state=notloaded]");

                        if (next.length > 0) {
                            if (infinitecontent.getPercentageOfDisplayedContent(infinitecontent.current) > 90 || newpos + infinitecontent.preloaddistance > infinitecontent.current.offset().top) {
                                infinitecontent.loadElement(next, function (e) {
                                    var height = e.height();

                                    e.height(0).animate({ height: height }, 2000, function () {
                                        e.removeAttr("style height");
                                    });
                                });
                            }
                        }
                    }
                }
                else {
                    var prevunloaded = infinitecontent.current.prevAll("[inf-con-state=notloaded]");

                    if (prevunloaded.length > 0) {
                        prevunloaded = $(prevunloaded[0]);
                        var lastloaded = prevunloaded.next();

                        if (newpos - infinitecontent.preloaddistance < lastloaded.offset().top) {
                            infinitecontent.block();
                            $(".inf-con-loading").addClass("active").removeClass("bottom");
                            prevunloaded.hide();
                            infinitecontent.setScrollable(false);

                            infinitecontent.loadElement(prevunloaded, function (x) {
                                infinitecontent.setScrollable(true);
                                x.show();
                                $(".inf-con-loading").removeClass("active");
                                $(window).scrollTop($(window).scrollTop() + x.prop("scrollHeight"));
                                infinitecontent.unblock();
                            });
                        }
                    }


                    //var prev = infinitecontent.current.prev();

                    //if (newpos - infinitecontent.preloaddistance < infinitecontent.current.offset().top && prev.length > 0) {
                    //    infinitecontent.block();
                    //    $(".inf-con-loading").addClass("active").removeClass("bottom");
                    //    prev.hide();

                    //    infinitecontent.loadElement(prev, function (x) {
                    //        x.show();
                    //        $(window).scrollTop($(window).scrollTop() + x.prop("scrollHeight"));
                    //        $(".inf-con-loading").removeClass("active");
                    //        infinitecontent.unblock();
                    //    });
                    //}
                }
            }
        }

        /*Get Current Element*/
        var inview = $(".inf-con-elem[inf-con-state=loaded]").toArray().map(function (x) {
            return { element: x, displayed: infinitecontent.getPercentageOfDisplayedContent($(x)), portion: infinitecontent.getPercentageInView($(x)) }
        }).filter(function (x) {
            return x.displayed > 0;
        });

        var full = inview.filter(function (x) {
            return x.displayed == 100;
        });

        if (full.length == 0) {
            if (inview.length > 0) {
                var ordered = inview.sort(function (x, y) {
                    return x.portion == y.portion ? 0 : x.portion > y.portion ? -1 : 1;
                });

                infinitecontent.current = $(ordered[0].element);
            } else {
                infinitecontent.current = [];
            }
        } else if (full.length == 1) {
            infinitecontent.current = $(full[0].element);
        } else if (full.length % 2 == 0) {
            infinitecontent.current = $(full[full.length / 2 - 1].element);
        } else if (full.length % 2 == 1) {
            infinitecontent.current = $(full[(full.length - 1) / 2 - 1].element);
        }

        /*Set active Navigation Link and Location*/
        $(".inf-con-nav").removeClass("active");


        if (infinitecontent.current.length > 0) {
            if (infinitecontent.current.attr("has-navigation") != null) {
                $(".inf-con-nav a[href='" + infinitecontent.current.attr("id") + "']").parent().addClass("active");
            } else {
                var prev = infinitecontent.current.prevAll("[has-navigation]");

                if (prev.length > 0) {
                    $(".inf-con-nav a[href='" + $(prev[0]).attr("id") + "']").parent().addClass("active");
                }
            }

            infinitecontent.writeHash(infinitecontent.current.attr("id"), infinitecontent.current);

            if (infinitecontent.previous.length == 0 || infinitecontent.previous.attr("id") != infinitecontent.current.attr("id")) {
                infinitecontent.previous = infinitecontent.current;
                infinitecontent.animateCurrent();
            }

        }
        else {
            infinitecontent.previous = [];
            infinitecontent.current = [];
            infinitecontent.animateCurrent();
            infinitecontent.writeHash("", null);
        }

        infinitecontent.prevscrollpos = newpos;
    },
    animateCurrent: function () {
        for (var i = 0; i < infinitecontent.customAnimations.length; i++) {
            var animation = infinitecontent.customAnimations[i];

            if (!animation.once == true) {
                if (animation.attrCondition != null) {
                    $("[" + animation.attrCondition.attr + "=" + animation.attrCondition.value + "] " + animation.selector).removeClass(animation.animation).addClass(animation.animationout);
                }
                else {
                    $(animation.selector).removeClass(animation.animation).addClass(animation.animationout);
                }
            }


            if (infinitecontent.current.length > 0) {
                if (animation.attrCondition != null) {
                    if (infinitecontent.current.attr(animation.attrCondition.attr) == animation.attrCondition.value) {
                        infinitecontent.current.find(animation.selector).removeClass(animation.animationout).addClass("animated").addClass(animation.animation);
                    }
                }
                else {
                    infinitecontent.current.find(animation.selector).removeClass(animation.animationout).addClass("animated").addClass(animation.animation);
                }
            }
        }
    },
    getBetween: function (element1, element2) {
        return element1.nextUntil(element2[0]);
    },
    getPercentageInView: function (element) {
        var offset = element.offset().top;
        var height = element.prop("scrollHeight");
        var win = $(window);

        if (offset > (win.scrollTop() + win.height()) || offset + height < win.scrollTop()) {
            return 0;
        } else {
            var topsub = offset - win.scrollTop();
            var bottomsub = ((win.scrollTop() + win.height()) - (offset + height));

            var heightinview = height + (topsub < 0 ? topsub : 0) + (bottomsub < 0 ? bottomsub : 0);

            var percentage = heightinview / win.height() * 100;
            return Math.round(percentage);
        }
    },
    getPercentageOfDisplayedContent: function (element) {
        var offset = element.offset().top;
        var height = element.prop("scrollHeight");
        var win = $(window);

        if (offset > (win.scrollTop() + win.height()) || offset + height < win.scrollTop()) {
            return 0;
        } else {
            var topsub = offset - win.scrollTop();
            var bottomsub = ((win.scrollTop() + win.height()) - (offset + height));

            var heightinview = height + (topsub < 0 ? topsub : 0) + (bottomsub < 0 ? bottomsub : 0);

            var percentage = heightinview / height * 100;
            return Math.round(percentage);
        }
    },
    isElementInView: function (element) {
        return element.offset().top >= $(window).scrollTop() && element.offset().top < $(window).scrollTop() + $(window).height();
    },
    current: [],
    previous: [],
    block: function () {
        infinitecontent.blocked = true;
    },
    unblock: function () {
        infinitecontent.blocked = false;
        infinitecontent.writeHash(infinitecontent.current.attr("id"), infinitecontent.current);
    },
    navigationMargin: "",
    cachedobjects: {},
    cacheddata: {},
    blocked: false,
    writeHash: function (hash, element) {

        if (history.pushState) {
            var h = ('/' + hash + '/').replace(/[\/]+/g, '/');
            var lh = (location.href + '/').replace(/[\/]+/g, '/');
            if (lh.endsWith(h) || infinitecontent.blocked) {
            } else {
                history.pushState(null, null, hash);
            }
        }
        else {
            var scrollpos = newpos;
            location.href = hash;
            $(window).scrollTop(scrollpos);
        }
        if (element != null) {
            var metas = element.attr('inf-con-meta').split(';');
            for (var k = 0; k < metas.length; k++) {
                var mapping = metas[k].split(':');
                var key = mapping[0];
                var value = mapping[1];
                if (key == 'Title') {
                    document.title = value;
                    // Hier könnte man auch alle MetaTags ändern!
                    continue;
                }
            }
        }
    },
    setScrollable: function (mode) {
        if (mode == true) {
            if (window.addEventListener) {
                window.removeEventListener("DOMMouseScroll", function (e) { e.preventDefault(); }, false);
            }
            window.onwheel = null;
            window.onmousewheel = null;
            window.ontouchmove = null;
        } else {
            if (window.addEventListener) {
                window.addEventListener("DOMMouseScroll", function (e) { e.preventDefault(); }, false);
            }
            window.onwheel = function (e) { e.preventDefault(); };
            window.onmousewheel = function (e) { e.preventDefault(); };
            window.ontouchmove = function (e) { e.preventDefault(); };
        }
    },
    customAnimations: [
    {
        selector: ".cm-widget.widget-title .icon",
        animation: "bounce",
        animationout: "fadeOut",
        once: true
    },
    {
        selector: ".widget-grid .three-column .col-md-4 .icon",
        animation: "slideInUp",
        animationout: "fadeOut",
        once: true
    },
    {
        selector: ".cm-widget.widget-grid .twocol-left .col-md-5 .icon",
        animation: "slideInLeft",
        animationout: "fadeOut",
        once: true
    },
    {
        selector: ".cm-widget.widget-grid .twocol-right .col-md-5 .icon",
        animation: "slideInRight",
        animationout: "fadeOut",
        once: true
    }
    ],
    navigationTopAvoid: [
        $("section.widget-header-section")
    ],
    navigationBottomAvoid: [
        $("section.widget-footer-section")
    ],
    clickHandler: function (e) {

        e.preventDefault();
        var ready = function (element) {

            var prev = element.prev("[inf-con-state=notloaded]");

            if (prev.length > 0) {
                $(".inf-con-loading").addClass("active").removeClass("bottom");

                prev.hide();

                infinitecontent.loadElement(prev, function (x) {
                    x.show();
                    $(window).scrollTop($(window).scrollTop() + x.prop("scrollHeight"));
                    $(".inf-con-loading").removeClass("active");
                });
            }
        };
        var hash = ('/' + e.target.pathname + '/').replace(/[\/]+/g, '/');
        var partialUrls = e.target.pathname.split("/");
        hash = partialUrls[partialUrls.length - 2];
        $('.inf-con-elem').each(function () {
            if ($(this).attr('id').includes(hash)) {
                hash = $(this).attr('id');
                return false;
            }
        });

        if (hash.length > 2) {
            if (!infinitecontent.isLoaded(hash) && !infinitecontent.isLoading(hash)) {
                infinitecontent.loadElement($("section[id='" + hash + "'].inf-con-elem[inf-con-state=notloaded]"), function (elem) {
                    infinitecontent.block();

                    infinitecontent.scrollToElement(elem, function () {
                        ready(elem);
                        infinitecontent.unblock();
                    });
                });
            } else {
                infinitecontent.block();

                infinitecontent.scrollToElement($("section[id='" + hash + "'].inf-con-elem"), function (elem) {
                    ready(elem);
                    infinitecontent.unblock();
                });
            }
        }
    },
    alignNavigation: function () {
        /*Get Data for Navigation*/
        infinitecontent.cacheddata.navTopOffset = 0;
        infinitecontent.cacheddata.navBottomOffset = 0;

        for (var i = 0; i < infinitecontent.navigationTopAvoid.length; i++) {
            infinitecontent.cacheddata.navTopOffset += infinitecontent.navigationTopAvoid[i].prop("scrollHeight");
        }

        for (var i = 0; i < infinitecontent.navigationBottomAvoid.length; i++) {
            infinitecontent.cacheddata.navBottomOffset += infinitecontent.navigationBottomAvoid[i].prop("scrollHeight");
        }
    }
};

$(document).ready(function () {
    infinitecontent.init();
});


$('.inf-con-nav a').click(function (e) {
    infinitecontent.clickHandler(e);
});
$('a.inf-con-nav-a').click(function (e) {

    infinitecontent.clickHandler(e);
});

/*Fix to get browsers dont jump to last position, what will produce some weird behavior*/
$(window).on('beforeunload', function () {
    $(window).scrollTop(0);
});

$(window).resize(function () {
    infinitecontent.alignNavigation();
});