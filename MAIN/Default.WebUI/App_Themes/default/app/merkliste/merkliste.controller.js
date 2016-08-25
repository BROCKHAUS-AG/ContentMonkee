'use strict';


angular.module('merklisteApp', ['ngAnimate', 'ngCookies', 'ngSanitize', 'ngResource', 'ngTouch', 'sharedApp'])
.controller('MerklisteController', ['$rootScope', '$scope', '$http', 'modelService',
      function ($rootScope, $scope, $http, modelService) {


          $scope.getSum = function () {

              var sum = 0;
              angular.forEach($rootScope.editions, function (e) {
                  sum += e.count * e.edition.price;
              });

              return sum;
          };

          $scope.addEdition = function (edition) {
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
              $rootScope.saveEditions();
          };

          $scope.removeEdition = function (obj) {
              var i = $rootScope.editions.indexOf(obj);
              if (i != -1) {
                  $rootScope.editions.splice(i, 1);
              }
              $rootScope.saveEditions();
          };



          $rootScope.editionsForm = {};
          $rootScope.editionsFormView = "close";
          $scope.openEditionsForm = function () {
              if ($rootScope.currentManufactory) //show dialog or flyout
              {
                  $rootScope.editionsFormView == "close";
                  $("#editionsForm").modal("show");
              }
              else
              { $rootScope.onSlideUpdate("show"); }
          };
          $scope.submitEditionsValid = function () { 
              if (!($rootScope.editionsFormView == "close" || !$rootScope.editionsFormView))
              { return false; }

              var form = $rootScope.editionsForm;

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

          $scope.submitEditionsSubmit = function () {

              $rootScope.editionsForm.manufactoryId = $rootScope.currentManufactory.id;
              $rootScope.editionsForm.type = "EditionsOrder";

              $rootScope.editionsForm.editions = [];
              angular.forEach($rootScope.editions, function (e) {
                  $rootScope.editionsForm.editions.push({ editionId: e.edition.id, count: e.count, note: e.note });
              });

              $rootScope.editionsFormView = "send";
              setTimeout(function () { 

                  $http.post('/ajax/editionsOrder', $rootScope.editionsForm).
                    then(function (response) {
                        console.log(response);
                        $rootScope.editionsFormView = "success";
                        setTimeout(function () {
                            $scope.resetEditions(); //from shared
                            $("#editionsForm").modal("hide");
                            $rootScope.editionsFormView = "close";
                        }, 15000);

                    }, function (response) {
                        console.error(response);
                        $rootScope.editionsFormView = "error"; //show error
                        setTimeout(function () { //go to start view
                            $rootScope.editionsFormView = "close";
                        }, 15000);
                    });
              },2000);
          };


      }]);