//*************************************************
// CONTROLLERS DE MARCAS
//*************************************************
yumKaaxControllers.controller("mexpedienteCtrl", ["$scope", "$location", "authService", "listService", "$http", "$routeParams",
    function ($scope, $location, authService, listService, $http, $routeParams) {
        $scope.tipoDeRegistroDeMarcas = [];
        $scope.model = {};
        $scope.grupos = [];
        $scope.showAlertInfo = "";
        $scope.alertInfo = false;
        $scope.model.EsFavorito = false;


        if ($routeParams.Id) {
            getExpediente('ExpedienteId', '?Id=' + $routeParams.Id);
        }

        function getExpediente(action, queryString) {
            var service = '/Marca/';

            $http.get(authService.api() + service + action + queryString,
            {
                headers: { "access_token": authService.getToken() }
            })
            .then(function (result) {
                $scope.model = result.data.Result.documento;
                //console.log(JSON.stringify($scope.opciones));
                console.log('Fetched!!');
            });
        }

        $scope.getExpedienteByNumero = function (expediente) {
            getExpediente('Expediente', '?numero=' + expediente);
        }

        $scope.getExpedienteByRegistro = function (tipoDeRegistroId, registro) {
            getExpediente('registro', '?tipoDeRegistroId=' + tipoDeRegistroId.Id + '&registro=' + registro);
        }

        $scope.tipoDeRegistroDeMarcas = [];

        function getTipoDeRegistro() {
            listService.reFetchLists();
            var lists = listService.getLists(); // all the lists
            $scope.tipoDeRegistroDeMarcas = lists.tipoDeRegistroDeMarcas;
        }
        getTipoDeRegistro()

    }]);

yumKaaxControllers.controller("mlogotiposCtrl", ["$scope", "$location", "authService", "classSrvc", function ($scope, $location, authService, classSrvc) {
    $scope.userIdentity = authService.getUserInfo();
    $scope.niza = classSrvc.getNizaClass();
    $scope.nizaSel = { niza: {} };
    $scope.vienalstSel = [];
    $scope.vienalst = classSrvc.getVienaClass();

    $scope.selectViena = function (viena) {
        $scope.vienalstSel.push(viena);
        $scope.vienaSelected = null;
    };
}]);

yumKaaxControllers.controller("mfoneticaCtrl", ["$scope", "$location", "authService", "classSrvc", "$http", function ($scope, $location, authService, classSrvc, $http) {
    $scope.userIdentity = authService.getUserInfo();
    $scope.niza = classSrvc.getNizaClass();
    $scope.nizaSel = { niza: {} };
    $scope.tipoDeBusqueda = [{ Id: 1, Nombre: "Identica" }, { Id: 2, Nombre: "Fonetica" }];
    $scope.tipoDeBusquedaSel = $scope.tipoDeBusqueda[1];
    $scope.pageNumber = 0;
    $scope.pageSize = 0;
    $scope.dataset = [];

    $scope.searchMarcas = function () {
        
        var csvClases = '';
        var nizaSel = $scope.nizaSel.niza;
        if (nizaSel['All']) {
            csvClases = ''
        }
        else {
            for (i = 0; i < 46; i++) {
                if (nizaSel[i]) {
                    csvClases += (i).toString() + ','
                }
            }
            if (nizaSel[99]) {
                csvClases += '99,'
            }
        }
        if (csvClases.length > 1) {
            csvClases = csvClases.slice(0, -1);
        }

        var service = '/Marca/';

        $scope.getExpedienteUrl = function (e) {
            return "mexpediente/" + e.ExpedienteId;
        };
        
        if (!$scope.tipoDeBusquedaSel || $scope.tipoDeBusquedaSel.Id == 2) {
            $http.get(authService.api() + service + 'BusquedaFonetica?pageNumber=' + $scope.pageNumber
                + "&pageSize=" + $scope.pageSize
                + "&textToSearch=" + $scope.textSearch.toUpperCase()
                + "&csvClases=" + csvClases,
            {
                headers: { "access_token": authService.getToken() }
            })
            .then(function (result) {
                $scope.dataset = result.data.Result.DataSet;
                console.log(JSON.stringify(result.data.Result.DataSet[0]));
                $scope.totalItems = result.data.Result.DataSet.length;//tbfix
            });
        }
        else {
            $http.get(authService.api() + service + 'BusquedaIdentica?pageNumber=' + $scope.pageNumber
                + "&pageSize=" + $scope.pageSize
                + "&textToSearch=" + $scope.textSearch.toUpperCase()
                + "&csvClases=" + csvClases,
            {
                headers: { "access_token": authService.getToken() }
            })
            .then(function (result) {
                $scope.dataset = result.data.Result.DataSet;
                console.log(JSON.stringify(result.data.Result.DataSet[0]));
                $scope.totalItems = result.data.Result.DataSet.length;//tbfix
            });
        }
    }

}]);


