var appPopupBox = function (scope) {

    this.scope = scope;

    this.pushMessage = function (message) {
        this.scope.appPopupMsg.unshift(message)
    }

    this.closeAlertInfo = (function (scope) {
        scope.showAlertInfo = false;
        scope.alertInfo = "";
        scope.$apply();
    });

    this.showAlertInfo = function (message) {
        this.scope.alertInfo = message;
        this.scope.showAlertInfo = true;

        window.setTimeout(this.closeAlertInfo, 2000, this.scope);
    }



    this.handleError = function (error, message) {
        if (error && error.status == 401)
            message = 'Usuario no tiene permiso para ejecutar esta opcion!'
        else if (!message)
            message = "El sistema ha encontrado un error inesperado, porfavor contacte al administrador del sistema!";

        this.pushMessage(message)
        this.showAlertInfo(message);
    }
};