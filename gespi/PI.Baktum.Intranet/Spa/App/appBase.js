var yumKaaxApp = angular.module('yumKaaxApp', [
  'ngRoute',
  'BaseControllers',
//  'usersFactory',
//  'pageFactory',
  'authFactory',
  //'userDirective',
  //'menuDirective',
  'grlDirectives',
  //'chieffancypants.loadingBar', 'ngAnimate'
  //, 'angularUtils.directives.dirPagination'
  , 'ui.bootstrap'
  , 'ui.bootstrap.typeahead'
]);

yumKaaxApp.config(function ($routeProvider) {
    var urlBase = './Spa/Views/';
    $routeProvider
    .when('/', {
        templateUrl: urlBase + 'Base/Home.html',
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
    .when('/login',
    {
        templateUrl: urlBase + 'Base/Login.html',
        controller: 'authCtrl'
    })
    .otherwise({
        redirectTo: ''
    });
});

yumKaaxApp.config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('sessionInjector');
}]);



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
