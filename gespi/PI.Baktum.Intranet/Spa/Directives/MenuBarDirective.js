angular.module('menuDirective', [])
    .directive('menu', function () {
        return {
            //scope: { menu: '=' },
            //restrict: 'A',
            templateUrl: 'fdc/Spa/Views/Menu.html',
            controller: function ($scope) {
                $scope.menu = $scope.menu;
            }
        };
    });