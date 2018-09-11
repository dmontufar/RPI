yumKaaxControllers.controller("BusquedasExtCtrl", ["$scope", "$location", "authService", "classSrvc", "$http", function ($scope, $location, authService, classSrvc, $http)
{
    $scope.userIdentity = authService.getUserInfo();
    $scope.niza = classSrvc.getNizaClass();
    $scope.nizaSel = { niza: {} };
    $scope.tipoDeBusqueda = [{ Id: 1, Nombre: "Identica" }, { Id: 2, Nombre: "Fonetica" }];
    $scope.tipoDeBusquedaSel = $scope.tipoDeBusqueda[1];
    $scope.pageNumber = 0;
    $scope.pageSize = 0;
    $scope.dataset = [];

    $scope.searchMarcas = function () {

        var csvClases = '';
        var nizaSel = $scope.nizaSel.niza;
        if (nizaSel['All']) {
            csvClases = ''
        }
        else {
            for (i = 0; i < 46; i++) {
                if (nizaSel[i]) {
                    csvClases += (i).toString() + ','
                }
            }
            if (nizaSel[99]) {
                csvClases += '99,'
            }
        }
        if (csvClases.length > 1) {
            csvClases = csvClases.slice(0, -1);
        }

        var service = '/Marca/';

        $scope.dataset2 = [];
        $scope.onSelect = function (exp) {
            $scope.dataset2.push(exp);
            for (var i = 0; i < $scope.dataset.length; i++) {
                if ($scope.dataset[i].ExpedienteId == exp.ExpedienteId) {
                    $scope.dataset.splice(i, 1);
                    break;
                }
            }

        }

        $scope.getExpedienteUrl = function (exp) {
            return "#/Marcas/" + exp.Numero;
        };

        if (!$scope.tipoDeBusquedaSel || $scope.tipoDeBusquedaSel.Id == 2) {
            $http.get(authService.api() + service + 'BusquedaFonetica?pageNumber=' + $scope.pageNumber
                + "&pageSize=" + $scope.pageSize
                + "&textToSearch=" + $scope.textSearch.toUpperCase()
                + "&csvClases=" + csvClases
            )
            .then(function (result) {
                $scope.dataset = result.data.Result.DataSet;
                console.log(JSON.stringify(result.data.Result.DataSet));
                $scope.totalItems = result.data.Result.DataSet.length;//tbfix
            });
        }
        else {
            $http.get(authService.api() + service + 'BusquedaIdentica?pageNumber=' + $scope.pageNumber
                + "&pageSize=" + $scope.pageSize
                + "&textToSearch=" + $scope.textSearch.toUpperCase()
                + "&csvClases=" + csvClases
                )
            .then(function (result) {
                $scope.dataset = result.data.Result.DataSet;
                console.log(JSON.stringify(result.data.Result.DataSet[0]));
                $scope.totalItems = result.data.Result.DataSet.length;//tbfix
            });
        }
    }

    var showAlertInfo = function (message) {
        $scope.alertInfo = message;
        $scope.showAlertInfo = true;
        $timeout(function () {
            $scope.showAlertInfo = false;
            $scope.alertInfo = "";
        }, 2400);
    }
}
]);