yumKaaxControllers.controller("mpreIngresoCtrl", ["$scope", "$location", "authService", "$routeParams", "$timeout",
                    function ($scope, $location, authService, $routeParams, $timeout) {
                        $scope.userIdentity = authService.getUserInfo();
                        $scope.misExpedientes = [];
                        $scope.totalItems = 0;
                        $scope.pageSize = 10;
                        $scope.expediente = {};
                        $scope.showAlertInfo = "";
                        $scope.alertInfo = false;

                        if ($routeParams.Id) {
                            //getEntity($routeParams.Id);
                        }
                        else {
                            //getResultsPage(1, $scope.pageSize);
                            $scope.misExpedientes = [
                                { Id: '100001', SignoDistintivo: 'Demo 1', TipoDeRegistro: 'Registro Inicial de Marca', EntidadSolicitante: 'Demo', FechaUltimaActualizacion: '05/01/20015' },
                                { Id: '100002', SignoDistintivo: "Demo 2", TipoDeRegistro: "Nombre Comercial", EntidadSolicitante: "Demo", FechaUltimaActualizacion: '05/01/20015' },
                                { Id: '100003', SignoDistintivo: "Demo 3", TipoDeRegistro: "Nombre Comercial", EntidadSolicitante: "Demo", FechaUltimaActualizacion: '05/01/20015' }
                            ];
                        }
                    }]);

//*************************************************
// CONTROLLERS DE PATENTES
//*************************************************
yumKaaxControllers.controller("pexpedienteCtrl", ["$scope", "$location", "authService", "listService", "$http", "$routeParams",
    function ($scope, $location, authService, listService, $http, $routeParams) {
        $scope.userIdentity = authService.getUserInfo();
        $scope.tipoDePatentes = [];
        $scope.model = {};
        $scope.service = '/Patente/';

        $scope.grupos = [];
        $scope.showAlertInfo = "";
        $scope.alertInfo = false;
        $scope.model.EsFavorito = false;

        if ($routeParams.Id) {
            getExpediente('ExpedienteId', '?Id=' + $routeParams.Id);
        }

        function getExpediente(action, queryString) {
            var service = '/Patente/';

            $http.get(authService.api() + service + action + queryString,
            {
                headers: { "access_token": authService.getToken() }
            })
            .then(function (result) {
                $scope.model = result.data.Result.documento;

                for (var i = 0; i < $scope.tipoDePatentes.length; i++) {
                    if ($scope.tipoDePatentes[i].Id == $scope.model.Expediente.TipoDeRegistroId)
                        $scope.model.Expediente.TipoDeRegistroId = $scope.tipoDePatentes[i];
                }

                console.log('Fetched by expediente!!');
            });
        }

        $scope.getExpedienteByNumero = function (tipoDeRegistroId, expediente) {
            getExpediente('Expediente', '?Numero=' + expediente + '&TipoDeRegistroId=' + tipoDeRegistroId.Id);
        }

        $scope.getExpedienteByRegistro = function (tipoDeRegistroId, registro) {
            getExpediente('Registro', '?Registro=' + registro + '&TipoDeRegistroId=' + tipoDeRegistroId.Id);
        }

        function getTipoDeRegistro() {
            listService.reFetchLists();
            var lists = listService.getLists(); // all the lists
            $scope.tipoDePatentes = lists.tipoDePatentes;
        }
        getTipoDeRegistro()
    }]);


