"use strict";

var infCon3 = {

    OnPartialLoaded: [],
    CallAfterNextLoadShow: [],
    ShowWarningsAndInfos: false,


    ScrollBlocked_ForNavigating: false,

    PointOfView: function () { return $(window).scrollTop() + infCon3.RelPointOfView(); },
    RelPointOfView: function () { return $(window).outerHeight() / 10; },
    LoadMarginTop: function () { return $(window).outerHeight() / 2; },
    LoadMarginBottom: function () { return $(window).outerHeight() + 20; },

    LastItemVisible: function () {
        return this.Partials.Items.length != 0 && this.Partials.Items[this.Partials.Items.length - 1].Visible;
    },
    FirstItemVisible: function () {
        return this.Partials.Items.length != 0 && this.Partials.Items[0].Visible;
    },

    init: function () {
        this.log("infCon3 will show all warnings and infos. You can disable warnings and infos by removing 'infCon3.ShowWarningsAndInfos = true;'", 'info');
        this.Partials.init();
        this.NavigationPartials.init();
        infCon3.Partials.loadArea(infCon3.Partials.CurrentFocusedItem);
        $(window).scroll(infCon3.onScroll);
    },


    onScroll: function () {
        if (infCon3.ScrollBlocked_ForNavigating) {
            return;
        }
        infCon3.Partials.scrollUpdate();
        if (infCon3.Partials.FocusChanged) {
            if (history.pushState) {
                history.pushState(infCon3.Partials.CurrentFocusedItem.Id, null, infCon3.Partials.CurrentFocusedItem.Url);
            } else {
                location.href = infCon3.Partials.CurrentFocusedItem.Url;
            }
        }
        infCon3.NavigationPartials.scrollUpdate();
    },

    Partials: {
        // sorted from top to bottom
        Items: [],

        TopLoadBar: null,
        BottomLoadBar: null,

        CurrentFocusedItem: null,
        FocusChanged: false,

        init: function () {
            $(".infCon3-partial").each(function () {
                var partial = null;
                if (!(partial = infCon3.Partials.add($(this)))) {
                    infCon3.log("Not initiated -> " + partial.toString());
                }
            });
            this.TopLoadBar = $(".infCon3-loadbar-top").first();
            this.BottomLoadBar = $(".infCon3-loadbar-bottom").first();
            if (this.TopLoadBar.length <= 0) {
                this.TopLoadBar == null;
            } else {
                this.TopLoadBar.hide();
            }
            if (this.BottomLoadBar.length <= 0) {
                this.BottomLoadBar == null;
            } else {
                this.BottomLoadBar.hide();
            }
        },


        GetLoadedItemAtPosition: function (position) {
            var result = null;
            $.each(this.Items, function () {
                if (!this.Loaded || !this.Visible) {
                    return true;
                }
                if (result == null) {
                    result = this;
                }
                if (this.Element.offset().top + this.Element.outerHeight() > position) {
                    result = this;
                    return false;
                }
            });
            return result;
        },

        scrollUpdate: function () {
            infCon3.CustomAnimations.animateNewVisibleElements(this.Items);
            var item = this.GetLoadedItemAtPosition(infCon3.PointOfView());
            var itemId = this.Items.indexOf(item);
            this.loadNextIfVisible(itemId);
            this.loadPrevIfVisible(itemId);
            this.addClassActiveIfVisible();

            if (this.CurrentFocusedItem != null && (item == null || this.CurrentFocusedItem.Id == item.Id)) {
                this.FocusChanged = false;
                return;
            }
            this.CurrentFocusedItem = item;
            this.FocusChanged = true;
        },

        addClassActiveIfVisible() {
            if (this.Items == null) {
                return;
            }
            var className = 'infCon3-displayed';
            this.Items.forEach(function (item, id) {
                var element = item.Element;
                if (element.hasClass(className)) {
                    element.removeClass(className);
                }
                if (element.offset().top + element.outerHeight() > $(window).scrollTop() && element.offset().top < $(window).scrollTop() + $(window).outerHeight()) {
                    element.addClass(className);
                }
            });

        },

        loadNextIfVisible: function (itemId) {
            var loadMarginBottom = infCon3.LoadMarginBottom();
            var loadMarginTop = infCon3.LoadMarginTop();
            var pov = infCon3.PointOfView();
            var item = this.Items[itemId];
            if (item == undefined) {
                return;
            }
            var loadedRange = item.Element.offset().top + item.Element.outerHeight();
            while (itemId < this.Items.length - 1) {
                itemId++;
                item = this.Items[itemId];
                if (item.StateBlocked) {
                    return;
                }
                if (item.Loaded && item.Visible) {
                    loadedRange = item.Element.offset().top + item.Element.outerHeight();
                    continue;
                }
                if (loadedRange > pov + loadMarginBottom) {
                    return;
                }
                if (!item.Loaded && infCon3.Partials.BottomLoadBar != null) {
                    infCon3.Partials.BottomLoadBar.show();
                }

                item.show(function () {
                    if (infCon3.Partials.BottomLoadBar != null) {
                        infCon3.Partials.BottomLoadBar.hide();
                    }
                    var height = item.Element.outerHeight();
                    item.Element.height(0).animate({ height: height }, 500, function () {
                        item.Element.removeAttr("style height");
                        infCon3.Partials.loadNextIfVisible(itemId);
                    });
                });
                break;
            }
        },
        loadPrevIfVisible: function (itemId) {
            var loadMarginBottom = infCon3.LoadMarginBottom();
            var loadMarginTop = infCon3.LoadMarginTop();
            var pov = infCon3.PointOfView();
            var item = this.Items[itemId];
            if (item == undefined) {
                return;
            }
            var loadedRange = item.Element.offset().top;
            while (itemId > 0) {
                itemId--;
                item = this.Items[itemId];
                if (item.StateBlocked) {
                    return;
                }
                if (item.Loaded && item.Visible) {
                    loadedRange = item.Element.offset().top;
                    continue;
                }
                if (loadedRange < pov - loadMarginTop) {
                    return;
                }

                if (!item.Loaded && infCon3.Partials.TopLoadBar != null) {
                    infCon3.Partials.TopLoadBar.show();
                }

                item.show(function () {
                    if (infCon3.Partials.TopLoadBar != null) {
                        infCon3.Partials.TopLoadBar.hide();
                    }
                    infCon3.Partials.loadPrevIfVisible(itemId);
                });
                break;
            }
        },

        // _callBack(top of focused partial)
        loadArea: function (focusPartial, _callBack) {
            var loadMarginBottom = infCon3.LoadMarginBottom();
            var loadMarginTop = infCon3.LoadMarginTop();
            if (focusPartial == null) {
                infCon3.log("Can't load Partial: Partial is 'null'");
            }
            this.hideNotDisplayedPartials();

            var itemIndex = this.Items.indexOf(focusPartial);
            var items = this.Items;
            loadAreaDown(items, itemIndex, loadMarginBottom, function (marginBottom) {
                marginBottom = Math.max(0, marginBottom);
                loadAreaUp(items, itemIndex - 1, loadMarginTop + marginBottom, function (marginTop) {
                    if (_callBack != null) {
                        _callBack(items[itemIndex].Element.offset().top);
                    }
                });

            });

            function loadAreaDown(items, itemId, marginBottom, _callBack) {
                if (marginBottom <= 0 || items.length <= itemId) {
                    _callBack(marginBottom);
                    return;
                }
                var item = items[itemId];
                item.show(function () {
                    var itemHeight = item.Element.outerHeight();
                    loadAreaDown(items, itemId + 1, marginBottom - itemHeight, _callBack);
                });
            }
            function loadAreaUp(items, itemId, marginTop, _callBack) {
                if (marginTop <= 0 || itemId < 0) {
                    _callBack(marginTop);
                    return;
                }
                var item = items[itemId];
                item.show(function () {
                    var itemHeight = item.Element.outerHeight();
                    loadAreaUp(items, itemId - 1, marginTop - itemHeight, _callBack);
                });
            }
        },


        hideNotDisplayedPartials: function () {
            $.each(this.Items, function () {
                if (this.Element.offset().top + this.Element.outerHeight() <= $(window).scrollTop() - infCon3.LoadMarginTop() ||
                    this.Element.offset().top > infCon3.PointOfView() + infCon3.LoadMarginBottom()) {
                    this.hide();
                }
            });
        },

        add: function (elem) {
            var newItem = {
                Element: elem,
                Loaded: false,
                StateBlocked: false,
                Id: null,
                Meta: null,
                Src: null,
                Url: null,
                Visible: false,
                NavPartial: null,


                toString: function () {
                    return "Partial, " + (this.Id || "unknown") + ": " + (this.Url || "noUrl");
                },
                getAttrFromElement: function (name) {
                    if (typeof this.Element.attr(name) != typeof undefined && this.Element.attr(name) != false) {
                        return this.Element.attr(name);
                    } else {
                        return null;
                    }
                },
                isValide: function () {
                    return this.Url == null ||
                            this.Id == null ||
                            (this.Src == null && !this.Loaded);
                },
                //_callback(partial, success)
                loadMe: function (_callback) {
                    if (this.StateBlocked == true) {
                        return;
                    }
                    this.StateBlocked = true;
                    var partial = this;
                    if (this.Loaded) {
                        partial.StateBlocked = false;
                        if (_callback != null) {
                            _callback(partial, false);
                        }
                        return;
                    }
                    this.Element.load(this.Src, null, function (e, status) {
                        partial.Loaded = status == "success";
                        if (!partial.Loaded) {
                            partial.StateBlocked = false;
                            if (_callback != null) {
                                _callback(partial, false);
                            }
                            return;
                        }
                        partial.Element.waitForImages(function () {
                            infCon3.CustomAnimations.hidePreHiddenElementsAnimatedElements(partial);
                            partial.StateBlocked = false;
                            if (_callback != null) {
                                _callback(partial, true);
                            }
                        });
                    });
                },

                hide: function () {
                    var wasVisible = this.Element.is(":visible");
                    var prevPosition = $(window).scrollTop();
                    var prevSpaceToBottom = $(document).outerHeight() - prevPosition;
                    var elementTop = this.Element.offset().top;
                    var elementHeight = this.Element.outerHeight();
                    var heightToCorrect = Math.min(elementHeight, prevSpaceToBottom);


                    this.Element.hide();
                    this.Visible = false;
                    if (elementTop + elementHeight < infCon3.PointOfView()) {
                        $(window).scrollTop(prevPosition - heightToCorrect);
                    }
                },

                show: function (_callback) {
                    var elem = this;
                    if (!this.Loaded) {
                        var _cb = _callback;
                        this.loadMe(function (partial, success) {
                            if (!success) {
                                infCon3.log("Not loaded -> " + partial.toString());
                                return;
                            }
                            infCon3.NavigationPartials.addAllNavigationItems(partial.Element.find(".infCon3-nav"));
                            infCon3.NavigationPartials.addAllDeprecatedNavigationItems(partial.Element.find(".inf-con-nav-a"));
                            $.each(infCon3.OnPartialLoaded, function () {
                                this(partial.Element, success);
                            });
                            partial.show(_cb);
                        });
                        return;
                    }
                    var prevPosition = $(window).scrollTop();
                    var wasHidden = !this.Visible && this.Element.is(":hidden");
                    this.Visible = true;
                    this.Element.show();
                    var elementTop = this.Element.offset().top;
                    var currentTop = infCon3.Partials.GetLoadedItemAtPosition(infCon3.PointOfView()).Element.offset().top;
                    if (elementTop <= currentTop && wasHidden) {
                        $(window).scrollTop(prevPosition + this.Element.outerHeight());
                    }
                    $.each(infCon3.CallAfterNextLoadShow, function () {
                        this();
                    });
                    infCon3.CallAfterNextLoadShow = [];
                    if (_callback != null) {
                        _callback();
                    }
                },
            }

            var state = newItem.getAttrFromElement("infCon3-state");
            newItem.Loaded = state == null || state == "loaded";
            newItem.Id = newItem.getAttrFromElement("infCon3-Id");
            newItem.Meta = newItem.getAttrFromElement("infCon3-Meta");
            newItem.Src = newItem.getAttrFromElement("infCon3-Src");
            newItem.Url = newItem.getAttrFromElement("id");
            newItem.Visible = newItem.Loaded;
            if (!newItem.Visible) {
                newItem.Element.hide();
            }
            if (newItem.Visible && this.CurrentFocusedItem == null) {
                this.CurrentFocusedItem = newItem;
            }

            if (newItem.isValide) {
                this.Items.push(newItem);
            }
            return newItem;
        }
    },


    NavigationPartials: {
        Items: [],

        ActiveNavPoint: null,


        init: function () {
            this.addAllNavigationItems($(".infCon3-nav"));
            this.addAllDeprecatedNavigationItems($(".inf-con-nav-a"));

            $(window).on("popstate", function (e) {
                if (e.originalEvent.state == null) {
                    return;
                }
                var target = null;
                $.each(infCon3.Partials.Items, function () {
                    if (this.Id == e.originalEvent.state) {
                        target = this;
                        return false;
                    }
                });
                if (target == null) {
                    return;
                }
                infCon3.NavigationPartials.navigateTo(target);
            });
        },

        scrollUpdate: function () {
            this.setActiveNavPoint();
        },

        setActiveNavPoint: function () {
            var items = infCon3.Partials.Items;
            var focusedItemIndex = items.indexOf(infCon3.Partials.CurrentFocusedItem);
            var resultNavPoint = null;

            for (var k = focusedItemIndex; k >= 0; k--) {
                var partial = items[k];
                if (partial.NavPartial != null) {
                    resultNavPoint = partial.NavPartial;
                    break;
                }
            }
            if (resultNavPoint == null) {
                return;
            }
            this.ActiveNavPoint = resultNavPoint.setActive(this.ActiveNavPoint);
        },

        // this scope => nav-element
        clickHandler: function (e) {
            e.preventDefault();
            var id = $(this).attr('infCon3-id');
            var target = null;
            $.each(infCon3.NavigationPartials.Items, function () {
                if (id == this.Id) {
                    target = this.Partial;
                }
            });
            if (target == null) {
                return;
            }
            infCon3.NavigationPartials.navigateTo(target);
        },

        navigateTo: function (partial) {
            infCon3.ScrollBlocked_ForNavigating = true;
            infCon3.Partials.loadArea(partial, function (newPosition) {

                var body = $("html, body");
                body.stop().animate({
                    scrollTop: newPosition
                }, "slow").promise().done(function () {
                    infCon3.Partials.hideNotDisplayedPartials();
                    infCon3.ScrollBlocked_ForNavigating = false;
                    infCon3.onScroll();
                });
            });
        },

        addAllDeprecatedNavigationItems: function (navElems) {
            navElems.each(function () {
                infCon3.log("'inf-con-nav-a' for url '" + $(this).attr('href') + "'  is deprecatded, please use infCon3-nav and infCon3-id", 'warning');

                var navElem = $(this);
                var target = null;
                $.each(infCon3.Partials.Items, function () {
                    if (this.Url == navElem.attr('href')) {
                        target = this;
                    }
                });
                if (target == null) {
                    var navUrl = navElem.attr('href');
                    if (navUrl == "") {
                        infCon3.log("could not convert element with class='inf-con-nav-a' and empty url");
                    } else {
                        infCon3.log("could not convert element with class='inf-con-nav-a' and url='" + navElem.attr('href') + "'");
                    }
                    return true;
                }
                infCon3.log("'inf-con-nav-a' for url '" + $(this).attr('href') + "' was converted successfully", 'info');
                navElem.attr("infCon3-id", target.Id);
                navElem.addClass("infCon3-nav");
                navElem.removeClass("inf-con-nav-a");
            })
            this.addAllNavigationItems(navElems.filter(".infCon3-nav"));
        },

        addAllNavigationItems: function (navElems) {
            navElems.each(function () {
                var navPartial = null;
                if (!(navPartial = infCon3.NavigationPartials.add($(this)))) {
                    infCon3.log("Not initiated -> " + navPartial.toString());
                }
                $(this).click(infCon3.NavigationPartials.clickHandler);
            });
        },

        add: function (elem) {
            var newItem = {
                Element: elem,
                Id: null,
                Url: null,
                Partial: null,
                // AoN-Elemente, sind Elemente, welche class='infCon3-active' erhalten, falls Navigationspunkt aktiv ist.
                AoNElements: [],

                toString: function () {
                    return "NavigationPartial, " + (this.Id || "unknown") + ": " + (this.Url || "noUrl");
                },
                getAttrFromElement: function (name) {
                    if (typeof this.Element.attr(name) != typeof undefined && this.Element.attr(name) != false) {
                        return this.Element.attr(name);
                    } else {
                        return null;
                    }
                },
                isValide: function () {
                    return (newItem.Url == null ||
                            newItem.Id == null ||
                            newItem.Partial == null);
                },
                setActive: function (currentActive) {
                    if (currentActive != null) {
                        $.each(currentActive.AoNElements, function () {
                            this.removeClass("infCon3-active");
                        });
                    }
                    $.each(this.AoNElements, function () {
                        this.addClass("infCon3-active");
                    });
                    return this;
                }
            };
            var id = newItem.getAttrFromElement("infCon3-Id");
            newItem.Id = id;
            newItem.Url = newItem.getAttrFromElement("href");
            var isMainNav = newItem.getAttrFromElement("infCon3-nav-type") == "main";

            var p = null;
            $.each(infCon3.Partials.Items, function () {
                if (id == this.Id) {
                    p = this;
                    return false;
                }
            });
            newItem.Partial = p;

            if (isMainNav) {
                if (newItem.Partial != null || newItem.Partial != undefined) {
                    newItem.Partial.NavPartial = newItem;
                }

                $(".infCon3-nav-AoN").each(function () {
                    var aonId = $(this).attr("infCon3-id");
                    if (newItem.Id == aonId) {
                        newItem.AoNElements.push($(this));
                    }
                });
            }

            if (newItem.isValide) {
                this.Items.push(newItem);
            }
            return newItem;
        }
    },




    CustomAnimations: {
        Animations: [],

        // Einschränkung des animierten Sichtbereichs. Fensterhöhe - 2*offset => animierter Bereich.
        Offset: 2,

        animateNewVisibleElements: function (items) {
            $.each(items, function () {
                if (!this.Loaded || !this.Visible) {
                    return true;
                }
                var item = this;
                $.each(infCon3.CustomAnimations.Animations, function () {
                    var elements = item.Element.find(this.selector);
                    var animation = this;
                    elements.each(function () {
                        if ($(this).offset().top > $(window).scrollTop() + infCon3.CustomAnimations.Offset
                            && $(this).offset().top + $(this).outerHeight() < $(window).scrollTop() + $(window).outerHeight() - infCon3.CustomAnimations.Offset) {
                            infCon3.CustomAnimations.animate($(this), animation.animations);
                        }
                    });
                });
            });
        },


        animate: function (element, animations) {
            var animatedStr = element.attr('infCon3-animated');
            var notAnimated = animatedStr == undefined || animatedStr == null || animatedStr == "false";
            if (!notAnimated) {
                return;
            }
            element.attr("infCon3-animated", "true");
            element.css('visibility', 'visible');

            var centerX = element.offset().left + element.outerWidth() / 2;
            var centerY = element.offset().top + element.outerHeight() / 2;
            var left = centerX <= $(window).outerWidth() / 2;
            var top = centerY <= $(window).scrollTop() + $(window).outerHeight() / 2;

            _animate(element, animations, left, top, 0);

            function _animate(element, animations, left, top, index) {
                if (index >= animations.length) {
                    return;
                }
                var animation = animations[index];
                var dontSkipAni = true;
                switch (animation.dir) {
                    case "left":
                        dontSkipAni = left;
                        break;
                    case "right":
                        dontSkipAni = !left;
                        break;
                    case "top":
                        dontSkipAni = top;
                        break;
                    case "bottom":
                        dontSkipAni = !top;
                        break;
                    case "all":
                        dontSkipAni = true;
                        break;
                    default:
                        dontSkipAni = false;
                }

                if (!dontSkipAni) {
                    _animate(element, animations, left, top, index + 1);
                    return;
                }
                var oneEndStr = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
                element.addClass('animated ' + animation.ani.trim()).bind(oneEndStr, function () {
                    element.removeClass('animated');
                    element.removeClass(animation.ani.trim());

                    _animate(element, animations, left, top, index + 1);
                });
            }
        },

        // Soll alle animierten Elemente verstecken, welche durch zB fadeIn erst sichtbar werden.
        hidePreHiddenElementsAnimatedElements: function (partial) {
            this.getAllAnimatedElements(partial, function (e) {
                return e.preHidden;
            }).css('visibility', 'hidden');
        },


        getAllAnimatedElements: function (partial, condition) {
            var selectors = [];
            $.each(this.Animations, function () {
                if (condition == null || condition(this)) {
                    selectors.push(this.selector);
                }
            });
            return partial.Element.find(selectors.join(', '));
        },
    },


    log: function (value, logType) {
        if (logType == undefined || logType == null) {
            logType = "error";
        }

        var warning = false;
        var error = false;
        var info = false;

        switch (logType) {
            case 'warning':
                warning = true;
                break;
            case 'error':
                error = true;
                logType = logType + '  ';
                break;
            case 'info':
                info = true;
                logType = logType + '   ';
                break;
            default:
                error = true;
                logType = 'error';
                logType = logType + '  ';
        }

        if ((warning || info) && !infCon3.ShowWarningsAndInfos) {
            return;
        }

        if (window.console) {
            console.log(logType + ":\t infCon3 | " + value);
        }
    }

};