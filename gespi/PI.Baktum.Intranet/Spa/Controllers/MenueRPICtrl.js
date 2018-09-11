var yumKaaxControllers = angular.module('eRPIControllers', []);

yumKaaxControllers.controller('MenueRPICtrl', function ($scope, authService) {
    $scope.isAdmin = false;
    $scope.isLoggedIn = false;
    $scope.$watch(authService.getUserInfo, function () { $scope.userIdentity = authService.getUserInfo(); $scope.isAdmin = ($scope.userIdentity.Role == 'SuperAdmin' || $scope.userIdentity.Role == 'Admin'); $scope.isLoggedIn = ($scope.userIdentity != null) });
    $scope.logout = function () {
        authService.logout()
            .then(function (result) {
                $scope.userIdentity = null;
                $location.path("/login");
            }, function (error) {
                console.log(error);
            });
    };
});

