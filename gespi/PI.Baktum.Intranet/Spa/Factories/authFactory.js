/*
    Intranet Factories
*/

angular.module('authFactory', [])
    .factory('authService', function ($http, $q, $window) {
        var userIdentity;
        var urlBase = '/eRegistroAPI/Auth/';
        function login(userName, password) {
            var deferred = $q.defer();

            $http.post(urlBase + "Login", { userName: userName, password: password })
                .then(                    
                    function (result) {
                        if (result.data.Succeeded) {
                            userIdentity = {
                                accessToken: result.data.Result.token,
                                userName: result.data.Result.Nombre,
                                email: result.data.Result.Email
                            };

                            $.jrzStorage.local.setItem("appUser", userIdentity);
                            deferred.resolve(userIdentity);
                        }
                        else {
                            deferred.reject("-Cuenta no existe en el sistema-");
                        }
                    },
                    function (error) {
                        deferred.reject(error);
                    }
                );
            return deferred.promise;
        }

        function logout() {
            var deferred = $q.defer();

            $http({
                method: "POST",
                url: "/eRegistroAPI/Auth/logout",
                headers: {
                    "access_token": userIdentity.accessToken
                }
            }).then(function (result) {
                userIdentity = null;
                $.jrzStorage.local.removeItem("appUser");
                deferred.resolve(result);
            }, function (error) {
                deferred.reject(error);
            });

            return deferred.promise;
        }

        function getUserInfo() {
            return userIdentity;
        }

        function getToken() {
            return userIdentity.accessToken;
        }

        function api() {
            return '/eRegistroAPI';
        }

        function init() {
            if ($.jrzStorage.local.exists("appUser")) {
                userIdentity = $.jrzStorage.local.getItem("appUser");
            }
        }
        init();

        return {
            login: login,
            logout: logout,
            getUserInfo: getUserInfo,
            getToken: getToken,
            api: api
        };
    })
    .factory('listService', function ($http, $q, authService) {
        var lists;
        var classPat;

        function fetchLists() {
            var deferred = $q.defer();
            var urlBase = '/eRegistroAPI/Admin/Entities/';
            console.log('get lists...');
            console.log(urlBase + 'Get?entity=all');
            $http.get(urlBase + 'Get?entity=all')
                .then
                (
                    function (result) {                        
                        lists = result.data.Result;
                        $.jrzStorage.local.setItem("lists", lists)                        
                        deferred.resolve(lists);
                    },
                    function (error) {
                        deferred.reject(error);
                    }
                );
            return deferred.promise;
        }

        function fetchClassificacionPat() {
            var urlBase = '/eRegistroAPI/Admin/Entities/';

            if ($.jrzStorage.local.exists("classPat")) {
                classPat = $.jrzStorage.local.getItem("classPat");
                return classPat;
            }
            else {
                console.log(urlBase + 'GetClassificacionPat');
                $http.get(urlBase + 'GetClassificacionPat')
                    .then(
                        function (result) {
                            classPat = result.data.Result;
                            $.jrzStorage.local.setItem("classPat", classPat)
                            return classPat;
                        },
                        function (error) {
                            return [];
                        }
                    );
            }
            return classPat;
        }

        function getLists() {
            return lists;
        }

        function getMes(nMes) {
            var aMeses = new Array();
            aMeses[0] = "ENERO"; aMeses[1] = "FEBRERO"; aMeses[2] = "MARZO"; aMeses[3] = "ABRIL"; aMeses[4] = "MAYO"; aMeses[5] = "JUNIO"; aMeses[6] = "JULIO"; aMeses[7] = "AGOSTO"; aMeses[8] = "SEPTIEMBRE"; aMeses[9] = "OCTUBRE"; aMeses[10] = "NOVIEMBRE"; aMeses[11] = "DICIEMBRE";
            return aMeses[nMes];
        }

        function getEnLetras(nNumero) {
            var aletras = new Array();
            var aletras2 = new Array();
            aletras[0] = "";
            aletras[1] = "CERO"; aletras[2] = "UNO"; aletras[3] = "DOS"; aletras[4] = "TRES"; aletras[5] = "CUATRO";
            aletras[6] = "CINCO"; aletras[7] = "SEIS"; aletras[8] = "SIETE"; aletras[9] = "OCHO"; aletras[10] = "NUEVE"; aletras[11] = "DIEZ";
            aletras[12] = "ONCE"; aletras[13] = "DOCE"; aletras[14] = "TRECE"; aletras[15] = "CATORCE"; aletras[16] = "QUINCE"; 
            aletras2[0] = ""; aletras2[1] = "DIEZ"; aletras2[2] = "VEINTE"; aletras2[3] = "TREINTA"; aletras2[4] = "CUARENTA"; aletras2[5] = "CINCUENTA"; aletras2[6] = "SESENTA"; aletras2[7] = "SETENTA"; aletras2[8] = "OCHENTA"; aletras2[9] = "NOVENTA";
            var enLetras = "";
            var nY_N = 0;

            if (nNumero < 16) {
                enLetras = aletras[nNumero + 1];
            }
            else {
                nY_N = nNumero % 10;
                if (nY_N == 0)
                    enLetras = aletras2[(nNumero - nY_N) / 10];
                else
                    enLetras = aletras2[(nNumero - nY_N) / 10] + " Y " + aletras[nY_N + 1];
            }

            return enLetras;
        }

        function getGrlPerms() {
            return [
                { _id: "MAR999", opcion: "Ingresar Expediente Marcas" },
                { _id: "MAR998", opcion: "Actualizar Expediente Marcas" },
                { _id: "PAT997", opcion: "Ingresar Expediente Patentes" },
                { _id: "PAT996", opcion: "Actualizar Expediente Patentes" },
                { _id: "DA995", opcion: "Ingresar Expediente Derecho de Autor" },
                { _id: "DA994", opcion: "Actualizar Expediente Derecho de Autor" }
            ];
        }

        function getDAmnu() {
            return [
                { _id: "DA001", opcion: "Se declara con lugar",         view: "130conlugar.html", _estatus: [], migra: [], stl: "tile-blue" },//130, 138
                { _id: "DA002", opcion: "Rechazo",                      view: "130DARechazo.html", _estatus: [130], migra: [], stl: "tile-red" },
                { _id: "DA003", opcion: "Suspenso",                     view: "130DAsuspenso.html", _estatus: [130], migra: [], stl: "tile-yellow" },
                { _id: "DA004", opcion: "Operar Memorial",              view: "131OperarMemorial.html", _estatus: [131, 133], migra: [], stl: "tile-green" },
                { _id: "DA005", opcion: "Levantar Suspenso",            view: "133LevantarSuspenso.html", _estatus: [133], migra: [], stl: "tile-greenDark" },
                { _id: "DA006", opcion: "Recurso de Revocatoria",       view: "135RecursoRevocatoria.html", _estatus: [135], migra: [], stl: "tile-green" },
                { _id: "DA007", opcion: "Elevando Recurso Revocatoria", view: "136ElevandoRecurso.html", _estatus: [136], migra: [], stl: "tile-yellow" },
                { _id: "DA008R", opcion: "Por Recibido MINECO-Registro", view: "137PorRecibidoMINECOR.html", _estatus: [137], migra: [], stl: "tile-greenDark" },
                { _id: "DA008A", opcion: "Por Recibido MINECO-Archivo",  view: "137PorRecibidoMINECOA.html", _estatus: [137], migra: [], stl: "tile-greenDark" },
                { _id: "DA009", opcion: "Notificar",                    view: "140Notificar.html", _estatus: [140], migra: [], stl: "tile-green" },
                { _id: "DA010", opcion: "Emitir Titulo (Registro)",     view: "140EmitirTitulo.html", _estatus: [140, 141], migra: [], stl: "tile-blue" },
                { _id: "DA011", opcion: "Cambiar Estatus",              view: "00CambiarEstatus.html", _estatus: [], migra: [], stl: "tile-red" },
                { _id: "DA012", opcion: "Reposicion de Titulo",         view: "135ReposiciondeTitulo.html", _estatus: [135], migra: [], stl: "tile-orange" }
            ];
        }

        function getPatentesmnu() {
            return [
                { _id: "PAT001", opcion: "Requerimiento de examen de forma",    view: "Requerimiento_De_Examen_De_Forma.html", _estatus: [59], migra: [], stl: "tile-yellow" },
                { _id: "PAT002", opcion: "Gestor Oficioso",                     view: "Gestor_Oficioso.html", _estatus: [], migra: [], stl: "tile-blue" },
                { _id: "PAT003", opcion: "Admision Trámite",                    view: "Admision_Tramite.html", _estatus: [59, 60, 63], migra: [], stl: "tile-blue" },
                { _id: "PAT004", opcion: "Edicto",                              view: "Edicto.html", _estatus: [64, 116], migra: [], stl: "tile-blue" },
                { _id: "PAT005", opcion: "Publicaciones",                       view: "Publicaciones.html", _estatus: [66], migra: [], stl: "tile-green" },
                { _id: "PAT006", opcion: "Admite observaciones",                view: "Admite_Observaciones.html", _estatus: [67], migra: [], stl: "tile-yellow" },
                { _id: "PAT007", opcion: "Orden de Pago",                       view: "Orden_De_Pago.html", _estatus: [67, 106, 125, 126], migra: [], stl: "tile-greenDark" },
                { _id: "PAT008", opcion: "Pago Examen",                         view: "Pago_Examen.html", _estatus: [68], migra: [], stl: "tile-greenDark" },
                { _id: "PAT009", opcion: "Requerimiento examen de fondo A",     view: "Requerimiento_Examen_De_Fondo_A.html", _estatus: [97, 101], migra: [], stl: "tile-orange" },
                { _id: "PAT010", opcion: "Requerimiento examen de fondo B",     view: "Requerimiento_Examen_de_Fondo_B.html", _estatus: [97, 101], migra: [], stl: "tile-orange" },
                { _id: "PAT011", opcion: "Reporte de exámen de fondo",  view: "Reporte_De_Examen_De_Fondo.html", _estatus: [], migra: [], stl: "tile-greenDark" },
                { _id: "PAT012", opcion: "Informe de Busqueda",         view: "Informe_de_Busqueda.html", _estatus: [], migra: [], stl: "tile-yellow" },
                { _id: "PAT013", opcion: "Razón de Abandono",           view: "Razon_De_Abandono.html", _estatus: [], migra: [], stl: "tile-orange" },
                { _id: "PAT014", opcion: "Desistimiento Solicitud ",    view: "Desistimiento_Solicitud.html", _estatus: [], migra: [], stl: "tile-red" },
                { _id: "PAT015", opcion: "Titulo",                      view: "Titulo.html", _estatus: [70, 75, 81, 104], migra: [], stl: "tile-blue" },
                { _id: "PAT016", opcion: "Notificacion",                view: "Notificacion.html", _estatus: [61, 62, 64, 71, 72, 76, 77, 78, 82, 95, 98, 99, 106, 116], migra: [], stl: "tile-green" },
                { _id: "PAT017", opcion: "Traspaso",                    view: "Traspaso.html", _estatus: [], migra: [], stl: "tile-blue" },
                { _id: "PAT018", opcion: "Cambio de Nombre",            view: "Cambio_De_Nombre.html", _estatus: [], migra: [], stl: "tile-yellow" },
                { _id: "PAT019", opcion: "Titulo Renovación ",          view: "Titulo_Renovacion.html", _estatus: [88], migra: [], stl: "tile-greenDark" },
                { _id: "PAT020", opcion: "Reposición Titulo Patente ",  view: "Reposicion_Titulo_Patente.html", _estatus: [88], migra: [], stl: "tile-blue" },
                { _id: "PAT021", opcion: "Certificaciones",             view: "Certificaciones.html", _estatus: [88], migra: [], stl: "tile-greenDark" },
                { _id: "PAT022", opcion: "Cambiar Estatus",             view: "PAT022.html", _estatus: [], migra: [], stl: "tile-red" },
                { _id: "PAT023", opcion: "Resolución Customizada",      view: "PAT023.html", _estatus: [], migra: [], stl: "tile-blue" }
            ];
        }

        function getMarcasmnu() {
            return [
                {
                    _id: "MAR100", opcion: "Examen de Forma", _estatus: [], menu: [
                        { _id: "MAR101", opcion: "Gestor Oficioso",  view: "Gestor_Oficioso.html", _estatus: [7], migra: [], stl: "tile-yellow" },
                        { _id: "MAR102", opcion: "Requerimientos",   view: "Requerimientos.html", _estatus: [7], migra: [], stl: "tile-blue" },
                        { _id: "MAR103", opcion: "Objeciones Forma", view: "Objeciones_Forma.html", _estatus: [7], migra: [], stl: "tile-blue" }
                    ]
                },
                {
                    _id: "MAR200", opcion: "Examen Novedad", _estatus: [], menu: [
                        { _id: "MAR201", opcion: "Objeciones Fondo", view: "Objeciones_Fondo.html", _estatus: [7], migra: [], stl: "tile-yellow" }
                    ]
                },                
                { _id: "MAR400", opcion: "Edictos",             view: "Edictos.html", _estatus: [28], migra: [], stl: "tile-blue" },
                //{ _id: "MAR500", opcion: "Publicación",                 view: "MAR500.html", _estatus: [], migra: [], stl: "tile-blue" },
                { _id: "MAR600", opcion: "Orden de Pago",       view: "Orden_de_Pago.html", _estatus: [31], migra: [], stl: "tile-blue" },
                { _id: "MAR700", opcion: "Titulo",              view: "Titulo.html", _estatus: [33], migra: [], stl: "tile-blue" },
                {
                    _id: "MAR800",       opcion: "Resoluciones", _estatus: [], menu: [
                        { _id: "MAR801", opcion: "Reposición de edicto",        view: "Reposicion_de_edicto.html", _estatus: [], migra: [], stl: "tile-green" },
                        { _id: "MAR802", opcion: "Enmienda",                    view: "Enmienda.html", _estatus: [], migra: [], stl: "tile-green" },
                        { _id: "MAR803", opcion: "Cancelación",                 view: "Cancelacion.html", _estatus: [], migra: [], stl: "tile-green" },
                        { _id: "MAR804", opcion: "Traspaso",                    view: "Traspaso.html", _estatus: [], migra: [], stl: "tile-green" },
                        { _id: "MAR805", opcion: "División Registro",           view: "Division_Registro.html", _estatus: [], migra: [], stl: "tile-green" },
                        { _id: "MAR806", opcion: "Abandono",                    view: "Abandono.html", _estatus: [], migra: [], stl: "tile-green" },
                        { _id: "MAR807", opcion: "Desistimiento",               view: "Desistimiento.html", _estatus: [], migra: [], stl: "tile-green" },
                        { _id: "MAR808", opcion: "Rechazo por objecion",        view: "Rechazo_por_objecion.html", _estatus: [], migra: [], stl: "tile-green" },
                        { _id: "MAR809", opcion: "Revocatoria de Oficio",       view: "Revocatoria_de_Oficio.html", _estatus: [], migra: [], stl: "tile-green" },
                        { _id: "MAR810", opcion: "Resolución Customizada",      view: "Resolucion_Customizada.html", _estatus: [], migra: [], stl: "tile-green" },
                        { _id: "MAR811", opcion: "Errores Materiales",          view: "Errores_Materiales.html", _estatus: [], migra: [], stl: "tile-blue" }
                    ]
                },
                { _id: "MAR900", opcion: "Cambiar Estatus", view: "Cambiar_Estatus.html", _estatus: [], migra: [], stl: "tile-red" },
                { _id: "MAR901", opcion: "Notificaciones", view: "Notificaciones.html", _estatus: [], migra: [], stl: "tile-red" },
            ];
        }

        function getAnotacionesmnu() {
            return [
                { _id: "ANOTA100", opcion: "Resolución Customizada", view: "Resolucion_Customizada.html", _estatus: [], migra: [], stl: "tile-red" },
                { _id: "ANOTA200", opcion: "Notificaciones", view: "Notificaciones.html", _estatus: [], migra: [], stl: "tile-red" }
            ];
        }

        function printDoc(result) {

            var popupWin = window.open('', '_blank');
            popupWin.window.focus();
            popupWin.document.write(result.data);
            popupWin.focus();

            function printDoc() {
                popupWin.print();
            }
            window.setTimeout(printDoc, 500);

/*
            if (navigator.userAgent.toLowerCase().indexOf('chrome') > -1) {
                var popupWin = window.open('', '_blank');
                popupWin.window.focus();
                popupWin.document.write(result.data);
                popupWin.onbeforeunload = function (event) {
                    popupWin.close();
                    return '.\n';
                };
                popupWin.onabort = function (event) {
                    popupWin.document.close();
                    popupWin.close();
                }
            }
            else {
                var popupWin = window.open('', '_blank');
                popupWin.document.open();
                popupWin.document.write(result.data);
                popupWin.document.print
            }
            popupWin.print();
            popupWin.document.close();
*/
        }

        function reFetchLists() {
            fetchLists();
            return lists;
        }

        function init() {
            fetchClassificacionPat();
            if ($.jrzStorage.local.exists("lists")) {
                lists = $.jrzStorage.local.getItem("lists");
            }
            else {
                fetchLists();
            }
        }

        init();

        return {
            getLists: getLists,
            reFetchLists: reFetchLists,
            getMes: getMes,
            getEnLetras: getEnLetras,
            getDAmnu: getDAmnu,
            getPatentesmnu: getPatentesmnu,
            getGrlPerms: getGrlPerms,
            getMarcasmnu: getMarcasmnu,
            getAnotacionesmnu: getAnotacionesmnu,
            printDoc: printDoc,
            getClassificacionPat: fetchClassificacionPat
        };
    })

