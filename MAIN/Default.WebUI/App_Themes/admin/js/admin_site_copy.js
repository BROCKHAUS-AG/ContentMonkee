(function (factory) {
    /* global define */
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['jquery'], factory);
    } else if (typeof module === 'object' && module.exports) {
        // Node/CommonJS
        module.exports = factory(require('jquery'));
    } else {
        // Browser globals
        factory(window.jQuery);
    }
}(function ($) {
    $.extend($.summernote.options, {
        template: {
            list: []
        }
    });

    $.extend($.summernote.plugins, {
        'template': function (context) {
            var ui = $.summernote.ui;
            var options = context.options.template;

            context.memo('button.template', function () {
                var button = ui.buttonGroup([
                  ui.button({
                      className: 'dropdown-toggle',
                      contents: '<span class="template"/> Bootstrap <span class="caret"></span>',
                      data: {
                          toggle: 'dropdown'
                      }
                  }),
                  ui.dropdown({
                      className: 'dropdown-template',
                      items: options.list,
                      click: function (event) {
                          var $button = $(event.target);
                          var value = $button.data('value');
                          var path = options.path + '/' + value + '.html';

                          $.get(path)
                              .done(function (data) {
                                  var node = document.createElement('span');
                                  node.innerHTML = data;
                                  context.invoke('editor.insertNode', node);
                              }).fail(function () {
                                  alert('template not found in ' + path);
                              });
                      }
                  })
                ]);

                // create jQuery object from button instance.
                return button.render();
            });
        },
        'pickInternalLink': function (context) {
          var self = this;
          var ui = $.summernote.ui;

          context.memo('button.pickInternalLink', function () {
            var button = ui.button({
              contents: '<i class="fa fa-link"/>',
              tooltip: 'Einen internen Link auswählen',
              click: function (event, editor, layoutInfo, value) {
                  $("#internalSiteSelection").modal("show");
                  $(".note-link-text").keyup(function () {
                      if ($('.note-link-text').val().length > 0) {
                          $('.note-link-btn').removeClass("disabled");
                          $('.note-link-btn').removeAttr("disabled");
                      }
                      else {
                          $('.note-link-btn').addClass("disabled");
                          $('.note-link-btn').attr("disabled", "disabled");
                      }
                  });

                  $('.note-link-btn').on("click", function () {
                      var noteLinkText = $(".note-link-text").val();
                      var noteLinkUrl = $(".note-link-url").val();
                      var noteLinkNewWindow = $(".note-link-newwindow").is(":checked");
                      console.log(noteLinkNewWindow);

                      $(".summernote").summernote("createLink", {
                          text: noteLinkText,
                          url: noteLinkUrl,
                          newWindow: noteLinkNewWindow
                      });

                      $("#internalSiteSelection").modal("hide");

                  });
              }
            });

            var $pickInternalLink = button.render();
            return $pickInternalLink;
          });
        },
        'embedObject': function (context) {
            var self = this;
            var ui = $.summernote.ui;

            context.memo('button.embedObject', function () {
                var button = ui.button({
                    contents: '<i class="fa fa-object-group"/>',
                    tooltip: 'Medien einbetten',
                    click: function (event, editor, layoutInfo, value) {
                        var $editable = layoutInfo.editable(),
                        dialog = $('<div />').dialogelfinder({
                            url: '/file',
                            lang: 'de',
                            getFileCallback: function (file) {
                                $(dialog).dialogelfinder('close');

                                var imageTypes = Array('.png', '.jpg', '.jpeg', '.gif', '.tiff', '.svg');
                                $.each(imageTypes, function (index, value) {
                                    if (file.url.includes(value)) {
                                        editor.insertImage($editable, file.url.replace(gwc.serverpath, ''), file.url.replace(gwc.serverpath, ''));
                                        return;
                                    }
                                });

                                var linkNode = document.createElement('a');
                                $(linkNode).attr('href', file.url.replace(gwc.serverpath, '')).text(file.name + ' ');
                                editor.insertNode($editable, linkNode);
                            }
                        });
                    }
                });

                var $embedObject = button.render();
                return $embedObject;
            });
        }
    });
}));
$(function () {


    $(function () {
        $('[data-toggle="popover"]').popover();
    });

    $(function () {
        $(".tablesorter").tablesorter();
    });

    $('.btn-product-selector').on('click', function (evt) {
        evt.preventDefault();
        var self = $(evt.target),
            modalId = self.data('modal-id'),
            targetId = self.data('select-target'),
            modal = $('#' + modalId);
        if (modalId !== "undefined") {
            modal.find('input[name=target]').val(targetId);
            var btnSelect = modal.find('.btn-select'),
                btnAbort = modal.find('.btn-abort');
            btnSelect.on('click', function (evt) {
                evt.preventDefault();
                var elementTable = modal.find('table tbody'),
                    checkedElements = elementTable.find('input:checked'),
                    data = [];
                if (checkedElements.length > 0) {
                    if (checkedElements.length === 1) {
                        var elementId = $(checkedElements.eq(0)).val();
                        $(targetId).val(elementId);
                    } else {
                        $.each(checkedElements, function (index) {
                            var elementId = $(this).val();
                            data.push(elementId);
                        });
                        $(targetId).val(data.join());
                        modal.modal('hide');
                    }
                }
            });

            btnAbort.on('click', function (evt) {
                evt.preventDefault();
                modal.modal('hide');
            });

            modal.modal();
        }
    });


    // template, editor
    //var tmpl = $.summernote.renderer.getTemplate();

    //$.summernote.addPlugin({
    //    name: 'embedResource',
    //    buttons: {
    //        embedObject: function () {
    //            return tmpl.iconButton('fa fa-object-group', {
    //                event: 'embedObject',
    //                title: 'Medien einbetten',
    //                hide: false
    //            });
    //        }
    //    },
    //    events: {
    //        embedObject: function (event, editor, layoutInfo, value) {
    //            var $editable = layoutInfo.editable(),
    //                dialog = $('<div />').dialogelfinder({
    //                    url: '/file',
    //                    lang: 'de',
    //                    getFileCallback: function (file) {
    //                        $(dialog).dialogelfinder('close');

    //                        var imageTypes = Array('.png', '.jpg', '.jpeg', '.gif', '.tiff', '.svg');
    //                        $.each(imageTypes, function (index, value) {
    //                            if (file.url.includes(value)) {
    //                                editor.insertImage($editable, file.url.replace(gwc.serverpath, ''), file.url.replace(gwc.serverpath, ''));
    //                                return;
    //                            }
    //                        });

    //                        var linkNode = document.createElement('a');
    //                        $(linkNode).attr('href', file.url.replace(gwc.serverpath, '')).text(file.name + ' ');
    //                        editor.insertNode($editable, linkNode);
    //                    }
    //                });
    //        }
    //    }
    //});

    $('.summernote').summernote({
        height: 300, // set editor height
        minHeight: null, // set minimum height of editor
        maxHeight: null, // set maximum height of editor
        lang: 'de-DE', // default: 'en-US'
        focus: true, // set focus to editable area after initializing summernote
        toolbar: [
            ['style', ['style']],
            ['font', ['bold', 'italic', 'underline', 'clear']],
            ['fontname', ['fontname']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['height', ['height']],
            ['table', ['table']],
            ['insert', ['link', 'pickInternalLink', 'embedObject', 'hr']],
            ['view', ['fullscreen', 'codeview']],
            ['help', ['help']],
            ['insert', ['template']],
        ],
        template: {
            path: '/App_Themes/admin/lib/summernote/tpls', // path to your template folder
            list: [
                '.container',
                '.row',
                '.col-2',
                '.col-3',
                '.col-4',
                '.col-6',
                'TwoColumn',
                'ThreeColumn',
                'FourColumn',
            ]
        },
    });

    // Medienseite
    $('#elfinder').elfinder({
        lang: 'de', // language (OPTIONAL)
        url: '/file', // connector URL (REQUIRED)
        rememberLastDir: false,
        docked: true,
        // treeDeep: 3,  
        height: 500,
        //   debug: true
    }).elfinder('instance');



    //$(function () {
    //    var myCommands = elFinder.prototype._options.commands;
    //    var disabled = ['extract', 'archive', 'help', 'select'];
    //    $.each(disabled, function (i, cmd) {
    //        (idx = $.inArray(cmd, myCommands)) !== -1 && myCommands.splice(idx, 1);
    //    });
    //    var selectedFile = null;
    //    var options = {
    //        url: 'file',
    //        commands: myCommands,
    //        lang: 'en',
    //        uiOptions: {
    //            toolbar: [
    //                ['back', 'forward'],
    //                ['reload'],
    //                ['home', 'up'],
    //                ['mkdir', 'mkfile', 'upload'],
    //                ['open', 'download'],
    //                ['info'],
    //                ['quicklook'],
    //                ['copy', 'cut', 'paste'],
    //                ['rm'],
    //                ['duplicate', 'rename', 'edit', 'resize'],
    //                ['view', 'sort']
    //            ]
    //        },
    //        handlers: {
    //            select: function (event, elfinderInstance) {

    //                if (event.data.selected.length == 1) {
    //                    var item = $('#' + event.data.selected[0]);
    //                    if (!item.hasClass('directory')) {
    //                        selectedFile = event.data.selected[0];
    //                        $('#elfinder-selectFile').show();
    //                        return;
    //                    }
    //                }
    //                $('#elfinder-selectFile').hide();
    //                selectedFile = null;
    //            }
    //        }
    //    };
    //    $('#elfinder').elfinder(options).elfinder('instance');

    //    $('.elfinder-toolbar:first').append('<div class="ui-widget-content ui-corner-all elfinder-buttonset" id="elfinder-selectFile" style="display:none; float:right;">' +
    //    '<div class="ui-state-default elfinder-button" title="Select" style="width: 100px;"></div>');
    //    $('#elfinder-selectFile').click(function () {
    //        if (selectedFile != null)
    //            $.post('files/selectFile', { target: selectedFile }, function (response) {
    //                alert(response);
    //            });

    //    });
    //});




    function setFormField() {
        var resultIds = [];
        $(".product-items tr td").each(function (index) {
            var productId = $(this).data('item-id');
            resultIds.push(productId);
        });
        var resultString = resultIds.join();
        $('.filepicker-result').val(resultString);
    }

    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
              .toString(16)
              .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
          s4() + '-' + s4() + s4() + s4();
    }

    /* BACKEND Slider Start */

    function getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min)) + min;
    }

    function setFormFieldSliderItems() {
        var resultItems = [];
        $(".slider-items tr").each(function () {
            var self = $(this),
                id = self.data('item-id'),
                hash = self.data('item-hash'),
                url = self.data('item-url'),
                itemString = id + ';' + hash + ';' + url;

            resultItems.push(itemString);
        });
        var resultString = resultItems.join();
        $('#slider-items').val(resultString);
    }

    function bind() {
        var sliderItems = $('.slider-items tr');
        sliderItems.each(function (index, e) {
            var self = $(e),
                btnSetLink = self.find('a.btn-enter-link'),
                hrefLink = self.find('a.item-link');

            btnSetLink.on('click', function (evt) {
                evt.preventDefault();
                var link = prompt("Bitte geben Sie einen validen Weblink ein.");
                if (link !== null) {
                    hrefLink.val('href', link);
                    hrefLink.text(link);
                    self.data('item-url', link);
                }
            });

        });
    }

    $('.btn-delete-selected').on('click', function (evt) {
        evt.preventDefault();

        $(".slider-items tr td input:checked").each(function () {
            $(this).parent().parent().remove();
        });

        setFormFieldSliderItems();
    });

    $('.btn-add-image').on('click', function (evt) {
        evt.preventDefault();
        var inst = getRandomInt(1, 100);
        $('<div id="el_' + inst + '" />').dialogelfinder({
            url: '/file',
            lang: 'de',
            getFileCallback: function (file) {
                $('#el_' + inst).dialogelfinder('close');
                var newId = guid(),
                    url = file.url.replace(gwc.serverpath, ''),
                    link = prompt("Bitte geben Sie eine Url ein."),
                    itemTpl = '<tr data-item-id="' + newId + '" data-image-selecte="false" data-item-hash="' + url + '" data-item-url="' + link + '">' +
                                '   <td>' +
                                '       <input type="checkbox" name="marked-item" data-item-id="' + newId + '">' +
                                '       <img style="max-height: 150px; max-width: 100%;" src="' + url + '">' +
                                '   </td>' +
                                '   <td><a class="item-link" href="' + link + '">' + link + '</a></td>' +
                                '   <td style="text-align: right"><a href="#" class="btn btn-primary btn-enter-link">...</a></td>' +
                                '</tr>';

                $(".slider-items").append(itemTpl);
                setFormFieldSliderItems();
            }
        });
        bind();
    });

    bind();

    /* BACKEND Slider End */

    $('#widgetSelector .btn-select').click(function () {
        var checkedTableElements = $('.item-selection input[type=checkbox]:checked'),
            products = [],
            itemCount = checkedTableElements.length;
        $('.product-items').empty();
        if (itemCount == 0) {
            $('.product-items')
                .append('<tr><td>Keine Produkte ausgewählt.</td></tr>');
        } else {
            checkedTableElements.each(function (index) {
                var productId = $(this).data('item-id'),
                    productName = $(this).data('item-name');
                products.push([productId, productName]);

                $('.product-items')
                    .append("<tr><td data-item-id=\"" + productId + "\">" + productName + "</td></tr>");
            });
        }
        setFormField();
    });

    // Table row sorting functions

    //Helper function to keep table row from collapsing when being sorted
    var fixHelperModified = function (e, tr) {
        var $originals = tr.children();
        var $helper = tr.clone();
        $helper.children().each(function (index) {
            $(this).width($originals.eq(index).width());
        });
        return $helper;
    };

    // Renumber table rows
    function renumber_table(tableID) {
        $(tableID + " tr").each(function () {
            count = $(this).parent().children().index($(this)) + 1;
            $(this).find('.priority').html(count);
        });
    }

    // Make product items sortable
    $('.product-items').sortable({
        helper: fixHelperModified,
        stop: function (event, ui) {
            renumber_table('.product-items');
            setFormField();
        }
    }).disableSelection();

    $('.slider-items').sortable({
        helper: fixHelperModified,
        stop: function (event, ui) {
            setFormFieldSliderItems();
        }
    }).disableSelection();
    //make widget sortable
    $('.widget-items').sortable({
        helper: fixHelperModified,
        stop: function (event, ui) {

            var resultItems = [];
            $(".widget-items tr").each(function () {
                var itemString = $(this).data('item-id') + '';
                resultItems.push(itemString);
            });
            $('#widget-items').val(resultItems.join());

        }
    }).disableSelection();

    //make widget sortable
        $('.site-items').sortable({
        helper: fixHelperModified,
        stop: function (event, ui) {

            var resultItems = [];
            $(".site-items tr").each(function () {
                var itemString = $(this).data('item-id') + '';
                resultItems.push(itemString);
            });
            $('#site-items').val(resultItems.join());
        }
    }).disableSelection();

    //$("#slider").excoloSlider();

    window.codemirrors = [];

    $(".codemirror").each(function (i, e) {
        var editor = CodeMirror.fromTextArea(e, {
            mode: "htmlmixed",
            //value: $(e).val(),
            //,
            //theme: "material",
            viewportMargin: Infinity,
            lineNumbers: true
            //,
            //showCursorWhenSelecting: true
            //styleActiveLine: true,
            //matchBrackets: true
        });
        // on and off handler like in jQuery
        editor.on('change', function (cMirror) {
            // get value right from instance
            e.value = cMirror.getValue();
        });

        codemirrors.push(editor);
    });

    $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        setTimeout(function () {
            jQuery.each(window.codemirrors, function (i) {
                codemirrors[i].refresh();
            });

        }, 100);


    });

    $('.confirm-delete,.delete-confirm').on('click', function (e) {
        e.preventDefault();

        var id = $(this).data('id');
        var widgetid = $(this).data('widgetid');
        var siteid = $(this).data('siteid');

        var title = $(this).data('title');
        var action = $(this).data('action');
        var description = $(this).data('description');

        $('#deleteConfirm input[name=id]').val(id);
        $('#deleteConfirm input[name=siteid]').val(siteid);
        $('#deleteConfirm input[name=widgetid]').val(widgetid);
        $('#deleteConfirm .modal-header h3').html(title);
        $('#deleteConfirm .modal-body form p').html(description);
        $('#deleteConfirm .modal-body form').attr('action', action);

        $('#deleteConfirm').modal("show");
    });

    /* ElFinder based form image selector */
    $('.img-selector-container').each(function (i, e) {
        var parent = $(e),
            previewImage = parent.find('img.preview'),
            formControl = parent.find('input[type=text].selection, input[type=hidden].selection'),
            instanceName = parent.data('data-name');

        parent.find('.select-image,.image-select').on('click', function (evt) {
            evt.preventDefault();
            var targetButton = $(evt.target);


            $('<div id="el_' + instanceName + '" />').dialogelfinder({
                url: '/file',
                lang: 'de',
                getFileCallback: function (file) {
                    $('#el_' + instanceName).dialogelfinder('close');
                    $(previewImage).attr('src', file.url.replace(gwc.serverpath, ''));
                    $(formControl).val(file.url.replace(gwc.serverpath, ''));
                }
            });

        });

        parent.find('.reset-image,.image-reset').on('click', function (evt) {
            evt.preventDefault();
            $(previewImage).attr('src', '');
            $(formControl).val('');
        });

    });

    //SiteSetting select
    $('#selectSiteSetting .dropdown-menu li.site a').click(function (e) {
        e.preventDefault();
        $("#selectCurrentSiteSetting").val($(this).attr('data-id'));
        $("#selectCurrentSiteSettingId").submit();
    });


    function activeTabs() {
        var hash = document.location.hash;
        var prefix = "!";
        if (hash) {
            hash = hash.replace(prefix, '');
            var hashPieces = hash.split('?');
            activeTab = $('[role="tablist"] a[href=' + hashPieces[0] + ']');
            activeTab && activeTab.tab('show');
        }

        $('[role="tablist"] a').on('shown.bs.tab', function (e) {
            window.location.hash = e.target.hash.replace("#", "#" + prefix);
        });
    }

    activeTabs();

    $('#btn-widget-list').click(function (e) {
        console.log('ololo');
        e.preventDefault();
        $('.admin-wdt-list .item:first-child').clone().appendTo('.admin-wdt-list .items').find('textarea').val('').end();
    });

    $('[data-toggle="tooltip"]').tooltip();


    $('form').change(function () {
        $(window).bind('beforeunload', function () {
            return '';
        });
    });

    $('form').submit(function () {
        $(window).unbind('beforeunload');
    });


});