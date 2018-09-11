var yumKaaxApp = angular.module('yumKaaxApp', [
  ,'ngRoute'
  ,'AdminControllers'
//  'usersFactory',
  ,'pageFactory'
  ,'authFactory'
//  'menuDirective',
  ,'grlDirectives'
  //'chieffancypants.loadingBar',
  ,'ngAnimate'
  //, 'ui.grid', 'ui.grid.exporter', 'ui.grid.selection'
  //,'angularUtils.directives.dirPagination',
  ,'ui.bootstrap'
  //,'ui.bootstrap.typeahead'
]);

yumKaaxApp.config(function ($routeProvider) {
    var urlBase = './Spa/Views/';

    $routeProvider
    .when('/Agentes', {
        templateUrl: urlBase + 'Admin/Agentes.html',
        controller: 'AgentesCtrlr'
    })
    .when('/Agentes/:Id', {
        templateUrl: urlBase + 'Admin/AgentesAbc.html',
        controller: 'AgentesCtrlr'
    })
    .when('/Paises', {
        templateUrl: urlBase + 'Admin/Paises.html',
        controller: 'PaisesCtrlr'
    })
    .when('/Paises/:Id', {
        templateUrl: urlBase + 'Admin/PaisesAbc.html',
        controller: 'PaisesCtrlr'
    })
    .when('/Estatus', {
        templateUrl: urlBase + 'Admin/Estatus.html',
        controller: 'EstatusesCtrlr'
    })
    .when('/Estatus/:Id', {
        templateUrl: urlBase + 'Admin/EstatusAbc.html',
        controller: 'EstatusesCtrlr'
    })
    .when('/Eventos', {
        templateUrl: urlBase + 'Admin/Eventos.html',
        controller: 'EventosCtrlr'
    })
    .when('/Eventos/:Id', {
        templateUrl: urlBase + 'Admin/EventosAbc.html',
        controller: 'EventosCtrlr'
    })
    .when('/OpsXEvento/:Id', {
        templateUrl: urlBase + 'Admin/OpsXEvento.html',
        controller: 'OxECtrlr'
    })    
    .when('/Leyes', {
        templateUrl: urlBase + 'Admin/Leyes.html',
        controller: 'LeyesCtrlr'
    })
    .when('/Niza', {
        templateUrl: urlBase + 'Admin/Niza.html',
        controller: 'NizaCtrlr'
    })
    .when('/Niza/:Id', {
        templateUrl: urlBase + 'Admin/NizaAbc.html',
        controller: 'NizaCtrlr'
    })
    .when('/Vienna', {
        templateUrl: urlBase + 'Admin/Vienna.html',
        controller: 'ViennaCtrlr'
    })
    .when('/Vienna/:Id', {
        templateUrl: urlBase + 'Admin/ViennaAbc.html',
        controller: 'ViennaCtrlr'
    })
    .when('/Roles', {
        templateUrl: urlBase + 'Admin/Roles.html',
        controller: 'RolesCtrlr'
    })
    .when('/Usuarios/:Id', {
        templateUrl: urlBase + 'Admin/UsuariosAbc.html',
        controller: 'UsuariosCtrlr'
    })
    .when('/Usuarios/ResetPW/:spk/:Id', {
        templateUrl: urlBase + 'Admin/UsuariosResetPW.html',
        controller: 'UsuariosCtrlr'
    })
    .when('/Usuarios/Permisos/:Id', {
        templateUrl: urlBase + 'Admin/UsuariosPerm.html',
        controller: 'UsuariosCtrlr'
    })
    .when('/Usuarios', {
        templateUrl: urlBase + 'Admin/Usuarios.html',
        controller: 'UsuariosCtrlr'
    })
    // Usuarios Publicos
    .when('/eConsulta/:Id', {
        templateUrl: urlBase + 'Admin/UsuariosPublicosAbc.html',
        controller: 'UsuariosPublicosCtrlr'
    })
    .when('/eConsulta/ResetPW/:spk/:Id', {
        templateUrl: urlBase + 'Admin/UsuariosPublicosResetPW.html',
        controller: 'UsuariosPublicosCtrlr'
    })
    .when('/eConsulta', {
        templateUrl: urlBase + 'Admin/UsuariosPublicos.html',
        controller: 'UsuariosPublicosCtrlr'
    })
    .otherwise({
        redirectTo: ''
    });
});

yumKaaxApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('sessionInjector');
}]);


//yumKaaxApp.config(function (cfpLoadingBarProvider) {
//      cfpLoadingBarProvider.includeSpinner = true;
//  })


//yumKaaxApp.run(["$rootScope", "$location", function ($rootScope, $location) {

//    $rootScope.$on("$routeChangeSuccess", function (userInfo) {
//        console.log(userInfo);
//    });

//    $rootScope.$on("$routeChangeError", function (event, current, previous, eventObj) {
//        if (eventObj.authenticated === false) {
//            $location.path("/login");
//        }
//    });
//}]);

/*
 * Interceptor
 * */
yumKaaxApp.config(function ($httpProvider) {
    $httpProvider.interceptors.push(function ($q) {
        return {
            'request': function (config) {
                $('#appSpinner').show();
                return config;
            },

            // optional method
            'requestError': function (rejection) {
                // do something on error
                $('#appSpinner').hide();
                /*                if (canRecover(rejection)) {
                                    return responseOrNewPromise
                                }*/
                return $q.reject(rejection);
            },

            'response': function (response) {
                $('#appSpinner').hide();
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
    });
});


yumKaaxApp.filter("dateformat", function () {
    var re = /\\\/Date\(([0-9]*)\)\\\//;
    return function (x) {
        var m = x.match(re);
        if (m) return new Date(parseInt(m[1]));
        else return null;
    };
});

function findById(data, idToLookFor) {
    for (var i = 0; i < data.length; i++) {
        if (data[i].id == idToLookFor) {
            return (data[i]);
        }
    }
}

function fixDotNetToJsonDate(x) {
    var date = new Date();
    if (x)
        date = new Date(parseInt(x.substr(6)));
    return date;
};
