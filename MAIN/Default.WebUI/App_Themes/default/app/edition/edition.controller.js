'use strict';


angular.module('editionApp', ['ngAnimate', 'ngCookies', 'ngSanitize', 'ngResource', 'ngTouch', 'sharedApp'])
.controller('EditionController', ['$rootScope', '$scope', '$http', 'modelService',
      function ($rootScope, $scope, $http, modelService) {



          $scope.criteria = "";
          if (window.localStorage !== undefined) {
              var item = window.localStorage.getItem("criteria");
              if (item !== null) {
                  $scope.criteria = item;
              }
          }
          $scope.$watch('criteria', function () {
              if (window.localStorage !== undefined) {
                  localStorage.setItem("criteria", $scope.criteria);
              }
          });

          $scope.criteriaMatch = function (criteria) {
              return function (item) {
                  if (criteria == "" || criteria === null)
                  { return true; }

                  return item.category === criteria;
              };
          };

          $scope.categoryCount = [];

          $scope.countAllProducts = function () {
              angular.forEach($rootScope.model.site.widgets[0].products, function (product) {
                  if (product in $scope.categoryCount) { $scope.categoryCount[product.category] += 1; }
                  else { $scope.categoryCount[product.category] = 0; }
              });
          }

          $scope.getProductCount = function (category) {
              if (category in $scope.categoryCount) { return $scope.categoryCount[category]; }
              else { return -1; }
          }

          $scope.countProducts = function (category) {
              var productCount = 0;
              angular.forEach($rootScope.model.site.widgets[0].products, function (product) {
                  if (product.category == category) { productCount++; }
              });

              return productCount;
          }

          $scope.currentImage = "";
          $scope.changeCurrentImage = function (imagePath) {
              $scope.currentImage = imagePath;
              setTimeout(function () {
                  $('#zoomPreview').trigger('zoom.destroy');
                  $('#zoomPreview').zoom();
              }, 50);
          };
          $rootScope.editionForm = { noteState: 'off', count: 1 };
          $rootScope.editionFormView = "close";
          $scope.openEditionForm = function () {
              if ($rootScope.currentManufactory) //show dialog or flyout
              {
                  if ($rootScope.editionFormView == "close") {
                      $("#editionForm").modal("show");
                  }
              } else {

                  if (window.localStorage !== undefined) {
                      localStorage.setItem("comeFromEdition", 'true');
                  }
                  $rootScope.onSlideUpdate("show");
              }
          };
          $scope.submitEditionValid = function () {
              if (!($rootScope.editionFormView == "close" || !$rootScope.editionFormView))
              { return false; }

              var form = $rootScope.editionForm;

              if (form.firstName === undefined ||
                  form.lastName === undefined ||
                  form.mail === undefined ||
                  form.telephone === undefined)
              { return false; }

              var mailRegEx = new RegExp('^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,10})$'),
                  telephoneRegEx = new RegExp('^[0-9\s]*$');
              return form.firstName.length > 0 &&
                  form.lastName.length > 0 &&
                  mailRegEx.test(form.mail) &&
                  telephoneRegEx.test(form.telephone) &&
                  form.telephone.length > 0;
          };

          $scope.submitEditionSubmit = function () {

              $rootScope.editionForm.manufactoryId = $rootScope.currentManufactory.id;
              $rootScope.editionForm.type = "EditionOrder";

              $rootScope.editionForm.editions = [];
              $rootScope.editionForm.editions.push({
                  editionId: $rootScope.editionForm.editionId,
                  count: $rootScope.editionForm.count,
                  note: $rootScope.editionForm.note
              });


              $rootScope.editionFormView = "send";
              setTimeout(function () {

                  $http.post('/ajax/editionsOrder', $rootScope.editionForm).
                    then(function (response) {
                        console.log(response);
                        $rootScope.editionFormView = "success";
                        setTimeout(function () {
                            $("#editionForm").modal("hide");
                            $rootScope.editionFormView = "close";
                        }, 15000);

                    }, function (response) {
                        console.error(response);
                        $rootScope.editionFormView = "error"; //show error
                        setTimeout(function () { //go to start view
                            $rootScope.editionFormView = "close";
                        }, 15000);
                    });
              }, 2000);
          };



          $scope.setEditionFormView = function (state) {


              $rootScope.editionFormView = state;

          };
          $scope.showCount = false;
          var wert;
          var change = document.getElementById("change");
          $("#change").click(function () {
              $scope.showCount = !$scope.showCount;
              wert = $rootScope.editionForm.count;
          });

          var save = document.getElementById("save");
          $("#save").click(function () {
              $scope.showCount = !$scope.showCount;
          });

          var abort = document.getElementById("abort");
          $("#abort").click(function () {
              $rootScope.editionForm.count = wert;
              $scope.showCount = !$scope.showCount;
          });


          $(function () {
              setTimeout(function () {
                  var resize_items = $(".full-width-resize");
                  $(resize_items.data("target-resize")).height(resize_items.height());
              }, 50);

              setTimeout(function () {
                  var resize_items = $(".full-width-resize");
                  $(resize_items.data("target-resize")).height(resize_items.height());
              }, 500);

              window.onresize = function () {
                  var resize_items = $(".full-width-resize");
                  $(resize_items.data("target-resize")).height(resize_items.height());
              };
          });

          $scope.getSum = function () {

              var sum = 0;
              angular.forEach($rootScope.editions, function (e) {
                  sum += e.count * e.edition.price;
              });

              return sum;
          };

          /**
          * INIT
          */
          (function () {
              if (window.location.pathname.indexOf('/edition') > -1) {
                  $scope.countAllProducts();
              }
              if (window.location.pathname.indexOf('/edition/') > -1) {



                  if (window.localStorage !== undefined) {
                      if (localStorage.getItem("comeFromEdition") && localStorage.getItem("comeFromEdition") == 'true') {
                          localStorage.setItem("comeFromEdition", null);
                          $scope.openEditionForm();

                      }
                  }

              }
          })();
      }]);