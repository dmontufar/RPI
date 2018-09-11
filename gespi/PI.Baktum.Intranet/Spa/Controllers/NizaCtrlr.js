//var yumKaaxControllers = angular.module('AdminControllers', []);

///---------------
/// Niza
///---------------
yumKaaxControllers.controller("NizaCtrlr", ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "authService",
function ($scope, $http, $q, $routeParams, $timeout, $route, authService) {
    $scope.$route = $route;
    $scope.Niza = [];
    $scope.niza = {};
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
        $http.get(authService.api() + '/Admin/Niza/Index?id=' + Id)
        .then(function (result) {
            $scope.niza = result.data.Result;
            //console.log(JSON.stringify(result.data.Result));
            console.log('Fetch!!');
        });
    };

    $scope.isUnchanged = function (niza) {
        return angular.equals(niza, $scope.master);
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
        $http.get(authService.api() + '/Admin/Niza/GetPageFilter?filter=' + element + '&page=' + pageNumber + "&pageSize=" + pageSize)
            .then(function (result) {
                $scope.Niza = result.data.Result.DataSet;
                $scope.totalItems = result.data.TotalItems;
                console.log(JSON.stringify(result));
            });
    };

    function getResultsPage(pageNumber, pageSize) {
        if ($scope.textSearch)
            $scope.filterFunction($scope.textSearch, pageNumber, pageSize);
        else
            $http.get(authService.api() + '/Admin/Niza/GetPage?page=' + pageNumber + "&pageSize=" + pageSize)
            .then(function (result) {
                $scope.Niza = result.data.Result.DataSet;
                $scope.totalItems = result.data.Result.TotalItems;
                //console.log(JSON.stringify(result));
            });
    }

    $scope.update = function (niza) {
        $http.post(authService.api() + "/Admin/Niza/save", { model: niza })
            .then(
                function (result) {
                    if (result.data.Succeeded) {
                        $scope.showAlertInfo = true;
                        $scope.alertInfo = "niza ha sido grabado correctamente...";
                        $timeout(function () {
                            $scope.showAlertInfo = false;
                            $scope.alertInfo = "";
                        }, 1400);

                    }
                    else {
                        $scope.showAlertInfo = true;
                        $scope.alertInfo = "Error al intentar grabar el niza...consulte al admin del sistema";
                        $timeout(function () {
                            $scope.showAlertInfo = false;
                            $scope.alertInfo = "";
                        }, 1400);
                    }
                },
                function (error) {
                    $scope.showAlertInfo = true;
                    $scope.alertInfo = "Error al intentar grabar el niza...consulte al admin del sistema";
                    $timeout(function () {
                        $scope.showAlertInfo = false;
                        $scope.alertInfo = "";
                    }, 1400);
                }
            );
    };

}]); //End Niza
