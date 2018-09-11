//var yumKaaxControllers = angular.module('AdminControllers', []);

///---------------
/// Usuarios
///---------------
yumKaaxControllers.controller("UsuariosCtrlr", ["$scope", "$http", "$q", "$routeParams", "$timeout", "$location", "authService", "listService",
    function ($scope, $http, $q, $routeParams, $timeout, $location, authService, listService) {
        $scope.usuarios = [];
        $scope.usuario = {}
        $scope.totalItems = 0;
        $scope.currentPage = 1;
        $scope.maxSize = 5;
        $scope.pageSize = 10;
        $scope.roles = [];
        $scope.perms = undefined;
        $scope.appPopupBox = new appPopupBox($scope);

        if ($routeParams.spk && $routeParams.Id) {
            getUserWithSpk($routeParams.spk, $routeParams.Id);
        }
        if ($routeParams.Id) {
            getRoles();
            getUser($routeParams.Id);
        }
        else {
            getResultsPage(1, $scope.pageSize);
        }
        
        function getUserWithSpk(spk, userId) {
            $http.get(authService.api() + '/Admin/Usuarios/getwithspk?spk=' + spk + '&id=' + userId)
            .then(function (result) {
                $scope.usuario = result.data.Result;
                //console.log(JSON.stringify(result.data.Result));
            });
        };

        function getRoles() {
            $http.get(authService.api() + '/Admin/Roles/GetPage?page=0&pageSize=0')
            .then(function (result) {
                $scope.roles = result.data.Result.DataSet;
                console.log('roles');
                console.log(JSON.stringify(result.data.Result.DataSet));
            });
        };

        function getUser(userId) {
            $http.get(authService.api() + '/Admin/Permisos/GetAll?usuarioId=' + userId)
            .then(function (result) {
                var dbPerms = result.data.Result.DataSet;
                console.log(JSON.stringify(dbPerms));

                var grlPerms = listService.getGrlPerms();
                var da = listService.getDAmnu();
                var pat = listService.getPatentesmnu();
                var mar = listService.getMarcasmnu();
                var marmnu = [];
                for (var i = 0; i < mar.length; i++) {
                    if (mar[i].menu) {
                        for (var j = 0; j < mar[i].menu.length; j++) {
                            marmnu.push(mar[i].menu[j]);
                        }
                    }
                    else {
                        marmnu.push(mar[i]);
                    }
                }

                var perms = grlPerms.concat(pat, da, marmnu);
                //console.log(JSON.stringify(perms));

                var found = 0;
                for (var i = 0; i < perms.length; i++) {
                    found = 0;
                    for (var j = 0; j < dbPerms.length; j++) {
                        if (dbPerms[j].Opcion == perms[i]._id) {
                            perms[i].Acceso = dbPerms[j].Acceso;
                            found = 1;
                            break;
                        }
                    }
                    if (!found)
                        perms[i].Acceso = false;
                }
                $scope.perms = perms;
                //show perms

                //console.log(JSON.stringify(result.data.Result));
            });

            $http.get(authService.api() + '/Admin/Usuarios/Index?id=' + userId)
            .then(function (result) {
                $scope.usuario = result.data.Result;
                //console.log(JSON.stringify(result.data.Result));
            });
        };

        $scope.pageChanged = function () {
            console.log('Page changed to: ' + $scope.currentPage);
            getResultsPage($scope.currentPage, $scope.pageSize);
        };

        $scope.isUnchanged = function (usuario) {
            return angular.equals(usuario, $scope.master);
        };

        $scope.pagination = {
            current: 1
        };


        $scope.$watch('textSearch', function (newVal, oldVal) {
            if (newVal !== oldVal) {
                $scope.filterFunction(newVal, $scope.pageNumber, $scope.pageSize);
            }
        }, true);

        function fixModelDates() {
            for (var i = 0; i < $scope.usuarios.length; i++) {
                $scope.usuarios[i].SpkExpiration = fixDotNetToJsonDate($scope.usuarios[i].SpkExpiration);
            }
        }

        $scope.filterFunction = function (element, pageNumber, pageSize) {
            $http.get(authService.api() + '/Admin/Usuarios/GetPageFilter?filter=' + element + '&page=' + pageNumber + "&pageSize=" + pageSize)
                .then(function (result) {
                    $scope.totalItems = result.data.Result.TotalItems;
                    $scope.usuarios = result.data.Result.DataSet;
                    fixModelDates()
                });
        };

        function getResultsPage(pageNumber, pageSize) {
            if ($scope.textSearch)
                $scope.filterFunction($scope.textSearch, pageNumber, pageSize);
            else
                console.log(pageSize);
                $http.get(authService.api() + '/Admin/Usuarios/GetPage?page=' + pageNumber + "&pageSize=" + pageSize)
                .then(function (result) {
                    $scope.totalItems = result.data.Result.TotalItems;
                    $scope.usuarios = result.data.Result.DataSet;
                    fixModelDates();
                });
        }

        $scope.update = function (usuario) {
            $http.post(authService.api() + "/Admin/Usuarios/save", { model: usuario })
                .then(
                    function (result) {                        
                        if (result.data.Succeeded) {
                            $scope.appPopupBox.handleError(null,"Usuario ha sido grabado correctamente...");
                        }
                        else {
                            $scope.appPopupBox.handleError(error);
                        }
                    },
                    function (error) {
                        $scope.appPopupBox.handleError(error);
                    }
                );
        };

        $scope.savePerms = function () {
            var perms = [];            
            for (var i = 0; i < $scope.perms.length; i++) {
                perms.push({ UsuarioId: $scope.usuario.Id, Opcion: $scope.perms[i]._id, Acceso: $scope.perms[i].Acceso, OtorgadoPorUsuarioId: 0 })
            }

            $http.post(authService.api() + "/Admin/Permisos/save", { model: perms })
            .then(
                function (result) {
                    if (result.data.Succeeded) {                        
                        $scope.appPopupBox.handleError(null, "Permisos han sido actualizados correctamente...");
                    }
                    else {
                        $scope.appPopupBox.handleError(error);
                    }
                },
                function (error) {
                    $scope.appPopupBox.handleError(error);
                }
            );
        }

        $scope.setPW = function (usuario) {
            //TODO: review reset password process
            $http.post(authService.api() + "/Admin/Usuarios/ResetPW", { model: usuario })
                .then(
                    function (result) {
                        if (result.data.Succeeded) {
                            $scope.appPopupBox.handleError(null, "Contrasena ha sido grabada correctamente...");
                        }
                        else {
                            $scope.appPopupBox.handleError(error);
                        }
                    },
                    function (error) {
                        $scope.appPopupBox.handleError(error);
                    }
                );
        };

}]);