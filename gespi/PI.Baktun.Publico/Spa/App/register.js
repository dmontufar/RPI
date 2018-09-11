var yumKaaxApp = angular.module('yumKaaxApp', [
  'ngRoute',
  'PublicControllers',
//  'usersFactory',
//  'pageFactory',
  'authFactory',
  //'userDirective',
  //'menuDirective',
//  'grlDirectives',
  'chieffancypants.loadingBar', 'ngAnimate'
  , 'angularUtils.directives.dirPagination'
  , 'ui.bootstrap'  
  , 'ui.bootstrap.typeahead'
  ,'ui.utils'
  //, 'ui.bootstrap.popover'
]);

yumKaaxApp.config(function ($routeProvider) {

    var urlBase = './Spa/Views/';

    $routeProvider
    .when('/', {
        templateUrl: urlBase + 'Home.html',
        controller: 'HomeCtrl',
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
    .when('/home',
    {
        templateUrl: urlBase + 'home.html',
        controller: 'HomeCtrl'
    })    
    .when('/gaceta',
    {
        templateUrl: urlBase + 'gaceta.html',
        controller: 'gacetaCtrl'
    })
    .when('/gacetasemanal',
    {
        templateUrl: urlBase + 'gacetasemanal.html',
        controller: 'gacetaSemanalCtrl'
    })
    .when('/mregistro',
    {
        templateUrl: urlBase + 'mregistro.html',
        controller: 'mexpedienteCtrl'
    })
    .when('/mexpediente',
    {
        templateUrl: urlBase + 'mexpediente.html',
        controller: 'mexpedienteCtrl'
    })
    .when('/mexpediente/:Id',
    {
        templateUrl: urlBase + 'mexpediente.html',
        controller: 'mexpedienteCtrl'
    })

    .when('/mpreIngreso',
    {
        templateUrl: urlBase + 'mpreIngreso.html',
        controller: 'mpreIngresoCtrl'
    })
    .when('/mpreIngreso/:Id', {
        templateUrl: urlBase + 'mpreIngresoABC.html',
        controller: 'mpreIngresoCtrl'
    })
    .when('/mfonetica',
    {
        templateUrl: urlBase + 'mfonetica.html',
        controller: 'mfoneticaCtrl'
    })
    .when('/mlogotipos',
    {
        templateUrl: urlBase + 'mlogotipos.html',
        controller: 'mlogotiposCtrl'
    })

    .when('/pregistro',
    {
        templateUrl: urlBase + 'pregistro.html',
        controller: 'pexpedienteCtrl'
    })
    .when('/pexpediente',
    {
        templateUrl: urlBase + 'pexpediente.html',
        controller: 'pexpedienteCtrl'
    })
    .when('/pexpediente/:Id',
    {
        templateUrl: urlBase + 'pexpediente.html',
        controller: 'pexpedienteCtrl'
    })
    .when('/pbusquedad',
    {
        templateUrl: urlBase + 'pbusquedad.html',
        controller: 'pbusquedadCtrl'
    })

    .when('/dregistro',
    {
        templateUrl: urlBase + 'dregistro.html',
        controller: 'dexpedienteCtrl'
    })
    .when('/dexpediente',
    {
        templateUrl: urlBase + 'dexpediente.html',
        controller: 'dexpedienteCtrl'
    })
    .when('/dexpediente/:Id',
    {
        templateUrl: urlBase + 'dexpediente.html',
        controller: 'dexpedienteCtrl'
    })

    .when('/misexpedientes',
    {
        templateUrl: urlBase + 'misexpedientes.html',
        controller: 'misexpedientesCtrl'
    })    

    .when('/rpilive',
    {
        templateUrl: urlBase + 'rpilive.html',
        controller: 'genericCtrl'
    })
    .when('/eTomo',
    {
        templateUrl: urlBase + 'eTomo.html',
        controller: 'genericCtrl'
    })
    .when('/econtacto',
    {
        templateUrl: urlBase + 'econtacto.html',
        controller: 'genericCtrl'
    })
    .when('/eclave',
    {
        templateUrl: urlBase + 'UsuariosPublicosAbc.html',
        controller: 'UsuariosPublicosCtrlr'
    })
    .when('/ResetPW/:miClave/:Id',
    {
        templateUrl: urlBase + 'UsuariosPublicosResetPW.html',
        controller: 'UsuariosPublicosCtrlr'
    })


    .when('/login',
    {
        templateUrl: urlBase + 'Login.html',
        controller: 'authCtrl'
    })
    .otherwise({
        redirectTo: ''
    });
});

yumKaaxApp.config(function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = true;
})


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

function findById(data, idToLookFor) {
    for (var i = 0; i < data.length; i++) {
        if (data[i].id == idToLookFor) {
            return (data[i]);
        }
    }
}