yumKaaxControllers.controller("pbusquedadCtrl", ["$scope", "$location", "authService", "$http", "listService", function ($scope, $location, authService, $http, listService) {
    $scope.userIdentity = authService.getUserInfo();
    $scope.pageNumber = 0;
    $scope.pageSize = 0;
    $scope.dataset = [];
    $scope.tipoDePatentes = [];
    
    function getTipoDeRegistro() {
        listService.reFetchLists();
        var lists = listService.getLists(); // all the lists
        $scope.tipoDePatentes = lists.tipoDePatentes;
    }
    getTipoDeRegistro()


    $scope.searchPatentes = function () {
        var service = '/Patente/';

        $scope.getExpedienteUrl = function (e) {
            return "pexpediente/" + e.ExpedienteId;
        };
        var tipoDePatente = null;
        if ($scope.tipoDeRegistroId) {
            tipoDePatente = $scope.tipoDeRegistroId.Id;
        }
        
        $http.get(authService.api() + service + 'BusquedaPatentesDsc?pageNumber=' + $scope.pageNumber
            + "&pageSize=" + $scope.pageSize
            + "&textToSearch=" + $scope.textSearch.toUpperCase()
            + "&tipoDeRegistro=" + tipoDePatente,
        {
            headers: { "access_token": authService.getToken() }
        })
        .then(function (result) {
            $scope.dataset = result.data.Result.DataSet;
            $scope.totalItems = result.data.Result.DataSet.length;//tbfix
        });
    }

}]);


//*************************************************
// FAVORITOS - GOES EVERYWHERE
//*************************************************
yumKaaxControllers.controller("favoritosCtrl", ["$scope", "$location", "authService", "listService", "classSrvc", "$http", "$timeout",
    function ($scope, $location, authService, listService, classSrvc, $http, $timeout) {

        Init();

        function Init() {
            classSrvc.getGrupos()
                .success(function (result) {
                    $scope.grupos = result.Result.DataSet;
                })
                .error(function (error) {
                    $scope.grupos = [{ id: 0, Nombre: 'Todos' }];
                });
        }

        $scope.addFavorito = function addFavorito(expediente, grupoId) {
            $http.post(authService.api() + "/Favoritos/AddFavorito", { expediente: expediente, grupoId: grupoId }, { headers: { "access_token": authService.getToken() } })
            .then(
                function (result) {
                    if (result.data.Succeeded) {
                        $scope.model.EsFavorito = true;
                    }
                    else {
                        $scope.showAlertInfo = true;
                        $scope.alertInfo = "Error agregando expediente a mi listado de expedientes...";
                        $timeout(function () {
                            $scope.showAlertInfo = false;
                            $scope.alertInfo = "";
                        }, 1400);
                    }
                },
                function (error) {
                    $scope.showAlertInfo = true;
                    $scope.alertInfo = "Error agregando expediente a mi listado de expedientes...";
                    $timeout(function () {
                        $scope.showAlertInfo = false;
                        $scope.alertInfo = "";
                    }, 1400);
                }
            );
        }

        $scope.esFavoritoBtn = function esFavoritoBtn() {
            if ($scope.model.EsFavorito)
                return 'btn btn-primary btn-xs dropdown-toggle';
            else
                return 'btn btn-default btn-xs dropdown-toggle';
        }

        $scope.esFavoritoIcon = function esFavoritoIcon() {
            if ($scope.model.EsFavorito)
                return 'glyphicon glyphicon-saved';
            else
                return 'glyphicon glyphicon-pushpin';
        }
    }]);



