'use strict';


angular.module('tischlerApp', ['ngAnimate', 'ngCookies', 'ngSanitize', 'ngResource', 'ngTouch', 'ngMap', 'sharedApp'])
.controller('TischlerController', ['$rootScope', '$scope', '$http', '$timeout', 'modelService', 'GeoCoder',
      function ($rootScope, $scope, $http, $timeout, modelService, GeoCoder) {


          $scope.filterModel = {
              value1: '',
              value2: '',
              value3: '',
              value4: '',
          };

          $scope.$rootScope = $rootScope;
          $scope.order = "zip"; // zip|distance
          $scope.criteria = "";

          $scope.criteriaMatch = function (criteria) {

              return function (item) {
                  if (criteria == "" || criteria === null)
                  { return true; }

                  return $.inArray(criteria, item.categories) >= 0;
              };
          };

          $scope.manufactories_all = $scope.model.site.widgets[0].manufactories;

          $scope.checkForSelectedManufactory = function (i) { return ($scope.isCurrentManufactory($scope.manufactories_all[i]));  }
          $scope.onContactManufactory        = function (i) { $scope.openContactManufactoryForm($scope.manufactories_all[i]);     }
          $scope.onSelectManufactory         = function (i) { $scope.selectManufactory($scope.manufactories_all[i]);              }

          $scope.$on('mapInitialized', function (e, map)
          {
              $scope.manufactories     = [];
              $scope.map               = map;
              $scope.currentPosition   = $scope.map.getCenter();
              $scope.marker            = null;

              if ($scope.zipCode.length == 0)
              {
                  $scope.order = "zip";
                  $scope.updateVisibleMarkers();
              }
              else
              {
                  $scope.search();
              }

              $scope.onZoomButton = function (e)
              {
                  $scope.updateZoomLevel($scope.zoomButton);
                  $scope.updateVisibleMarkers();
              }

              $scope.zoomChanged = function (e)
              {
                  $scope.updateZoomLevel($scope.map.getZoom());
                  $scope.updateVisibleMarkers();
              }

              $scope.dragend = function (e)
              {
                  $scope.updateVisibleMarkers();
              }
          });

          $scope.findNearest = function()
          {
              var distance = null;
              var nearest  = null;
              var bounds   = $scope.map.getBounds();

              angular.forEach(document.getElementsByTagName('marker'), function (m, i)
              { 
                  var position = new google.maps.LatLng($scope.manufactories_all[i].latitude, $scope.manufactories_all[i].longitude);
                  var getDist = $scope.getDistance($scope.currentPosition, position);
                  $scope.manufactories_all[i].distance = getDist;

                  if (distance == null)
                  {
                      distance = getDist;
                      nearest = position;
                  }
                  else
                  {
                      if (getDist < distance)
                      {
                          distance = getDist;
                          nearest = position;
                      }
                  }
              });

              if($scope.marker != null) { $scope.removeMarker(); }

              console.log($scope.manufactories_all);
              $scope.addMarker();
              bounds.extend(nearest);
              $scope.map.fitBounds(bounds);
          }

          $scope.getRad = function (x)
          {
              return (x * Math.PI / 180);
          };

          $scope.getDistance = function (p1, p2)
          {
              var R     = 6378137;
              var dLat  = $scope.getRad(p2.lat() - p1.lat());
              var dLong = $scope.getRad(p2.lng() - p1.lng());
              var a     = Math.sin(dLat / 2) * Math.sin(dLat / 2) + Math.cos($scope.getRad(p1.lat())) * Math.cos($scope.getRad(p2.lat())) * Math.sin(dLong / 2) * Math.sin(dLong / 2);
              var c     = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
              var d = R * c;

              return (d / 1000);
          };

          $scope.addMarker = function()
          {
              $scope.marker = new google.maps.Marker(
                  {
                    position: $scope.currentPosition,
                    map:      $scope.map,
                    title: $scope.zipCode,
                    animation: google.maps.Animation.DROP,
                    icon:     "http://maps.google.com/mapfiles/ms/icons/red-dot.png",
                  });
          }

          $scope.removeMarker = function()
          {
              $scope.marker.setMap(null);
              $scope.marker = null;
          }

          $scope.updateVisibleMarkers = function ()
          {
              var manu   = [];
              var bounds = $scope.map.getBounds();

              angular.forEach(document.getElementsByTagName('marker'), function (m, i)
              {
                  var position = new google.maps.LatLng($scope.manufactories_all[i].latitude, $scope.manufactories_all[i].longitude);
                  if (bounds.contains(position)) { manu[i] = $scope.manufactories_all[i]; }
              });

              $timeout(function ()
              {
                  $scope.$apply(function () { $scope.manufactories = manu; });
              });
          }

          $scope.updateZoomLevel = function (value)
          {
              if($scope.map.getZoom() != value) { $scope.map.setZoom(value) }
              $scope.zoomButton = value;
          }


          $scope.search = function ()
          {
              $scope.order = "distance";
              if ($scope.zipCode != "")
              {
                  GeoCoder.geocode(
                  {
                    address: $scope.zipCode,
                    region: "de",
                  }
                  ).then(function (results)
                  {
                      $scope.currentPosition = results[0].geometry.location;
                      $scope.map.setCenter($scope.currentPosition);
                      $scope.updateZoomLevel(14);
                      $scope.findNearest();
                      $scope.updateVisibleMarkers();
                  });
                $rootScope.saveZipCode();
              }
          }

      }]);