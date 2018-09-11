Array.prototype.remove = function () {
    var what, a = arguments, L = a.length, ax;
    while (L && this.length) {
        what = a[--L];
        while ((ax = this.indexOf(what)) !== -1) {
            this.splice(ax, 1);
        }
    }
    return this;
};


///---------------
/// Patente
///---------------
yumKaaxControllers.controller("AnotacionesCtrl", ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "listService", "authService", "$location", "dateHelper", "$uibModal", "rpiServices",
function ($scope, $http, $q, $routeParams, $timeout, $route, listService, authService, $location, dateHelper, $uibModal, rpiServices) {
    $scope.config = { _expediente: "./Spa/Views/eAnota/AnotaExpediente.html", _modulo: "Anotaciones" };
    $scope.$route = $route;
    $scope.opciones = listService.getAnotacionesmnu();
    $scope.tipoDePatentes = [];
    $scope.paises = [];
    $scope.model = { Expediente: { Id: -1, TipoDeRegistroId: 0, Numero: '', FechaDeSolicitud: undefined }, Patente: { Descripcion: '' }, Titulares : [] };
    $scope.titular = { Id: -1, Direccion: '', PaisId: -1 };
    $scope.Estatuses = [];
    $scope.classPat = [];
    $scope.dateHelper = dateHelper;
    $scope.appPopupBox = new appPopupBox($scope);

    /*SYSTEM*/
    $scope.showAlertInfo = false;
    $scope.alertInfo = "";
    $scope.res = "";
    $scope.evento = "";
    $scope._resolucion = { FechaResolucion: new Date() };
    $scope.Estatuses = [];
    $scope.user = authService.getUserInfo();
    /*SYSTEM*/

    function getListas()
    {
        var lists = listService.getLists();
        console.log(lists);
        $scope.tipoDeRegistroDeMarcas = lists.tipoDeRegistroDeMarcas;
        $scope.tipoDeAnotaciones = lists.tipoDeAnotaciones;
        $scope.paises = lists.paises;
        $scope.classPat = listService.getClassificacionPat();
    }

    function getEstatus() {
        $http.get(authService.api() + '/Admin/Estatus/GetPage?page=1&pageSize=200&moduloId=4') //status for multiple areas
        .then(function (result) {
            $scope.Estatuses = result.data.Result.DataSet;
        });
    }

    function _ctrlInit() {
        getListas();
        getEstatus();
    }

    function fixModelDates() {
        $scope.model.Expediente.FechaDeSolicitud = dateHelper.parseStrDate($scope.model.Expediente.FechaDeSolicitud);
        $scope.model.Expediente.FechaDeEstatus = dateHelper.parseStrDate($scope.model.Expediente.FechaDeEstatus);
        for (var i = 0; i < $scope.model.Cronologia.length; i++) {
            $scope.model.Cronologia[i].Fecha = dateHelper.parseStrDate($scope.model.Cronologia[i].Fecha);
        }
        for (var i = 0; i < $scope.model.ExpedientesConAnotacion.length; i++) {
            $scope.model.ExpedientesConAnotacion[i].FechaActualizacion = dateHelper.parseStrDate($scope.model.ExpedientesConAnotacion[i].FechaActualizacion);
            $scope.model.ExpedientesConAnotacion[i].FechaDeRegistro = dateHelper.parseStrDate($scope.model.ExpedientesConAnotacion[i].FechaDeRegistro);
            $scope.model.ExpedientesConAnotacion[i].FechaDeRegistroEnMarcas = dateHelper.parseStrDate($scope.model.ExpedientesConAnotacion[i].FechaDeRegistroEnMarcas);
        }
        
    }

    function getDocto(tipoDeRegistro, expediente) {        
        rpiServices.anota.getExpedienteDeAnotacionByNumero(tipoDeRegistro, expediente)
        .then(
            function (result) {
                console.log(result);
                $scope.model = result.data.Result.documento;
                fixModelDates();
                console.log($scope.model);
                console.log($scope.model.Expediente.FechaDeSolicitud);
                console.log('Expediente de anotaciones !!');
            },
            function (error) {
                $scope.appPopupBox.handleError(error, 'Expediente no encontrado - ' + ($routeParams.numero ? $routeParams.numero : ""))
                var docType = $scope.model.Expediente.TipoDeRegistroId;
                $scope.model = { Expediente: { TipoDeRegistroId: docType } };
            }
        );
    }
    $scope.enExpediente = {};

    $scope.getExpedienteDeMarcas = function () {
        rpiServices.marcas.getExpedienteByRegistro($scope.enExpediente.TipoDeRegistroId, $scope.enExpediente.Registro, $scope.enExpediente.Raya)
        .then(
            function (result) {
                console.log(result);
                $scope.enExpediente.AnotacionEnExpedienteId = result.data.Result.documento.Expediente.Id;
                $scope.enExpediente.Denominacion = result.data.Result.documento.Marca.Denominacion;

                if (result.data.Result.documento.Titulares.length > 0)
                    $scope.enExpediente.NombreTitular = result.data.Result.documento.Titulares[0].Nombre;

                $scope.enExpediente.FechaDeRegistro = fixDate(result.data.Result.documento.Expediente.FechaDeEstatus);

                $scope.enExpediente.ClassificacionDeNiza = result.data.Result.documento.Marca.ClassificacionDeNiza;

                console.log('expediente by registro');
            },
            function (error) {
                $scope.appPopupBox.handleError(error, 'Expediente no encontrado - ' + ($routeParams.numero ? $routeParams.numero : ""))
            }
        );
    }

    function refreshExpediente() {
        $scope.getExpediente($scope.model.Expediente.TipoDeRegistroId, $scope.model.Expediente.Numero);
        $scope._resolucion = { FechaResolucion: new Date() };
        //showLeftBox();        
        $scope.res = './Spa/Views/empty.html'
    }


    //function getEntityById(Id) {
    //    getDocto('ExpedienteId', '?Id=' + Id);
    //};

    $scope.getExpediente2 = function getEntityByNumero(tipoDeRegistro, expediente) {
        if (!$routeParams.numero) {
            $location.path("/Anotaciones/" + expediente + "/" + tipoDeRegistro);
        }
        else
        {
            $scope.model.Expediente.TipoDeRegistroId = 'number:'+tipoDeRegistro;
            $scope.model.Expediente.Numero = expediente;
            getDocto(tipoDeRegistro, expediente);
            $location.path("/Anotaciones/" + expediente + "/" + tipoDeRegistro, false);
        }
    };
    
    $scope.closeModal = function () {
        $uibModalInstance.dismiss('cancel');
    }

    $scope.openTest = function () {
        $uibModal.open({
            size: 'lg',
            templateUrl: './Spa/Views/eAnota/ExpedienteConAnotacion.html',
            scope: $scope
        })
    };

    $scope.getNew = function () {
        $scope.model = { Expediente: { Id: -1, TipoDeRegistroId: 0, Numero: '', FechaDeSolicitud: undefined }, Anotacion: {}, ExpedientesConAnotacion: [] };
    };

    $scope.titular = { id: 0, Direccion: '', PaisId: 0, Nombre:'' };
    $scope.onSelectTitular = function ($item, $model, $label) {
        $scope.titular.Id = $item.Id;
        $scope.titular.PaisId = $item.PaisId;
    }

    $scope.searchTitulares = function (textToSearch) {
        var service = '/PatTitular/Search';        
        return $http.get(authService.api() + service + '?textToSearch='+textToSearch)
        .then (
            function (result) { return result.data.Result.DataSet; },
            function (error) { return [];}
        );
    }

    $scope.onSelectAgente = function ($item, $model, $label) {
        $scope.model.Patente.AgenteId = $item.Id;
        $scope.model.Agente = $item.Nombre;
    }

    $scope.searchAgentes = function (textToSearch) {
        console.log('search agentes');
        var service = '/Admin/Agentes/Search';
        console.log(authService.api() + service + '?textToSearch=' + textToSearch);
        return $http.get(authService.api() + service + '?textToSearch=' + textToSearch)
        .then(
            function (result) { return result.data.Result.DataSet; },
            function (error) { console.log('error search agentes'); return []; }
        );
    }

    $scope.agregarAnotacionEnRegistro = function (frm) {
        var service = '/Anotacion/';

        $http.post(authService.api() + service + "enExpediente", { model: $scope.model })
            .then(
                function (result) {
                    $scope.model = result.data.Result.documento;
                    fixModelDates();
                    $scope.appPopupBox.showAlertInfo('Solicitud Actualizada...');
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }

    $scope.saveSolicitud = function (frmSol) {
        var service = '/Patente/';
        if (!frmSol.$valid) 
            return;

        $http.post(authService.api() + service + "SaveSolicitud", { model: $scope.model })
            .then(
                function (result) {
                    $scope.model = result.data.Result.documento;
                    fixModelDates();
                    $scope.appPopupBox.showAlertInfo('Solicitud Actualizada...');
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }

   


    $scope.ResolucionCustomizada = function (res) {
        var jsondoc = JSON.stringify(res);

        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            EstatusId: res.EstatusId.Id,
            Titulo : res.Titulo,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        }

        createResolucion("/anotacion/ResolucionCustomizada", resolucion );
    }

    function createResolucion(service, resolucion) {
        $http.post(authService.api() + service, { tramite: resolucion })
            .then(
                function (result) {
                    listService.printDoc(result);
                    refreshExpediente();
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }

    $scope.Notificacion = function (res) {
        var jsondoc = JSON.stringify(res);

        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: res.FechaResolucion,
            JSONDOC: jsondoc,
            HTMLDOC: res.htmlResol
        }

        createResolucion("/anotacion/Notificacion", resolucion );

    }    


    /*********************
    SYSTEM
    **********************/
    //calendar flag controllers
    $scope.fds = {};
    $scope.ppf = {};
    $scope.saf = {};
    $scope.fresolucion = {};
    $scope.fmem = {};
    $scope.fnoti = {};
    $scope.fecha2 = {};
    $scope.fecha3 = {};
    $scope.fref = {};
    $scope.fa = {};
    $scope.fv = {};
    $scope.fr = {};
    $scope.fpct = {};    

    $scope.orightml = '<h2>Titulo del contenido de Resolución</h2><p>Este es un ejemplo de texto o guia para escribir una resolucion customizada</p>';
    $scope._resolucion.htmlResol = $scope.orightml;
    //$scope.disabled = false;

    // Disable weekend selection
    $scope.WeekendDisabled = function (date, mode) {
        return (mode === 'day' && (date.getDay() === 10 || date.getDay() === 6));
    };

    $scope.minDate = new Date(2012, 1, 1);
    $scope.maxDate = new Date(2016, 1, 1);
    $scope.open = function ($event, wtf) {
        $event.preventDefault();
        $event.stopPropagation();
        if (wtf == 'fds')
            $scope.fds.open = true
        else if (wtf == 'ppf')
            $scope.ppf.open = true
        else if (wtf == 'saf')
            $scope.saf.open = true
        else if (wtf == 'fresolucion')
            $scope.fresolucion.open = true
        else if (wtf == 'fmem')
            $scope.fmem.open = true
        else if (wtf == 'fnoti')
            $scope.fnoti.open = true
        else if (wtf == 'fecha2')
            $scope.fecha2.open = true
        else if (wtf == 'fecha3')
            $scope.fecha3.open = true
        else if (wtf == 'fref')
            $scope.fref.open = true
        else if (wtf == 'fa')
            $scope.fa.open = true
        else if (wtf == 'fv')
            $scope.fv.open = true
        else if (wtf == 'fr')
            $scope.fr.open = true
        else if (wtf == 'fpct')
            $scope.fpct.open = true
        
    };

    //calendar options
    $scope.dateOptions = {
        formatYear: 'yy',
        startingDay: 1
    };

    function fixDate(x) {
        var date = new Date(0);
        if (x)
            date = new Date(parseInt(x.substr(6)));
        return date;
    };

    //calendar date format
    $scope.formats = ['MM/dd/yyyy hh:mm:ss', 'dd-MM-yyyy', 'dd.MM.yyyy', 'shortDate'];
    $scope.format = $scope.formats[1];

    $scope.isMnuEnabled = function (opcion) {
        var _disabled = "";
        try {
            if (opcion._estatus.length == 0) {
                _disabled = "";
            }
            else {
                //Uncomment to validate status on each option
                //_disabled = (opcion._estatus.indexOf($scope.model.Expediente.EstatusId) == -1) ? "disabled" : "";
            }
        }
        catch (err) {
            _disabled = "disabled";
        }
        return _disabled;
    }

    $scope.Resolucion = function (opcion) {
        //console.log('opcion:' + opcion)
        var view = opcion.view;
        console.log(view);
        //Uncomment to validate status on each option
        //if (opcion._estatus.length != 0 && (view == '' || opcion._estatus.indexOf($scope.model.Expediente.EstatusId) == -1)) {
        if (!true){
            $scope.res = './Spa/Views/empty.html'
            $scope.$apply();
        }
        else {
            $scope.res = './Spa/Views/eAnota/' + view;
            $scope.evento = opcion.opcion;
        }
        $scope._resolucion = { resId: "", FechaResolucion: new Date() };
    }

    $scope.rollbackResolution = function () {
        if ($routeParams.numero)
            $location.path("/Anotaciones/" + $routeParams.numero + "/" + $routeParams.tipo);
        else
            $location.path("/Anotaciones");
        $scope.Resolucion({ _estatus: [0] });//reset resolution
        showLeftBox();
    };

    _ctrlInit();
    if ($routeParams.numero && $routeParams.tipo) {
        $scope.getExpediente2($routeParams.tipo, $routeParams.numero);
    }
    //else if ($routeParams.numero) {
    //    getEntityById($routeParams.numero);
    //}
}]); 
