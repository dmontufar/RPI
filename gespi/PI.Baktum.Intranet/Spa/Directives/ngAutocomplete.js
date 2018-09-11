
angular.module("ngAutocomplete", [])
  .directive('autoComplete', function ($timeout) {
      return function (scope, iElement, iAttrs) {

          scope.nameId = iElement.value;
          iElement.autocomplete({
              source: scope[iAttrs.uiItems],
              select: function () {
                  $timeout(function () {
                      iElement.trigger('input');
                  }, 0);
              }
          });
      }
  });



app.directive('autoComplete', function ($rootScope, locationAutoCompleteService, $timeout, $http, programLocationModel) {
    return {
        restrict: 'A',
        scope: {
            serviceType: '@serviceType'
        },
        link: function (scope, elem, attr, ctrl) {
            var autoItem = [];
            scope.change = function () {
                locationAutoCompleteService.unSubscribe();
                var service = locationAutoCompleteService.getServiceDefinition();
                service.filters.pattern = scope.inputVal;
                locationAutoCompleteService.subscribe();
            };
            scope.$on('myData', function (event, message) {
                if (message !== null && message.results !== null) {
                    autoItem = [];
                    for (var i = 0; i < message.results.length; i++) {
                        autoItem.push({
                            label: message.results[i].name,
                            id: message.results[i].id
                        });
                    }
                    elem.autocomplete({
                        source: autoItem,
                        select: function (event, ui) {
                            $timeout(function () {
                                elem.trigger('input');
                            }, 0);
                        }
                    });
                }
            });
        }
    };
});