//*************************************************
// CONTROLLERS DE DERECHO DE AUTOR
//*************************************************
yumKaaxControllers.controller("dexpedienteCtrl", ["$scope", "$location", "authService", "listService", "classSrvc", "$http", "$timeout", "$routeParams",
    function ($scope, $location, authService, listService, classSrvc, $http, $timeout, $routeParams) {
        $scope.userIdentity = authService.getUserInfo();
        $scope.model = {};
        $scope.grupos = [];
        $scope.showAlertInfo = "";
        $scope.alertInfo = false;
        $scope.model.EsFavorito = false;

        if ($routeParams.Id) {
            getExpediente('ExpedienteId', '?Id=' + $routeParams.Id);
        }

        function getExpediente(action, queryString) {
            var service = '/DAutor/';

            $http.get(authService.api() + service + action + queryString,
            {
                headers: { "access_token": authService.getToken() }
            })
            .then(function (result) {
                $scope.model = result.data.Result.documento;
                //console.log(JSON.stringify($scope.opciones));
                console.log('Fetched!!');
            });
        }

        $scope.getExpedienteByNumber = function (expediente) {
            getExpediente('Expediente', '?Numero=' + expediente);
        }

        $scope.getExpedienteByRegistro = function (registro) {
            getExpediente('Registro', '?Registro=' + registro);
        }

    }]);




yumKaaxControllers.controller("gacetaCtrl", ["$scope", "$location", "authService", "$http", function ($scope, $location, authService, $http) {
    $scope.userIdentity = authService.getUserInfo();
    $scope.Publicaciones = [];
    $scope.totalItems = 0;
    $scope.pageSize = 100;
    $scope.showAlertInfo = "";
    $scope.alertInfo = false;

    $scope.pagination = {
        current: 1
    };

    $scope.pageChanged = function (newPage) {
        getResultsPage(newPage, $scope.pageSize);
    };

    $scope.$watch('textSearch', function (newVal, oldVal) {
        if (newVal !== oldVal) {
            if (newVal == '')
                getResultsPage($scope.pageNumber, $scope.pageSize);
            else
                $scope.filterFunction(newVal, $scope.pageNumber, $scope.pageSize);
        }
    }, true);

    $scope.filterFunction = function (element, pageNumber, pageSize) {
        $http.get(authService.api() + '/gaceta/GetPageFilter?filter=' + element + '&page=' + pageNumber + "&pageSize=" + pageSize,
            {
                headers: { "access_token": authService.getToken() }
            }
            )
            .then(function (result) {
                //$scope.Publicaciones = result.data.Result.DataSet;
                setFlag(result.data.Result.DataSet);
                $scope.totalItems = result.data.TotalItems;
                console.log(JSON.stringify(result));
            });
    };

    function getResultsPage(pageNumber, pageSize) {
        if ($scope.textSearch)
            $scope.filterFunction($scope.textSearch, pageNumber, pageSize);
        else
            $http.get(authService.api() + '/gaceta/GetPage?page=' + pageNumber + "&pageSize=" + pageSize,
            {
                headers: { "access_token": authService.getToken() }
            })
            .then(function (result) {

                setFlag(result.data.Result.DataSet);
                $scope.totalItems = result.data.Result.TotalItems;

            });
    }

    function setFlag(publicaciones) {
        for (var i = 0; i < publicaciones.length; i++) {
            if (publicaciones[i].logotipo.toLowerCase().indexOf("mp3") > -1)
                publicaciones[i].format = 'mp3';
            else
                publicaciones[i].format = '';
        }
        $scope.Publicaciones = publicaciones;
        console.log(JSON.stringify($scope.Publicaciones));
    }

    getResultsPage(1, $scope.pageSize);
}]);


yumKaaxControllers.controller("gacetaSemanalCtrl", ["$scope", "$location", "authService", "$http", function ($scope, $location, authService, $http) {
    $scope.userIdentity = authService.getUserInfo();

    $scope.fechaSelected = new Date();

    //calendar flag controllers
    $scope.fs = {};

    // Disable weekend selection
    $scope.disabled = function (date, mode) {
        return (mode === 'day' && (date.getDay() === 10 || date.getDay() === 6));
    };

    $scope.minDate = new Date(2012, 1, 1);
    $scope.maxDate = new Date(2016, 1, 1);
    $scope.open = function ($event, wtf) {
        $event.preventDefault();
        $event.stopPropagation();
        if (wtf == 'fs')
            $scope.fs.open = true
    };

    //calendar options
    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };

    function fixDate(x) {
        var date = new Date();
        if (x)
            date = new Date(parseInt(x.substr(6)));
        return date;
    };

    //calendar date format
    $scope.formats = ['MM/dd/yyyy hh:mm:ss', 'dd-MM-yyyy', 'dd/MM/yyyy', 'shortDate'];
    $scope.format = $scope.formats[2];
}]);


