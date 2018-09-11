// nice design
//http://ironsummitmedia.github.io/startbootstrap-sb-admin-2/pages/index.html

///------------------
/// Derecho de Autor
///------------------
yumKaaxControllers.controller("DACtrl", ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "listService", "authService", "$location", "formUtils", "dateHelper", 
function ($scope, $http, $q, $routeParams, $timeout, $route, listService, authService, $location, formUtils, dateHelper) {
    $scope.config = { _expediente: "./Spa/Views/eDA/DAExpediente.html", _modulo: "Derecho de Autor" };
    $scope.$route = $route;
    $scope.model = {};
    $scope.opciones = listService.getDAmnu();
    $scope.tipoDeObras = [];
    $scope.paises = [];
    $scope.classObra = [{ Id: "R", Nombre: "Rustico" }, { Id: "L", Nombre: "De Lujo" }];
    $scope.enCalidadLst = [{ Id: "1", Nombre: "Autor" }, { Id: "2", Nombre: "Titular del Derecho" }];
    $scope.estadoCivilLst = [{ Id: "", Nombre: "" }, { Id: "1", Nombre: "Soltero" }, { Id: "2", Nombre: "Casado" }, { Id: "3", Nombre: "Divorciado" }, { Id: "4", Nombre: "Viudo" }];
    $scope.partituraLst = [{ Id: "", Nombre: "" }, { Id: "S", Nombre: "SI" }, { Id: "N", Nombre: "NO" }];
    $scope.dateHelper = dateHelper;
    $scope.appPopupBox = new appPopupBox($scope);
    $scope.Autor_FechaNacimiento = undefined;
    $scope.Autor_Defuncion = undefined;


    /*SYSTEM*/
    $scope.showAlertInfo = false;
    $scope.alertInfo = "";
    $scope.res = "";
    $scope.evento = "";
    $scope._resolucion = { FechaResolucion: new Date() };
    $scope.Estatuses = [];
    $scope.user = authService.getUserInfo();
    /*SYSTEM*/

    function _ctrlInit() {
        var lists = listService.getLists();
        $scope.tipoDeObras = lists.tipoDeObras;
        $scope.paises = lists.paises;
        $scope.model = { Expediente: { Id: -1, TipoDeRegistroId: 0, Numero: '', FechaDeSolicitud: undefined } };
        getEstatus();
    }

    function getEstatus() {
        $http.get(authService.api() + '/Admin/Estatus/GetPage?page=1&pageSize=200&moduloId=3')
            .then(function (result) {
                console.log(result);
            $scope.Estatuses = result.data.Result.DataSet;
        });
    }

    function saveOrgModel() {
        $scope.modelCpy = angular.copy($scope.model);
    }

    function fixModelDates()
    {        
        $scope.model.Expediente.FechaDeSolicitud = dateHelper.parseStrDate($scope.model.Expediente.FechaDeSolicitud);
        $scope.model.Expediente.FechaDeEstatus = dateHelper.parseStrDate($scope.model.Expediente.FechaDeEstatus);
        $scope.model.DerechoDeAutor.FechaEdicion = dateHelper.parseStrDate($scope.model.DerechoDeAutor.FechaEdicion);
        $scope.model.DerechoDeAutor.FechaPublicacion = dateHelper.parseStrDate($scope.model.DerechoDeAutor.FechaPublicacion);
        saveOrgModel();
    }

    function getDocto(action, queryString) {
        var service = '/DAutor/';
        $http.get(authService.api() + service + action + queryString)
        .then(function (result) {
                $scope.model = result.data.Result.documento;
                fixModelDates();
                console.log('Fetched by expediente!!');
            },
            function (error) {
                $scope.appPopupBox.handleError(error, 'Expediente no encontrado - ' + ($routeParams.numero ? $routeParams.numero : ""));
                console.log(error);
                $scope.model = { };
            }
        );
    }

    function getEntityById(Id) {
        getDocto('ExpedienteId','?id=' + Id);
    };

    $scope.getExpediente = function (expediente) {
        if (!$routeParams.numero) {
            $location.path("/DA/" + expediente);
        }
        else {
            $scope.model.Expediente.Numero = expediente;
            getDocto('Expediente', '?numero=' + expediente);
            $location.path("/DA/" + expediente, false);
        }
    };


    $scope.saveSolicitud = function (frmSol) {
        var service = '/DAutor/';
        console.log(frmSol);
        if (!frmSol.$valid)
            return;
        //console.log('CPY');
        //console.log(JSON.stringify($scope.modelCpy));

        var changes = formUtils.getFormChanges(frmSol, $scope.modelCpy, $scope.model);
        console.log(JSON.stringify(changes));
        var genEntity = { Generic: $scope.model, Auditoria: JSON.stringify(changes) };

        $http.post(authService.api() + service + "SaveSolicitud", { model: genEntity })
            .then(
                function (result) {
                    if ($scope.model.Expediente.Id == -1)                        
                        $scope.getExpediente($scope.model.Expediente.Numero);
                    else
                        $scope.getExpediente($routeParams.numero);
                        
                    $scope.appPopupBox.showAlertInfo('Solicitud Actualizada...');
                    $scope.appPopupBox.handleError(null, 'Expediente actualizado - ' + $scope.model.Expediente.Numero);
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }


    /*
    * Save Autor
    */
    $scope.saveAutor = function (autor) {

        console.log(JSON.stringify(autor));
        var service = '/DAutor/';
        autor.ExpedienteId = $scope.model.Expediente.Id;
        var auditoria = JSON.stringify(autor);
        var genEntity = { Generic: autor, Auditoria: auditoria };

        $http.post(authService.api() + service + "SaveAutor", { model: genEntity })
            .then(
                function (result) {
                    console.log("response: " + JSON.stringify(result.data));
                    if (!$scope.model.Autores)
                        $scope.model.Autores = [];
                    $scope.model.Autores.push(result.data.Result);
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                    console.log(error);                    
                }
            );
    }

    $scope.deleteAutor = function (autor) {
        var service = '/DAutor/';
        var auditoria = JSON.stringify(autor);
        var genEntity = { Generic: autor, Auditoria: auditoria };

        $http.post(authService.api() + service + "DeleteAutor", { model: genEntity })
            .then(
                function (result) {
                    //removeItemByhashKey($scope.model.Inventores, inventor.$$hashKey);
                    $scope.model.Autores.remove(autor);
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }


    /*
    * Save FonogramaTituloDeObra
    */
    $scope.saveFonogramaTituloDeObra = function (entry) {

        console.log(JSON.stringify(entry));
        var service = '/DAutor/';
        entry.ExpedienteId = $scope.model.Expediente.Id;
        var auditoria = JSON.stringify(entry);
        var genEntity = { Generic: entry, Auditoria: auditoria };

        $http.post(authService.api() + service + "saveFonogramaTituloDeObra", { model: genEntity })
            .then(
                function (result) {
                    console.log("response: " + JSON.stringify(result.data));
                    if (!$scope.model.FonogramaTituloDeObras)
                        $scope.model.FonogramaTituloDeObras = [];
                    $scope.model.FonogramaTituloDeObras.push(result.data.Result);
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }

    $scope.deleteFonogramaTituloDeObra = function (entry) {
        var service = '/DAutor/';
        var auditoria = JSON.stringify(entry);
        var genEntity = { Generic: entry, Auditoria: auditoria };

        $http.post(authService.api() + service + "deleteFonogramaTituloDeObra", { model: genEntity })
            .then(
                function (result) {
                    //removeItemByhashKey($scope.model.Inventores, inventor.$$hashKey);
                    $scope.model.FonogramaTituloDeObras.remove(entry);
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }


    /*
        * Save saveFonogramaArtista
        */
    $scope.saveFonogramaArtista = function (entry) {

        console.log(JSON.stringify(entry));
        var service = '/DAutor/';
        entry.ExpedienteId = $scope.model.Expediente.Id;
        var auditoria = JSON.stringify(entry);
        var genEntity = { Generic: entry, Auditoria: auditoria };

        $http.post(authService.api() + service + "saveFonogramaArtista", { model: genEntity })
            .then(
                function (result) {
                    console.log("response: " + JSON.stringify(result.data));
                    if (!$scope.model.FonogramaArtistas)
                        $scope.model.FonogramaArtistas = [];
                    $scope.model.FonogramaArtistas.push(result.data.Result);
                },
                function (error) {
                    console.log('Error:');
                    console.log(error);
                    $scope.appPopupBox.handleError(error)
                }
            );
    }

    $scope.deleteFonogramaArtista = function (entry) {
        var service = '/DAutor/';
        var auditoria = JSON.stringify(entry);
        var genEntity = { Generic: entry, Auditoria: auditoria };

        $http.post(authService.api() + service + "deleteFonogramaArtista", { model: genEntity })
            .then(
                function (result) {
                    //removeItemByhashKey($scope.model.Inventores, inventor.$$hashKey);
                    $scope.model.FonogramaArtistas.remove(entry);
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }

    /*
    * Save GuionAutor
    */
    $scope.saveGuionAutor = function (entry) {

        console.log(JSON.stringify(entry));
        var service = '/DAutor/';
        entry.ExpedienteId = $scope.model.Expediente.Id;
        var auditoria = JSON.stringify(entry);
        var genEntity = { Generic: entry, Auditoria: auditoria };

        $http.post(authService.api() + service + "saveGuionAutor", { model: genEntity })
            .then(
                function (result) {
                    console.log("response: " + JSON.stringify(result.data));
                    if (!$scope.model.GuionAutores)
                        $scope.model.GuionAutores = [];
                    $scope.model.GuionAutores.push(result.data.Result);
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }

    $scope.deleteGuionAutor = function (entry) {
        var service = '/DAutor/';
        var auditoria = JSON.stringify(entry);
        var genEntity = { Generic: entry, Auditoria: auditoria };

        $http.post(authService.api() + service + "deleteGuionAutor", { model: genEntity })
            .then(
                function (result) {
                    //removeItemByhashKey($scope.model.Inventores, inventor.$$hashKey);
                    $scope.model.GuionAutores.remove(entry);
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }


    /*
    * Save AudiovisualAutores
    */
    $scope.saveAudiovisualAutor = function (entry) {

        console.log(JSON.stringify(entry));
        var service = '/DAutor/';
        entry.ExpedienteId = $scope.model.Expediente.Id;
        var auditoria = JSON.stringify(entry);
        var genEntity = { Generic: entry, Auditoria: auditoria };

        $http.post(authService.api() + service + "saveAudiovisualAutor", { model: genEntity })
            .then(
                function (result) {
                    console.log("response: " + JSON.stringify(result.data));
                    if (!$scope.model.AudiovisualAutores)
                        $scope.model.AudiovisualAutores = [];
                    $scope.model.AudiovisualAutores.push(result.data.Result);
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }

    $scope.deleteAudiovisualAutor = function (entry) {
        var service = '/DAutor/';
        var auditoria = JSON.stringify(entry);
        var genEntity = { Generic: entry, Auditoria: auditoria };

        $http.post(authService.api() + service + "deleteAudiovisualAutor", { model: genEntity })
            .then(
                function (result) {
                    //removeItemByhashKey($scope.model.Inventores, inventor.$$hashKey);
                    $scope.model.AudiovisualAutores.remove(entry);
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }


    /*
    * Save ComposicionAutores
    */
    $scope.saveComposicionAutor = function (entry) {

        console.log(JSON.stringify(entry));
        var service = '/DAutor/';
        entry.ExpedienteId = $scope.model.Expediente.Id;
        var auditoria = JSON.stringify(entry);
        var genEntity = { Generic: entry, Auditoria: auditoria };

        $http.post(authService.api() + service + "saveComposicionAutor", { model: genEntity })
            .then(
                function (result) {
                    console.log("response: " + JSON.stringify(result.data));
                    if (!$scope.model.ComposicionAutores)
                        $scope.model.ComposicionAutores = [];
                    $scope.model.ComposicionAutores.push(result.data.Result);
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }

    $scope.deleteComposicionAutor = function (entry) {
        var service = '/DAutor/';
        var auditoria = JSON.stringify(entry);
        var genEntity = { Generic: entry, Auditoria: auditoria };

        $http.post(authService.api() + service + "deleteComposicionAutor", { model: genEntity })
            .then(
                function (result) {
                    //removeItemByhashKey($scope.model.Inventores, inventor.$$hashKey);
                    $scope.model.ComposicionAutores.remove(entry);
                },
                function (error) {
                    $scope.appPopupBox.handleError(error)
                }
            );
    }

    $scope.getNew = function () {
        $scope.model = { Expediente: { Id: -1, TipoDeRegistroId: 0, Numero: '', FechaDeSolicitud: undefined } };
    };

    function getFechaEnLetras(_fecha) {
        var _day = _fecha.getDate();
        var _month = _fecha.getMonth(); //0-11
        var _year = _fecha.getFullYear();
        var _enLetras = listService.getEnLetras(_day) + " DE " + listService.getMes(_month) + " DEL DOS MIL " + listService.getEnLetras(_year - 2000);
        return _enLetras;
    }

    function genResolucion(_resolucion) {
        $scope.model.Expediente.FechaDeSolicitud = new Date($scope.model.Expediente.FechaDeSolicitud);
        var _fsolicitud = getFechaEnLetras($scope.model.Expediente.FechaDeSolicitud);

        var _autor = "";
        var variosAutores = 0;
        for (var i = 0; i < $scope.model.Autores.length; i++) {
            if (_autor == "") {
                _autor += $scope.model.Autores[i].NombreAutor;
            }
            else {
                variosAutores = 1;
                if (i + 1 < $scope.model.Autores.length) {
                    _autor += ', ' + $scope.model.Autores[i].NombreAutor;
                }
                else {
                    _autor += ' Y ' + $scope.model.Autores[i].NombreAutor;
                }
            }
        }
        if ($scope.model.Expediente.TipoDeRegistroId == 13 || $scope.model.Expediente.TipoDeRegistroId == 17) {
            _autor = $scope.model.Productor.Nombre;
            _autor = "productor es " + _autor;
        }
        else {
            if (variosAutores)
                _autor = "autores son " + _autor;
            else
                _autor = "autor es " + _autor;
        }

        var considerando = "CONSIDERANDO: Que el artículo 104 de la Ley de Derecho de Autor y Derechos Conexos - Decreto 33-98, reformado por el Decreto 56-2000, ambos del Congreso de la República;  establece que el Registro de la Propiedad Intelectual, tiene por atribución principal, sin perjuicio de lo que dispongan otras leyes, garantizar la seguridad jurídica de los autores, de los titulares de los derechos conexos y de los titulares de los derechos patrimoniales respectivos y sus causahabientes, así como dar una adecuada publicidad a las obras, actos y documentos a través de su inscripción cuando así lo soliciten los titulares. CONSIDERANDO; Que el artículo 24 de la ley citada estipula que, por el derecho de autor, queda protegida exclusivamente la forma mediante la cual las ideas del autor son descritas, explicadas, ilustradas o incorporadas a las obras; CONSIDERANDO: Que en su artículo 105 la citada  ley preceptúa que la inscripción en el Registro presume ciertos los hechos y los actos que en ella consten, salvo prueba en contrario; y que toda inscripción deja a salvo los derechos de terceros; CONSIDERANDO: Que este Registro, al efectuar el análisis del presente expediente, establece que la solicitud satisface los requisitos contemplados en la Ley y su Reglamento, por lo que considera procedente acceder a lo solicitado. POR TANTO: Con base en lo considerado y lo que para el efecto establecen los artículos del 104 al 112 de la Ley de Derecho de Autor y Derechos Conexos y  41 de su Reglamento, Acuerdo Gubernativo 233-2003";
        var _Resuelve = "";
        var _delaObra = "";
        var titulada = "";
        if ($scope.model.Expediente.TipoDeRegistroId == 13) {
            _delaObra = "del fonograma ";
            titulada = " del fonograma titulado ";
        }
        else if ($scope.model.Expediente.TipoDeRegistroId == 17) {
            _delaObra = "del programa de ordenador ";
            titulada = " del programa de ordenador titulado ";
        }
        else {
            _delaObra = "de la Obra ";
            titulada = " de la obra titulada ";
        }
        _Resuelve = "RESUELVE: I- Con lugar la solicitud de depósito e inscripción " + _delaObra + $scope.model.DerechoDeAutor.Titulo + " cuyo ";

        _Resuelve += _autor + " II. Procédase";
        _Resuelve += " a efectuar la inscripción de conformidad con la Ley y su Reglamento y efectuado lo anterior, ";
        _Resuelve += " extiéndase el certificado correspondiente. III. Notifiquese.";

        var aLaVista = "I) Se tiene a la vista para resolver la solicitud presentada por " + $scope.model.Solicitante.Nombre.trim() + " con fecha " + _fsolicitud + ",";

        _Resuelve = aLaVista + titulada + $scope.model.DerechoDeAutor.Titulo + " II) " + considerando + " este Registro," + _Resuelve;
        return _Resuelve;
    }

    function refreshExpediente() {
        $scope.getExpediente($scope.model.Expediente.Numero);
        $scope._resolucion = { FechaResolucion: new Date() };
        showLeftBox();
        $scope.res = './Spa/Views/eDA/empty.html'
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

    //**********************************
    // CON LUGAR
    //**********************************
    $scope.resolucionTexto = function (_resolucion) {
        _resolucion.Resuelve = genResolucion(_resolucion);
        $scope._resolucion.Resuelve = _resolucion.Resuelve;
    }

    $scope.ConLugar = function (_resolucion) {
        var jsondoc = JSON.stringify(_resolucion);
        var resolucion = {
            ExpedienteId : $scope.model.Expediente.Id,
            Fecha : _resolucion.FechaResolucion,
            Articulos : _resolucion.Resuelve,
            JSONDOC: jsondoc
        }
        
        createResolucion("/DAutor/ConLugar", resolucion);
    }
    
    //**********************************
    // RECHAZO <resolucionTexto>
    //**********************************
    $scope.Rechazo = function (_resolucion) {
        var jsondoc = JSON.stringify(_resolucion);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: _resolucion.FechaResolucion,
            Articulos: _resolucion.Resuelve,
            JSONDOC: jsondoc
        }
        
        createResolucion("/DAutor/Rechazo", resolucion);
    }

    //**********************************
    // SUSPENSO <resolucionTexto>
    //**********************************
    $scope.Suspenso = function (_resolucion) {
        var jsondoc = JSON.stringify(_resolucion);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: _resolucion.FechaResolucion,
            Articulos: _resolucion.Resuelve,
            JSONDOC: jsondoc
        }

        createResolucion("/DAutor/Suspenso", resolucion);
    }

    //**********************************
    // OPERAR MEMORIAL
    //**********************************
    $scope.OperarMemorial = function (_resolucion) {
        var jsondoc = JSON.stringify(_resolucion);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: _resolucion.FechaResolucion,
            Observaciones: _resolucion.Observaciones,
            JSONDOC: jsondoc
        }

        createResolucion("/DAutor/OperarMemorial", resolucion);
    }

    //**********************************
    // LEVANTAR SUSPENSO <resolucionTexto>
    //**********************************
    $scope.LevantarSuspenso = function (_resolucion) {
        var jsondoc = JSON.stringify(_resolucion);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: _resolucion.FechaResolucion,
            Articulos: _resolucion.Resuelve,
            JSONDOC: jsondoc
        }

        createResolucion("/DAutor/LevantarSuspenso", resolucion);
    }


    //**********************************
    // RECURSO DE REVOCATORIA <resolucionTexto>
    //**********************************
    $scope.RecursoRevocatoria = function (_resolucion) {
        var jsondoc = JSON.stringify(_resolucion);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: _resolucion.FechaResolucion,
            Articulos: _resolucion.Resuelve,
            JSONDOC: jsondoc
        }

        createResolucion("/DAutor/RecursoRevocatoria", resolucion);
    }

    //**********************************
    // ELEVANDO RECURSO REVOCATORIA <resolucionTexto>
    //**********************************
    $scope.ElevandoRecurso = function (_resolucion) {
        var jsondoc = JSON.stringify(_resolucion);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: _resolucion.FechaResolucion,
            Articulos: _resolucion.Resuelve,
            JSONDOC: jsondoc
        }

        createResolucion("/DAutor/ElevandoRecurso", resolucion);
    }


    //**********************************
    // POR RECIBIDO MINECO - Registro <resolucionTexto>
    //**********************************
    $scope.PorRecibidoMINECOaRegistro = function (_resolucion) {
        var jsondoc = JSON.stringify(_resolucion);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: _resolucion.FechaResolucion,
            Articulos: _resolucion.Resuelve,
            JSONDOC: jsondoc
        }

        createResolucion("/DAutor/PorRecibidoMINECOaRegistro", resolucion);
    }

    //**********************************
    // POR RECIBIDO MINECO - Registro  <resolucionTexto>
    //**********************************
    $scope.PorRecibidoMINECOArchivo = function (_resolucion) {
        var jsondoc = JSON.stringify(_resolucion);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: _resolucion.FechaResolucion,
            Articulos: _resolucion.Resuelve,
            JSONDOC: jsondoc
        }

        createResolucion("/DAutor/PorRecibidoMINECOArchivo", resolucion);
    }



    //**********************************
    // NOTIFICAR
    //**********************************
    $scope._140NotificarSaveNPrint = function (_resolucion) {
        var jsondoc = JSON.stringify(_resolucion);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: _resolucion.FechaResolucion,
            Hora: _resolucion.Hora,
            Notificador: _resolucion.Notificador,
            JSONDOC: jsondoc
        }

        //DA130To131 ->ConLugar
        createResolucion("/DAutor/Notificar", resolucion);
    }

    //**********************************
    // EMITIR TITULO
    //**********************************
    $scope._140EmitirTituloSaveNPrint = function (_resolucion) {
        var jsondoc = JSON.stringify(_resolucion);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: _resolucion.FechaResolucion,
            Tomo: _resolucion.Tomo,
            Folio: _resolucion.Folio,
            Registro: _resolucion.Registro,
            Libro: _resolucion.Libro,
            JSONDOC: jsondoc
        }

        
        createResolucion("/DAutor/EmitirTitulo", resolucion);
    }

    //**********************************
    // CAMBIAR ESTATUS //potentially we could print a resolution here...
    //**********************************
    $scope._00CambiarEstatusSaveNPrint = function (_resolucion) {
        
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: _resolucion.FechaResolucion,
            EstatusId: _resolucion.EstatusId.Id,
        }
        var jsondoc = JSON.stringify(resolucion);
        resolucion.JSONDOC = jsondoc;

        //DA130To131 ->ConLugar
        createResolucion("/DAutor/CambiarEstatus", resolucion);
    }
    
    //**********************************
    // REPOSICION TITULO
    //**********************************
    $scope._135ReposiciondeTituloSaveNPrint = function (_resolucion) {
        var jsondoc = JSON.stringify(_resolucion);
        var resolucion = {
            ExpedienteId: $scope.model.Expediente.Id,
            Fecha: _resolucion.FechaResolucion,
            JSONDOC: jsondoc
        }

        //DA130To131 ->ConLugar
        createResolucion("/DAutor/ReposiciondeTitulo", resolucion);
    }

    /*********************
    SYSTEM
    **********************/
    //calendar flag controllers
    $scope.fds = {}; //fecha de solicitud
    $scope.fres = {}; // fecha de resolucion
    $scope.fde = {};
    $scope.fpub = {};
    $scope.saf = {};
    $scope.fresolucion = {};
    $scope.frechazo = {};
    $scope.afn = {};
    $scope.afd = {};
    
    // Disable weekend selection
    $scope.disabled = function (date, mode) {
        return (mode === 'day' && (date.getDay() === 10 || date.getDay() === 6));
    };

    $scope.minDate = new Date(2000, 1, 1);
    $scope.maxDate = new Date(2020, 1, 1);
    $scope.open = function ($event, wtf) {
        $event.preventDefault();
        $event.stopPropagation();
        if (wtf == 'fds')
            $scope.fds.open = true
        else if (wtf == 'fres')
            $scope.fres.open = true
        else if (wtf == 'fde')
            $scope.fde.open = true
        else if (wtf == 'fpub')
            $scope.fpub.open = true
        else if (wtf == 'fresolucion')
            $scope.fresolucion.open = true
        else if (wtf == 'frechazo')
            $scope.frechazo.open = true
        else if (wtf == 'afn')
            $scope.afn.open = true
        else if (wtf == 'afd')
            $scope.afd.open = true
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
    $scope.formats = ['dd/MM/yyyy', 'dd.MM.yyyy hh:mm:ss', 'shortDate'];
    $scope.format = $scope.formats[0];

    //$scope.saveAndPrintConLugar = function () {
    //    alert('here we go...');
    //    alert($scope.model.Expediente.Titulo);
    //}

    $scope.isMnuEnabled = function (opcion) {
        var _disabled = "";
        try {
            if (opcion._estatus.length == 0) {
                _disabled = "";
            }
            else {
                //uncomment this in production
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
        //uncomment this in production
        //if (opcion._estatus.length != 0 && (view == '' || opcion._estatus.indexOf($scope.model.Expediente.EstatusId) == -1)) {
        if (!true){
            $scope.res = './Spa/Views/empty.html'
            $scope.$apply();
        }
        else {
            $scope.res = './Spa/Views/eDA/' + view;
            $scope.evento = opcion.opcion;
        }
        $scope._resolucion = { resId: "", FechaResolucion: new Date() };
    }


    $scope.rollbackResolution = function () {
        if ($routeParams.numero)
            $location.path("/DA/" + $routeParams.numero);
        else
            $location.path("/DA");

        $scope.Resolucion({ _estatus: [0] });//reset resolution
        showLeftBox();
    };

    _ctrlInit();
    if ($routeParams.numero) {
        $scope.getExpediente($routeParams.numero)
    }

}]);