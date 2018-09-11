//var yumKaaxControllers = angular.module('AdminControllers', []);

///---------------
/// Usuarios Externos
///---------------
yumKaaxControllers.controller("UsuariosPublicosCtrlr", ["$scope", "$http", "$q", "$routeParams", "$timeout", "$location", "authService",
    function ($scope, $http, $q, $routeParams, $timeout, $location, authService) {
        $scope.usuarios = [];
        $scope.usuario = {}
        $scope.paises = [];

        $scope.totalItems = 0;
        $scope.currentPage = 1;
        $scope.maxSize = 5;
        $scope.pageSize = 10;

        if ($routeParams.spk && $routeParams.Id) {
            getUserWithSpk($routeParams.spk, $routeParams.Id);
        }
        if ($routeParams.Id) {
            getPaises();
            getUser($routeParams.Id);
        }
        else {
            getResultsPage(1, $scope.pageSize);
        }
        //alert('wtf usuarios');
        function getUserWithSpk(spk, userId) {
            $http.get(authService.api() + '/Admin/UsuariosPublicos/getwithspk?spk=' + spk + '&id=' + userId)
            .then(function (result) {
                $scope.usuario = result.data.Result;
                //console.log(JSON.stringify(result.data.Result));
            });
        };

        function getPaises() {
            $http.get(authService.api() + '/Admin/Entities/Get?entity=paises')
            .then(function (result) {
                $scope.paises = result.data.Result;
                console.log('paises');
                console.log(JSON.stringify(result.data.Result));
            });
        };

        function getUser(userId) {
            $http.get(authService.api() + '/Admin/UsuariosPublicos/Index?id=' + userId)
            .then(function (result) {

                $scope.usuario = result.data.Result;
                //console.log(JSON.stringify(result.data.Result));
            });
        };

        $scope.isUnchanged = function (usuario) {
            return angular.equals(usuario, $scope.master);
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
            $http.get(authService.api() + '/Admin/UsuariosPublicos/GetPageFilter?filter=' + element + '&page=' + pageNumber + "&pageSize=" + pageSize)
                .then(function (result) {
                    $scope.usuarios = result.data.Result.DataSet;
                    $scope.totalItems = result.data.TotalItems;
                    //`.log(JSON.stringify(result));
                });
        };

        function getResultsPage(pageNumber, pageSize) {
            if ($scope.textSearch)
                $scope.filterFunction($scope.textSearch, pageNumber, pageSize);
            else
                $http.get(authService.api() + '/Admin/UsuariosPublicos/GetPage?page=' + pageNumber + "&pageSize=" + pageSize)
                .then(function (result) {
                    $scope.usuarios = result.data.Result.DataSet;
                    $scope.totalItems = result.data.Result.TotalItems;
                    //console.log(JSON.stringify(result));
                });
        }

        $scope.update = function (usuario) {
            $http.post(authService.api() + "/Admin/UsuariosPublicos/save", { model: usuario })
                .then(
                    function (result) {
                        if (result.data.Succeeded) {
                            $scope.showAlertInfo = true;
                            $scope.alertInfo = "Usuario ha sido grabado correctamente...";
                            $timeout(function () {
                                $scope.showAlertInfo = false;
                                $scope.alertInfo = "";
                                $location.path('/eConsulta');
                            }, 1400);

                        }
                        else {
                            $scope.showAlertInfo = true;
                            $scope.alertInfo = "Error al intentar grabar el Usuario...consulte al admin del sistema";
                            $timeout(function () {
                                $scope.showAlertInfo = false;
                                $scope.alertInfo = "";
                            }, 1400);
                        }
                    },
                    function (error) {
                        $scope.showAlertInfo = true;
                        $scope.alertInfo = "Error al intentar grabar el Usuario...consulte al admin del sistema";
                        $timeout(function () {
                            $scope.showAlertInfo = false;
                            $scope.alertInfo = "";
                        }, 1400);
                    }
                );
        };

        $scope.setPW = function (usuario) {

            $http.post(authService.api() + "/Admin/UsuariosPublicos/ResetPW", { model: usuario })
                .then(
                    function (result) {
                        if (result.data.Succeeded) {
                            $scope.showAlertInfo = true;
                            $scope.alertInfo = "Contrasena ha sido grabada correctamente...";
                            $timeout(function () {
                                $scope.showAlertInfo = false;
                                $scope.alertInfo = "";
                            }, 1400);

                        }
                        else {
                            $scope.showAlertInfo = true;
                            $scope.alertInfo = "Error al intentar grabar contrasena...consulte al admin del sistema";
                            $timeout(function () {
                                $scope.showAlertInfo = false;
                                $scope.alertInfo = "";
                            }, 1400);
                        }
                    },
                    function (error) {
                        $scope.showAlertInfo = true;
                        $scope.alertInfo = "Error al intentar grabar contrasena...consulte al admin del sistema";
                        $timeout(function () {
                            $scope.showAlertInfo = false;
                            $scope.alertInfo = "";
                        }, 1400);
                    }
                );
        };

    }]);