yumKaaxControllers.controller("misexpedientesCtrl", ["$scope", "$location", "authService", "$http", "classSrvc", function ($scope, $location, authService, $http, classSrvc) {
    $scope.userIdentity = authService.getUserInfo();
    $scope.grupos = [];
    $scope.misExpedientes = [];
    $scope.totalItems = 0;
    $scope.pageSize = 50;
    $scope.currentGrupo = null;

    Init();

    function Init() {
        classSrvc.getGrupos()
            .success(function (result) {
                $scope.grupos = result.Result.DataSet;
            })
            .error(function (error) {
                $scope.grupos = [{ id: 0, Nombre: 'Todos' }];
            });
        getExpedientes(null, 1, $scope.pageSize);
    }

    $scope.getExpedienteUrl = function (e) {

        if (e.ModuloId == 1) {
            return "mexpediente/" + e.ExpedienteId;
        }
        else if (e.ModuloId == 2) {
            return "pexpediente/" + e.ExpedienteId;
        }
        else if (e.ModuloId == 3) {
            return "dexpediente/" + e.ExpedienteId;
        }
        return "mexpediente"
    };

    $scope.pageChanged = function (newPage) {
        getExpedientes($scope.currentGrupo, newPage, $scope.pageSize);
    };

    $scope.getExpedientes = function (grupo) {
        getExpedientes(grupo, 1, $scope.pageSize);
    }

    function getExpedientes(grupo, pageNumber, pageSize) {
        $scope.currentGrupo = grupo;

        if ($scope.currentGrupo) {
            $http.get(authService.api() + '/Favoritos/GetFavoritosPageFilter?idGrupoFilter=' + $scope.currentGrupo + '&page=' + pageNumber + "&pageSize=" + pageSize,
            {
                headers: { "access_token": authService.getToken() }
            })
            .then(function (result) {
                $scope.misExpedientes = result.data.Result.DataSet;
                $scope.totalItems = result.data.Result.TotalItems;
            });
        }
        else {
            $http.get(authService.api() + '/Favoritos/GetFavoritosPageFilter?idGrupoFilter=0&page=' + pageNumber + "&pageSize=" + pageSize,
            {
                headers: { "access_token": authService.getToken() }
            })
            .then(function (result) {

                $scope.misExpedientes = result.data.Result.DataSet;
                $scope.totalItems = result.data.Result.TotalItems;
            });
        }

    }

}]);


yumKaaxControllers.controller("UsuariosPublicosCtrlr", ["$scope", "$location", "$routeParams", "authService", "$http", "classSrvc", function ($scope, $location, $routeParams, authService, $http, classSrvc) {
    $scope.usuario = {}
    $scope.paises = [];


    if ($routeParams.miClave && $routeParams.Id) {
        loadPerfil()
    }

    function getPaises() {
        $http.get(authService.api() + '/Admin/Entities/Get?entity=paises',
        {
            headers: { "access_token": authService.getToken() }
        })
        .then(function (result) {
            $scope.paises = result.data.Result;
        });
    };

    function loadPerfil() { // the token 
        var service = "/Admin/UsuariosPublicos/";
        getPaises();
        $http.get(authService.api() + service + 'LoadMyPerfil',
        {
            headers: { "access_token": authService.getToken() }
        })
        .then(function (result) {
            $scope.usuario = result.data.Result;
            console.log(result.data.Result);
        });

    }

    loadPerfil();

    $scope.isUnchanged = function (usuario) {
        return angular.equals(usuario, $scope.master);
    };

    $scope.update = function (usuario) {
        $http.post(authService.api() + "/Admin/UsuariosPublicos/save", { model: usuario }, { headers: { "access_token": authService.getToken() } })
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

        $http.post(authService.api() + "/Admin/UsuariosPublicos/ResetPW", { model: usuario }, { headers: { "access_token": authService.getToken() } })
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

yumKaaxControllers.controller("genericCtrl", ["$scope", "$location", "authService", "$http", "classSrvc", function ($scope, $location, authService, $http, classSrvc) {
}]);

