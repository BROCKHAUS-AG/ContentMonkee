'use strict';
angular.module('sharedApp', ['ngAnimate', 'ngCookies', 'ngSanitize', 'ngResource', 'ngTouch'])
    .service('searchService', [
        '$resource', function ($resource) {
            var resource = $resource('/api/ManufactorySearch/:q', { q: '@q' }, {
                getByZip: { url: '/api/manufactorysearch/get', method: 'GET', isArray: true }
            });
            resource.get = function () { //override default
                return resource.query();
            };
            return resource;
        }
    ])
    .service('modelService', [
        function () {
            this.getViewModel = function () {
                return window.gwc.model;
            };
            this.getGuid = function () {
                var d = new Date().getTime();
                var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                    var r = (d + Math.random() * 16) % 16 | 0;
                    d = Math.floor(d / 16);
                    return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
                });
                return uuid;
            };
        }
    ])
    .controller('SharedController', [
        '$rootScope', '$scope', '$http', 'modelService', 'searchService',
        function ($rootScope, $scope, $http, modelService, searchService) {

            $rootScope.model = modelService.getViewModel();

            /* ZIP CODE START */
            $rootScope.saveZipCode = function () {
                localStorage.setItem("zipCode", $scope.zipCode);
            };
            /* ZIP CODE END */

            /**
            * Manufactory START
            */
            $rootScope.zipCode = "";
            $rootScope.searchManufactoryModel = {};
            $rootScope.currentManufactory = undefined;
            $rootScope.isFormValid = false;

            $rootScope.searchManufactory = function (redirect) {
                if (redirect) {

                    if (window.localStorage !== undefined) {
                        localStorage.setItem("comeFrom", window.location);
                    }
                    //Window.Location.
                } else {
                    $rootScope.searchManufactoryModel = searchService.getByZip({ q: $rootScope.zipCode });
                }
                $rootScope.saveZipCode();
            };
            $rootScope.unselectedManufactory = function (manufactory) {
                if (!$rootScope.currentManufactory) {
                    return false;
                }
                return manufactory.url !== $rootScope.currentManufactory.url;
            };
            //$rootScope.selectedManufactory = function (manufactory) {
            //    $rootScope.currentManufactory = manufactory;
            //    localStorage.setItem("currentManufactory", JSON.stringify($rootScope.currentManufactory));
            //};
            $rootScope.selectManufactory = function (manufactory) {
                $rootScope.currentManufactory = manufactory;
                localStorage.setItem("currentManufactory", JSON.stringify($rootScope.currentManufactory));

                if (window.localStorage !== undefined) {
                    var item = window.localStorage.getItem("comeFrom");
                    localStorage.setItem("comeFrom", "");

                    if (item) {
                        window.location = item;
                    }
                }

            };
            $rootScope.isCurrentManufactory = function (manufactory) {
                if (!$rootScope.currentManufactory) {
                    return false;
                }
                return $rootScope.currentManufactory.url == manufactory.url;
            };

            $rootScope.onSlideUpdate = function (change) {
                window.onSlideUpdate(change);
            }


            $rootScope.contactForm = {};
            $rootScope.contactFormView = "close";
            $rootScope.openContactManufactoryForm = function (manufactory) {
                $rootScope.contactFormView = "close";
                if (manufactory)
                    $rootScope.selectManufactory(manufactory);
                $("#contactForm").modal("show");
            };

            $rootScope.contactManufactoryValid = function () {
                if (!($rootScope.contactFormView == "close" || !$rootScope.contactFormView))
                { return false; }

                var form = $rootScope.contactForm;

                if (form.firstName === undefined ||
                    form.lastName === undefined ||
                    form.mail === undefined ||
                    form.telephone === undefined ||
                    form.msg === undefined)
                { return false; }

                var mailRegEx = new RegExp('^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,10})$'),
                  telephoneRegEx = new RegExp('^[0-9\s]*$');
                return form.firstName.length > 0 &&
                    form.lastName.length > 0 &&
                    form.msg.length > 0 &&
                    mailRegEx.test(form.mail) &&
                    telephoneRegEx.test(form.telephone) &&
                    form.telephone.length > 0;
            };

            $rootScope.reValidateForm = function ($event) {
                var element = $($event),
                    span = element.parent().find('span'),
                    error = false,
                    errorText = '',
                    valRequired = element.data('val-required'),
                    valPattern = element.data('val-regex-pattern'),
                    valPatternError = element.data('val-regex');
                if (valRequired) {
                    if (element.val() === '') {
                        error = true;
                        errorText = valRequired;
                    }
                }
                if (valPattern) {
                    var regEx = new RegExp(valPattern),
                        value = element.val();
                    if (!regEx.test(value)) {
                        error = true;
                        errorText = valPatternError;
                    }
                }

                if (error) {
                    span.text(errorText);
                    span.css('display', 'block');
                } else {
                    span.html('&nbsp;');
                    span.css('display', 'none');
                }

            }

            $rootScope.contactManufactorySubmit = function () {
                $rootScope.contactForm.manufactoryId = $rootScope.currentManufactory.id;

                $rootScope.contactForm.firstName = $("#contactForm #FirstName").val();
                $rootScope.contactForm.lastName = $("#contactForm #LastName").val();
                $rootScope.contactForm.mail = $("#contactForm #Mail").val();
                $rootScope.contactForm.from = $("#contactForm #Mail").val();
                $rootScope.contactForm.telephone = $("#contactForm #Telephone").val();
                $rootScope.contactForm.msg = $("#contactForm #Message").val();


                $rootScope.contactFormView = "send";
                setTimeout(function () {

                    $http.post('/ajax/contact', $rootScope.contactForm).
                      then(function (response) {
                          console.log(response);
                          $rootScope.contactFormView = "success";
                          setTimeout(function () {
                              $scope.resetEditions(); //from shared
                              $("#contactForm").modal("hide");
                              $rootScope.contactFormView = "close";
                          }, 15000);

                      }, function (response) {
                          console.error(response);
                          $rootScope.contactFormView = "error"; //show error
                          setTimeout(function () { //go to start view
                              $rootScope.contactFormView = "close";
                          }, 15000);
                      });
                }, 2000);

            }


            /**
             * Manufactory END
             */



            /**
            * Editions START + Merkliste.Controller
            */
            $rootScope.editions = [];



            $rootScope.getCounter = function () {
                var counter = 0;
                angular.forEach($rootScope.editions, function (e) {
                    counter += e.count;
                });
                return counter;
            };

            $rootScope.addEdition = function (edition) {
                var isNew = true;
                angular.forEach($rootScope.editions, function (e) {
                    if (e.edition.url == edition.url) {
                        e.count++;
                        isNew = false;
                    }
                });
                if (isNew) {
                    $rootScope.editions.push({
                        edition: edition,
                        count: 1,
                        noteState: "close",
                        note: ""
                    });
                }
                $scope.saveEditions();
            };

            $rootScope.resetEditions = function () {
                $rootScope.$apply(function () {
                    $rootScope.editions = [];
                });
                $rootScope.saveEditions();
            };

            $rootScope.saveEditions = function () {
                angular.forEach($rootScope.editions, function (e) {
                    e.count = e.count | 0; //to int
                });
                localStorage.setItem("editions", JSON.stringify($rootScope.editions));
            };

            $scope.removeEdition = function (obj) {
                var i = $rootScope.editions.indexOf(obj);
                if (i != -1) {
                    $rootScope.editions.splice(i, 1);
                }
                $scope.saveEditions();
            };
            /**
            * Editions END
            */



            /**
            * INIT
            */
            (function () {
                var editionsString = localStorage.getItem("editions");
                $rootScope.editions = JSON.parse(editionsString || "[]");

                var currentManufactoryString = localStorage.getItem("currentManufactory");
                $rootScope.currentManufactory = JSON.parse(currentManufactoryString || null);

                var zipCode = localStorage.getItem("zipCode");
                $rootScope.zipCode = zipCode || "";
                if ($rootScope.zipCode && (window.location.pathname == "/tischler" || window.location.pathname == "/tischler")) {
                    $rootScope.searchManufactory(false);
                }
            })();

            $rootScope.$watch('currentManufactory', function (newValue, oldValue) {
                setTimeout(function () {
                    $rootScope.onSlideUpdate(false);
                }, 100);
            });


            //alert("SharedController loaded");
            //$scope.cart = Cart;
            //$http.get('articles.json').then(function (articlesResponse) {
            //    $scope.articles = articlesResponse.data;
            //});

        }])
    .directive('bagChange', function () {
        return {
            link: function ($scope, $element, $attrs) {
                $element.change(function () {
                    var fn = $attrs.bagChange;
                    $scope[fn]($element);
                });
            }
        };
    });
//  .controller('CartCtrl', function ($scope, Cart) {
//      $scope.cart = Cart;
//  })
//;






//var phonecatApp = angular.module('phonecatApp', []);

//phonecatApp.controller('SharedCtrl', function ($scope) {
//    $scope.phones = [
//      {
//          'name': 'Nexus S',
//          'snippet': 'Fast just got faster with Nexus S.'
//      },
//      {
//          'name': 'Motorola XOOM™ with Wi-Fi',
//          'snippet': 'The Next, Next Generation tablet.'
//      },
//      {
//          'name': 'MOTOROLA XOOM™',
//          'snippet': 'The Next, Next Generation tablet.'
//      }
//    ];
//});