.factory('sessionInjector', ['$q', function ($q) {

    var userIdentity = { _id: 1010101010101010101 };

    function _init() {
        if ($.jrzStorage.local.exists("appUser")) { //_g3sp1_1d3nt1ty
            userIdentity = $.jrzStorage.local.getItem("appUser");
        }
    }

    return {
        'request': function (config) {
            _init();
            $('#appSpinner').show();
            config.headers['access_token'] = '';
            
            if (userIdentity.accessToken) {
                config.headers['access_token'] = userIdentity.accessToken;
            }

            return config;
        },

        // optional method
        'requestError': function (rejection) {
            console.log('REJECTION:');
            console.log(rejection);
            // do something on error
            $('#appSpinner').hide();
            /* if (canRecover(rejection)) {
                 return responseOrNewPromise
             }*/
            return $q.reject(rejection);
        },

        'response': function (response) {
            $('#circular-spinning').hide();
            return response;
        },

        // optional method
        'responseError': function (rejection) {
            // do something on error
            $('#appSpinner').hide();
            /*if (canRecover(rejection)) {
                return responseOrNewPromise
            }*/
            return $q.reject(rejection);
        }
    };

}])
.factory('dateHelper', ['uibDateParser', function (uibDateParser) {
    // Disable weekend selection

    var _sysdate = {};

    _sysdate.disabled = function (date, mode) {
        return (mode === 'day' && (date.getDay() === 10 || date.getDay() === 6));
    };

    //calendar options
    _sysdate.dateOptions = {
        formatYear: 'yy',
        startingDay: 1,
        timezone: 'CST'
    };

    _sysdate.fixDate = function(x) {
        var date = new Date();
        if (x)
            date = new Date(parseInt(x.substr(6)));
        return date;
    };

    //calendar date format
    _sysdate.formats = ['MM/dd/yyyy hh:mm:ss', 'dd/MM/yyyy', 'dd.MM.yyyy', 'shortDate'];
    _sysdate.format = _sysdate.formats[1];

    _sysdate.minDate = new Date(2000, 1, 1);
    //_sysdate.maxDate = new Date(2016, 1, 1);
    _sysdate.maxDate = new Date((new Date()).setDate((new Date()).getDate() + 365));

    _sysdate.parseStrDate = function (strDate) {
        if (strDate.length > 0)
            return uibDateParser.parse(strDate, "yyyy-MM-ddThh:mm:ss")
        else
            return null;
    }

    return _sysdate;
}])
.factory('classSrvc', function ($http, $q, authService) {

    var vienaList;

    function getNizaClass() {

        var niza = [];
        // create niza list   
        for (i = 0; i < 46; i++) {
            niza.push({ code: i, selected: false });
        }
        niza.push({ code: 99, selected: false });
        niza.push({ code: "All", selected: false });
        return niza;
    }

    function _fetchVienaClass() {
        var deferred = $q.defer();
        $http.get("/publico/content/json/viena.json")
            .then(
                function (result) {
                    vienaList = result.data;
                    $.jrzStorage.local.setItem("vienaClass", vienaList)
                    deferred.resolve(vienaList);
                },
                function (error) {
                    deferred.reject(error);
                }
            );
        return deferred.promise;
    }

    function getVienaClass() {
        if ($.jrzStorage.local.exists("vienaClass")) {
            vienaList = $.jrzStorage.local.getItem("vienaClass");
        }
        else {
            _fetchVienaClass();
        }
        return vienaList;
    }

    function _init() {
        console.log('classSrvc init...');
        _fetchVienaClass();
    }

    _init();


    function getGrupos() {
        return $http.get('/eRegistroAPI/Favoritos/GetGrupos', { headers: { "access_token": authService.getToken() } });
    };

    return {
        getNizaClass: getNizaClass,
        getVienaClass: getVienaClass,
        getGrupos: getGrupos
    };
});







Array.prototype.firstOrDefault = function (callbackExp) {
    for (var i = 0; i < this.length; i++) {
        if (callbackExp(this, i)) {
            return this[i];
        }
    }
    return null;
};

