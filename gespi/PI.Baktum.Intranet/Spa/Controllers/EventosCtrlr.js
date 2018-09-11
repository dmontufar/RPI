//var yumKaaxControllers = angular.module('AdminControllers', []);

///---------------
/// Eventos
///---------------
yumKaaxControllers.controller("EventosCtrlr", ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "authService",
function ($scope, $http, $q, $routeParams, $timeout, $route, authService) {
    $scope.$route = $route;
    $scope.Eventos = [];
    $scope.evento = {};
    $scope.showAlertInfo = "";
    $scope.alertInfo = false;
    $scope.Modulos = [{ Id: 1, name: 'Marcas' }, { Id: 2, name: 'Patentes' }, { Id: 3, name: 'Derecho de Autor' }];
    $scope.moduloSelected = 1;

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
        $http.get(authService.api() + '/Admin/Eventos/Index?id=' + Id)
        .then(function (result) {
            $scope.evento = result.data.Result;
            //console.log(JSON.stringify(result.data.Result));
        });
    };

    $scope.isUnchanged = function (evento) {
        return angular.equals(evento, $scope.master);
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

    $scope.$watch('moduloSelected', function (newVal, oldVal) {
        if (newVal !== oldVal) {
            $scope.filterFunction('', $scope.pageNumber, $scope.pageSize, newVal);
        }
    }, true);

    $scope.filterFunction = function (element, pageNumber, pageSize, moduloId) {
        $http.get(authService.api() + '/Admin/Eventos/GetPageFilter?filter=' + element + '&page=' + pageNumber + "&pageSize=" + pageSize + "&moduloId=" + moduloId)
            .then(function (result) {
                $scope.Eventos = result.data.Result.DataSet;
                $scope.totalItems = result.data.TotalItems;
                console.log(JSON.stringify(result));
            });
    };

    function getResultsPage(pageNumber, pageSize) {
        if ($scope.textSearch)
            $scope.filterFunction($scope.textSearch, pageNumber, pageSize, $scope.moduloSelected);
        else
            $http.get(authService.api() + '/Admin/Eventos/GetPage?page=' + pageNumber + "&pageSize=" + pageSize + "&moduloId=" + $scope.moduloSelected)
            .then(function (result) {
                $scope.Eventos = result.data.Result.DataSet;
                $scope.totalItems = result.data.Result.TotalItems;
                //console.log(JSON.stringify(result));
            });
    };

    $scope.update = function (evento) {

        $http.post(authService.api() + "/Admin/Eventos/save", { model: evento })
            .then(
                function (result) {
                    if (result.data.Succeeded) {
                        $scope.showAlertInfo = true;
                        $scope.alertInfo = "evento ha sido grabado correctamente...";
                        $timeout(function () {
                            $scope.showAlertInfo = false;
                            $scope.alertInfo = "";
                        }, 1400);

                    }
                    else {
                        $scope.showAlertInfo = true;
                        $scope.alertInfo = "Error al intentar grabar el evento...consulte al admin del sistema";
                        $timeout(function () {
                            $scope.showAlertInfo = false;
                            $scope.alertInfo = "";
                        }, 1400);
                    }
                },
                function (error) {
                    $scope.showAlertInfo = true;
                    $scope.alertInfo = "Error al intentar grabar el evento...consulte al admin del sistema";
                    $timeout(function () {
                        $scope.showAlertInfo = false;
                        $scope.alertInfo = "";
                    }, 1400);
                }
            );
    };

}]); //End Eventos