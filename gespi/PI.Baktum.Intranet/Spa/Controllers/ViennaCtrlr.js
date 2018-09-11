﻿//var yumKaaxControllers = angular.module('AdminControllers', []);

///---------------
/// Vienna
///---------------
yumKaaxControllers.controller("ViennaCtrlr", ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "authService",
function ($scope, $http, $q, $routeParams, $timeout, $route, authService) {
    $scope.$route = $route;
    $scope.Vienna = [];
    $scope.vienna = {};
    $scope.showAlertInfo = "";
    $scope.alertInfo = false;

    $scope.totalItems = 0;
    $scope.currentPage = 1;
    $scope.maxSize = 5;
    $scope.pageSize = 10;

    if ($routeParams.Id) {
        getEntity($routeParams.Id);
    }
    else {
        getResultsPage(1, $scope.pageSize);
    }

    function getEntity(Id) {
        $http.get(authService.api() + '/Admin/Vienna/Index?id=' + Id)
        .then(function (result) {
            $scope.vienna = result.data.Result;
            //console.log(JSON.stringify(result.data.Result));
            console.log('Fetch!!');
        });
    };

    $scope.isUnchanged = function (vienna) {
        return angular.equals(vienna, $scope.master);
    };

    $scope.pagination = {
        current: 1
    };

    $scope.pageChanged = function () {
        getResultsPage($scope.currentPage, $scope.pageSize);
    };

    $scope.$watch('textSearch', function (newVal, oldVal) {
        if (newVal !== oldVal) {
            $scope.filterFunction(newVal, $scope.pageNumber, $scope.pageSize);
        }
    }, true);

    $scope.filterFunction = function (element, pageNumber, pageSize) {
        $http.get(authService.api() + '/Admin/Vienna/GetPageFilter?filter=' + element + '&page=' + pageNumber + "&pageSize=" + pageSize)
            .then(function (result) {
                $scope.Vienna = result.data.Result.DataSet;
                $scope.totalItems = result.data.TotalItems;
                console.log(JSON.stringify(result));
            });
    };

    function getResultsPage(pageNumber, pageSize) {
        if ($scope.textSearch)
            $scope.filterFunction($scope.textSearch, pageNumber, pageSize);
        else
            $http.get(authService.api() + '/Admin/Vienna/GetPage?page=' + pageNumber + "&pageSize=" + pageSize)
            .then(function (result) {
                $scope.Vienna = result.data.Result.DataSet;
                $scope.totalItems = result.data.Result.TotalItems;
                //console.log(JSON.stringify(result));
            });
    }

    $scope.update = function (vienna) {
        $http.post(authService.api() + "/Admin/Vienna/save", { model: vienna })
            .then(
                function (result) {
                    if (result.data.Succeeded) {
                        $scope.showAlertInfo = true;
                        $scope.alertInfo = "vienna ha sido grabado correctamente...";
                        $timeout(function () {
                            $scope.showAlertInfo = false;
                            $scope.alertInfo = "";
                        }, 1400);

                    }
                    else {
                        $scope.showAlertInfo = true;
                        $scope.alertInfo = "Error al intentar grabar el vienna...consulte al admin del sistema";
                        $timeout(function () {
                            $scope.showAlertInfo = false;
                            $scope.alertInfo = "";
                        }, 1400);
                    }
                },
                function (error) {
                    $scope.showAlertInfo = true;
                    $scope.alertInfo = "Error al intentar grabar el vienna...consulte al admin del sistema";
                    $timeout(function () {
                        $scope.showAlertInfo = false;
                        $scope.alertInfo = "";
                    }, 1400);
                }
            );
    };

}]); //End Vienna
