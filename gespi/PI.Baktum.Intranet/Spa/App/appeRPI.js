var yumKaaxApp = angular.module('yumKaaxApp', [
  'ngRoute'
  , 'eRPIControllers'
  , 'eRegistroAPI'
//  'usersFactory',
//  'pageFactory',
  , 'authFactory'
  , 'utilsFactory'
  //'userDirective',
  //'menuDirective',
  , 'grlDirectives'
  //, 'chieffancypants.loadingBar'
  , 'ngAnimate', 'ngSanitize'
  //, 'angularUtils.directives.dirPagination'
  , 'ui.bootstrap'
  , 'ngSanitize'
  //, 'ui.bootstrap.typeahead'
  //, 'ui.grid', 'ui.grid.exporter', 'ui.grid.selection'
  , 'ui.utils'
  , 'textAngular'
]);

yumKaaxApp.config(function ($routeProvider) {
    var urlBase = './Spa/Views/';

    $routeProvider
    .when('/', {
        templateUrl: urlBase + 'eMarcas/Home.html',
        controller: 'eRPICtrl',
        resolve: {
            auth: function ($q, authService) {                
                var userInfo = authService.getUserInfo();

                if (userInfo) {
                    return $q.when(userInfo);
                } else {
                    return $q.reject({ authenticated: false });
                }
            }
        }
    })
    .when('/login',//redirect to base !!
    {
    templateUrl: urlBase + 'Base/Login.html',
        controller: 'authCtrl'
    })
    .when('/Marcas',
    {
        //templateUrl: urlBase + 'eMarcas/Marcas.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'MarcasCtrl'
    })
    .when('/Marcas/:numero',
    {
        //templateUrl: urlBase + 'eMarcas/Marcas.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'MarcasCtrl'
    })
    .when('/Marcas/id/:id',
    {
        //templateUrl: urlBase + 'eMarcas/Marcas.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'MarcasCtrl'
    })
    .when('/Anotaciones',
    {
        //templateUrl: urlBase + 'eAnota/Anota.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'AnotacionesCtrl'
    })
    .when('/Anotaciones/:numero/:tipo',
    {
        //templateUrl: urlBase + 'eAnota/Anota.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'AnotacionesCtrl'
    })
    .when('/Renovaciones',
    {
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'RenovacionesCtrl'
    })
    .when('/Renovaciones/:numero',
    {
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'RenovacionesCtrl'
    })
    .when('/BusquedasExt',
    {
        templateUrl: urlBase + 'eMarcas/busquedas-ext.html',
        controller: 'BusquedasExtCtrl'
    })
    .when('/Patentes',
    {
        //templateUrl: urlBase + 'ePatentes/Patentes.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'PatenteCtrl'
    })
    .when('/Patentes/:numero/:tipo', {
        //templateUrl: urlBase + 'ePatentes/Patentes.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'PatenteCtrl'
    })
    .when('/DA',
    {
        //templateUrl: urlBase + 'eDA/DA.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'DACtrl'
    })
    .when('/DA/:numero',
    {
        //templateUrl: urlBase + 'eDA/DA.html',
        templateUrl: urlBase + 'shared/SolicitudTpl.html',
        controller: 'DACtrl'
    })
    .otherwise({
        redirectTo: ''
    });
});


yumKaaxApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('sessionInjector');
}]);


//yumKaaxApp.config(function (cfpLoadingBarProvider) {
//    cfpLoadingBarProvider.includeSpinner = true;
//})


yumKaaxApp.run(["$rootScope", "$location", function ($rootScope, $location) {

    $rootScope.$on("$routeChangeSuccess", function (userInfo) {
        console.log(userInfo);
    });

    $rootScope.$on("$routeChangeError", function (event, current, previous, eventObj) {
        if (eventObj.authenticated === false) {
            $location.path("/login");
        }
    });
}]);


yumKaaxApp.filter("dateformat", function () {
    var re = /\\\/Date\(([0-9]*)\)\\\//;
    return function (x) {
        var m = x.match(re);
        if (m) return new Date(parseInt(m[1]));
        else return null;
    };
});

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

function findById(data, idToLookFor) {
    for (var i = 0; i < data.length; i++) {
        if (data[i].Id == idToLookFor) {
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
