
///---------------
/// Marcas
///---------------
yumKaaxControllers.controller("CronCtrlr", ["$scope", "$http", "$q", "$routeParams", "$timeout", "$route", "$location", "authService", "listService", 
function ($scope, $http, $q, $routeParams, $timeout, $route, $location, authService, listService) {
      
    //Place separate controller
    $scope.GetDOCResol = function (cronologiaId) {
        
        var service = '/Expediente/';
        $http.get(authService.api() + service + "GetDOCResol?cronologiaId=" + cronologiaId)
        .then(function (result) {
            if (result.data)
                listService.printDoc(result);

            $scope.appPopupBox.showAlertInfo("Evento no contiene documento imprimible...");
            console.log(result);
        });
    }

}]); //End Paises



