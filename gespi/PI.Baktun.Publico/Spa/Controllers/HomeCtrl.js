var yumKaaxControllers = angular.module('PublicControllers', []);

yumKaaxControllers.controller("HomeCtrl", ["$scope", "$location", "authService", function ($scope, $location, authService) {
    $scope.userIdentity = authService.getUserInfo();
 
    $scope.eRpiAdminMod = "/Admin";
    $scope.eRpiMod = "/eRPI";
    $scope.eRPIRecepcionMod = "/Admin";
    $scope.eRPIConsulta = "/Admin";
    
    $scope.isAdmin = false;
    $scope.isLoggedIn = false;

    $scope.logout = function () {
        authService.logout()
            .then(function (result) {
                $scope.userIdentity = null;
                $location.path("/login");
            }, function (error) {
                console.log(error);
            });
    };

    //$scope.$watch(authService.getUserInfo, function () { $scope.userIdentity = authService.getUserInfo(); $scope.isAdmin = ($scope.userIdentity.Role == 'SuperAdmin' || $scope.userIdentity.Role == 'Admin'); $scope.isLoggedIn = ($scope.userIdentity != null) });
}]);


yumKaaxControllers.controller("navBarCtrl", ["$scope", "$location", "authService", function ($scope, $location, authService) {
    $scope.userIdentity = authService.getUserInfo();

    $scope.logout = function () {
        authService.logout()
            .then(function (result) {
                $scope.userIdentity = null;
                $location.path("/login");
            }, function (error) {
                console.log(error);
            });
    };
}]);
