$(function () {
    //datepicker
    $(".datepickerForms").datepicker({
        dateFormat: "dd.mm.yy",
        dayNames: ["Sonntag", "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag", "Samstag"],
        monthNames: ["Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember"],
        monthNamesShort: ["Jan", "Feb", "Mar", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez"],
        dayNamesMin: ["So", "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So"],
        firstDay: 1, // Set the first day of the week: Sunday is 0, Monday is 1, etc.
        nextText: "Nächster Monat",
        prevText: "Letzter Monat",
        numberOfMonths: [1, 3],
        showCurrentAtPos: 1,
        showWeek: true,
        weekHeader: "W",
        gotoCurrent: true,
        showOtherMonths: true,
        selectOtherMonths: true,
        showButtonPanel: true
    });

    function getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min)) + min;
    }

    function setUrlToInput(path, url) {
        var input = $(path).parent().parent().find("input");
        input.val(url);
    }

    function setImage(path, url) {
        var img = $(path).parent().parent().find("img");
        img.attr("src", url);
    }

    $('.btn-employee-form-remove').on('click', function (evt) {
        setUrlToInput($(this), "");
        setImage($(this), "/App_Themes/admin/img/default.png");
    });

    //elfider qrcode + profilpic
    $('.btn-employee-form').on('click', function (evt) {
        evt.preventDefault();
        var path = $(this);
        var inst = getRandomInt(1, 100);
        $('<div id="el_' + inst + '" />').dialogelfinder({
            url: '/file',
            lang: 'de',
            rememberLastDir: false,
            getFileCallback: function (file) {
                var imgpath = file.url.replace("Content", gwc.sitesettingpath);
                $('#el_' + inst).dialogelfinder('close');
                url = imgpath.replace(gwc.serverpath, '')
                setUrlToInput(path, url);
                setImage(path, url);
            }
        });
    });

    //Custem Rule
    jQuery.validator.addMethod("alphanumeric", function(value, element) {
        return this.optional(element) || /^[a-zA-Z0-9]+$/.test(value);
    }); 

    //formvalidation for /admin/sitesettings
    $("#SiteSettingEdit").validate({
        ignore: "",
        invalidHandler: function (event, validator) {
            var errors = validator.numberOfInvalids();
            if (errors) {
                var message = errors == 1
                  ? 'You missed 1 field. It has been highlighted'
                  : 'You missed ' + errors + ' fields. They have been highlighted';
                $("div.error span").html(message);
                $("div.error").show();
                $("div.error").removeClass("hidden");
            } else {
                $("div.error").hide();
            }
        },
        rules: {
            Name: "required",
            MainDomain: "required",
            Bindings: "required",
            Template: "required"
        },

        messages: {
            Name: "Dieses Feld ist ein Pflichtfeld",
            MainDomain: "Dieses Feld ist ein Pflichtfeld",
            Bindings: "Dieses Feld ist ein Pflichtfeld",
            Template: "Bitte wähle ein Template aus"
        },

        submitHandler: function (form) {
            form.submit();
        }
    });

    //formvalidation for /admin/useredit/
    $("#UserEdit").validate({
        ignore: "",
        invalidHandler: function(event, validator) {
            var errors = validator.numberOfInvalids();
            if (errors) {
                var message = errors == 1
                  ? 'You missed 1 field. It has been highlighted'
                  : 'You missed ' + errors + ' fields. They have been highlighted';
                $("div.error span").html(message);
                $("div.error").show();
                $("div.error").removeClass("hidden");
            } else {
                $("div.error").hide();
            }
        },
        rules: {
            username: "required",
            firstname: "required",
            lastname: "required",
            email: "required",
            password: "required"
        },

        messages: {
            username: "Dieses Feld ist ein Pflichtfeld",
            firstname: "Dieses Feld ist ein Pflichtfeld",
            lastname: "Dieses Feld ist ein Pflichtfeld",
            email: "Dieses Feld ist ein Pflichtfeld",
            password: "Dieses Feld ist ein Pflichtfeld"
        },

        submitHandler: function (form) {
            form.submit();
        }
    });

    //formvalidation for /admin/employeeedit/
    $("#EmployeeEdit").validate({
        rules: {
            FirstName: "required",
            LastName: "required",
            Email: "required",
            EmailAlias: "required",
            Telephone: "required",
            ProfilImagePath: "required",
            QrCode: "required"
        },

        messages: {
            FirstName: "Dieses Feld ist ein Pflichtfeld",
            LastName: "Dieses Feld ist ein Pflichtfeld",
            Email: "Dieses Feld ist ein Pflichtfeld",
            EmailAlias: "Dieses Feld ist ein Pflichtfeld",
            Telephone: "Dieses Feld ist ein Pflichtfeld",
            ProfilImagePath: "Dieses Feld ist ein Pflichtfeld",
            QrCode: "Dieses Feld ist ein Pflichtfeld"
        },

        submitHandler: function (form) {
            form.submit();
        }
    });

    $("#addWidgetForm").validate({
        rules: {
            name: "required",
            widgetType: "required"
        },

        messages: {
            name: "Dieses Feld ist ein Pflichtfeld",
            widgetType: "Please Chose a Type for the widget"
        },

        submitHandler: function (form) {
            form.submit();
        }
    });

    $("#addWidgetCompositeForm").validate({
        rules: {
            name: "required",
            widgetType: "required"
        },

        messages: {
            name: "Dieses Feld ist ein Pflichtfeld",
            widgetType: "Please Chose a Type for the widget"
        },

        submitHandler: function (form) {
            form.submit();
        }
    });

    $("#WidgetEdit").validate({
        ignore: "",
        invalidHandler: function (event, validator) {
            var errors = validator.numberOfInvalids();
            if (errors) {
                var message = errors == 1
                  ? 'You missed 1 field. It has been highlighted'
                  : 'You missed ' + errors + ' fields. They have been highlighted';
                $("div.error span").html(message);
                $("div.error").show();
                $("div.error").removeClass("hidden");
            } else {
                $("div.error").hide();
            }
        },
        rules: {
            name: "required"
        },

        messages: {
            name: "Dieses Feld ist ein Pflichtfeld"
        },

        submitHandler: function (form) {
            form.submit();
        }
    });

    $("#addSiteForm").validate({
        rules: {
            name: "required"
        },

        messages: {
            name: "Dieses Feld ist ein Pflichtfeld"
        },

        submitHandler: function (form) {
            form.submit();
        }
    });

    $("#SiteEdit").validate({
        ignore: "",
        invalidHandler: function (event, validator) {
            var errors = validator.numberOfInvalids();
            if (errors) {
                var message = errors == 1
                  ? 'You missed 1 field. It has been highlighted'
                  : 'You missed ' + errors + ' fields. They have been highlighted';
                $("div.error span").html(message);
                $("div.error").show();
                $("div.error").removeClass("hidden");
            } else {
                $("div.error").hide();
            }
        },
        rules: {
            name: "required"
        },

        messages: {
            name: "Dieses Feld ist ein Pflichtfeld"
        },

        submitHandler: function (form) {
            form.submit();
        }
    });

    $("#createBackupForm").validate({
        rules: {
            name: {
                required: true,
                alphanumeric: true
            }
        },

        messages: {
            name: 
                {
                    required: "Dieses Feld ist ein Pflichtfeld",
                    alphanumeric: "Name darf nur aus Zahlen und Zeichen a-zA-Z bestehen"
            }
        },

        submitHandler: function (form) {
            form.submit();
        }
    });

});