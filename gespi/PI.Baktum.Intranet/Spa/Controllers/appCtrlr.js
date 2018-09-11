
///---------------
/// app-CTRLR
///---------------
yumKaaxControllers.controller("appCtrlr", ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "authService",
    function ($scope) {
        $scope.appPopupMsg = [];

        $scope.closeAppPopup = function () {
            $scope.appPopupMsg = [];
        }
    }
]); //End Agentes
