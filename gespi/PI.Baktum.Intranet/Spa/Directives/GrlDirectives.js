angular.module('grlDirectives', [])
    .directive('equals', function () {
        return {
            restrict: 'A', // only activate on element attribute
            require: '?ngModel', // get a hold of NgModelController
            link: function (scope, elem, attrs, ngModel) {
                if (!ngModel) return; // do nothing if no ng-model

                // watch own value and re-validate on change
                scope.$watch(attrs.ngModel, function () {
                    validate();
                });

                // observe the other value and re-validate on change
                attrs.$observe('equals', function (val) {
                    validate();
                });

                var validate = function () {
                    // values
                    var val1 = ngModel.$viewValue;
                    var val2 = attrs.equals;

                    // set validity
                    ngModel.$setValidity('equals', !val1 || !val2 || val1 === val2);
                };
            }
        }
    })
    .directive('ngConfirmClick',
        function () {
            return {
                link: function ($scope, element, attr) {
                    var msg = attr.ngConfirmClick || "Esta segur@? de continuar";
                    var clickAction = attr.confirmedClick;
                    var jsClick = attr.jsClick;
                    element.bind('click', function (event) {
                        if (window.confirm(msg)) {
                            $scope.$eval(clickAction);
                            eval(jsClick);
                        }
                    });
                }
            };
        })
    .directive('confirmAction', function ($modal, $parse) {
        return {
            restrict: 'EA',
            link: function (scope, element, attrs) {
                if (!attrs.continueWith) {
                    return;
                }

                var dialogCtrl = function ($scope) {
                    $scope.msg = attrs.msg ? attrs.msg : 'Continuar?';
                    $scope.confirmButtonText = attrs.confirmButtonText ? attrs.confirmButtonText : 'Yes';
                    $scope.cancelButtonText = attrs.cancelButtonText ? attrs.cancelButtonText : 'No';
                }

                element.click(function () {
                    var continueWith = $parse(attrs.continueWith);

                    if (attrs.askWhen) {
                        var askCondition = $parse(attrs.askWhen);
                        if (!askCondition(scope)) {
                            continueWith(scope);
                            scope.$apply();
                            return;
                        }
                    }

                    $modal
                        .open({
                            size: 'sm',
                            template: '<div class="modal-header"><h3 class="modal-title">{{msg}}</h3></div><div class="modal-footer"><div class="row"><div class="col-md-2"><button type="button" class="btn btn-primary btn-block" ng-click="$close(\'ok\')">{{confirmButtonText}}</button></div><div class="col-md-2"><button type="button" class="btn btn-primary btn-block" ng-click="$dismiss(\'cancel\')">{{cancelButtonText}}</button></div></div></div>',
                            controller: dialogCtrl
                        })
                        .result.then(function () {
                            continueWith(scope);
                            scope.$apply()
                        });

                });
            }
        }
    })
    .directive('loadSpinner', ['$http', '$rootScope', function ($http, $rootScope) {
    return {
        link: function (scope, elm, attrs) {
            $rootScope.spinnerActive = false;
            scope.isLoading = function () {
                return $http.pendingRequests.length > 0;
            };

            scope.$watch(scope.isLoading, function (loading) {
                $rootScope.spinnerActive = loading;
                if (loading) {
                    elm.removeClass('ng-hide');
                } else {
                    elm.addClass('ng-hide');
                }
            });
        },
        template:
        	'<div id="appSpinner">'+
                '<div class="kart-loader">'+
                    '<div class="sheath">'+
                        '<div class="segment"></div>'+
                    '</div>'+
                    '<div class="sheath">'+
                        '<div class="segment"></div>'+
                    '</div>'+
                    '<div class="sheath">'+
                        '<div class="segment"></div>'+
                    '</div>'+
                    '<div class="sheath">'+
                        '<div class="segment"></div>'+
                    '</div>'+
                    '<div class="sheath">'+
                        '<div class="segment"></div>'+
                    '</div>'+
                    '<div class="sheath">'+
                        '<div class="segment"></div>'+
                    '</div>'+
                    '<div class="sheath">'+
                        '<div class="segment"></div>'+
                    '</div>'+
                    '<div class="sheath">'+
                        '<div class="segment"></div>'+
                    '</div>'+
                    '<div class="sheath">'+
                        '<div class="segment"></div>'+
                    '</div>'+
                    '<div class="sheath">'+
                        '<div class="segment"></div>'+
                    '</div>'+
                    '<div class="sheath">'+
                        '<div class="segment"></div>'+
                    '</div>'+
                    '<div class="sheath">'+
                        '<div class="segment"></div>'+
                    '</div>'+
                '</div>' +
            '</div>'
    };
}]);
