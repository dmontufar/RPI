///---------------
/// Estatuses
///---------------
yumKaaxControllers.controller("EstatusesCtrlr", ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "authService",
function ($scope, $http, $q, $routeParams, $timeout, $route, authService) {
    $scope.$route = $route;
    $scope.Estatuses = [];
    $scope.estatus = {};
    $scope.showAlertInfo = "";
    $scope.alertInfo = false;
    $scope.Modulos = [{ Id: 1, name: 'Marcas' }, { Id: 2, name: 'Patentes' }, { Id: 3, name: 'Derecho de Autor' }];
    $scope.moduloSelected = { Id: 1, name: 'Marcas' };
    $scope.appPopupBox = new appPopupBox($scope);

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
        $http.get( authService.api() + '/Admin/Estatus/Index?id=' + Id)
        .then(function (result) {           
            //$scope.estatus = result.data.Result;
            $scope.estatus = parseModulo(result.data.Result);
            //console.log(JSON.stringify(result.data.Result));
        });
    };

    function parseModulo(estatus)
    {
        if (estatus.ModuloId == 1)
            estatus.ModuloId = $scope.Modulos[0];
        else if (estatus.ModuloId == 2)
            estatus.ModuloId = $scope.Modulos[1];
        else if (estatus.ModuloId == 3)
            estatus.ModuloId = $scope.Modulos[2];
        else
            estatus.ModuloId = $scope.Modulos[0];
        return estatus;
    }

    $scope.isUnchanged = function (estatus) {
        return angular.equals(estatus, $scope.master);
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
            $scope.filterFunction('', $scope.pageNumber, $scope.pageSize, newVal.Id);
        }
    }, true);

    $scope.filterFunction = function (element, pageNumber, pageSize, moduloId) {
        $http.get(authService.api() + '/Admin/Estatus/GetPageFilter?filter=' + element + '&page=' + pageNumber + "&pageSize=" + pageSize + "&moduloId=" + moduloId)
            .then(function (result) {
                $scope.Estatuses = result.data.Result.DataSet;
                $scope.totalItems = result.data.TotalItems;
                console.log(JSON.stringify(result));
            });
    };

    function getResultsPage(pageNumber, pageSize) {
        if ($scope.textSearch)
            $scope.filterFunction($scope.textSearch, pageNumber, pageSize, $scope.moduloSelected.Id);
        else
            $http.get(authService.api() + '/Admin/Estatus/GetPage?page=' + pageNumber + "&pageSize=" + pageSize + "&moduloId=" + $scope.moduloSelected.Id)
            .then(function (result) {
                $scope.Estatuses = result.data.Result.DataSet;
                $scope.totalItems = result.data.Result.TotalItems;
                //console.log(JSON.stringify(result));
            });
    }

    $scope.update = function (estatus) {
        estatus.ModuloId = estatus.ModuloId.Id;
        $http.post(authService.api() + "/Admin/Estatus/save", { model: estatus })
            .then(
                function (result) {
                    if (result.data.Succeeded) {
                        $scope.appPopupBox.handleError(null, "estatus ha sido grabado correctamente...");
                    }
                    else {
                        $scope.appPopupBox.handleError(null, "Error al intentar grabar el estatus...consulte al admin del sistema");
                    }
                    estatus = parseModulo(estatus);
                },
                function (error) {
                    $scope.appPopupBox.handleError(error);
                    estatus = parseModulo(estatus);
                }
            );        
    };

}]); //End Estatuses
