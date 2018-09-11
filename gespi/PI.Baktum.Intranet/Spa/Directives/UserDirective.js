angular.module('userDirective', [])
    .directive('user', function () {
        return {
            scope: { user: '=' },
            restrict: 'A',
            templateUrl: 'Spa/Views/user.html',
            controller: function ($scope, users) {
                users.find($scope.user.id, function (user) {
                    //$scope.userId = user.id;
                    $scope.user = user;
                });
            }
        };
    });