
yumKaaxControllers.controller
    (
    "authCtrl",
    [
        "$scope", "$location", "$window", "authService",
        function ($scope, $location, $window, authService) {
            $scope.userIdentity = null;
            $scope.login = function () {
                authService.login($scope.userName, $scope.password)
                    .then(function (result) {
                        $scope.userIdentity = result;
                        $location.path("/");
                    }, function (error) {
                        $window.alert("Invalid credentials**");
                        console.log(error);
                    });
            };

            $scope.cancel = function () {
                $scope.userName = "";
                $scope.password = "";
            };
        }
    ]
    );