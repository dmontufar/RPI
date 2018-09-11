angular.module('utilsFactory', [])
    .factory('formUtils', function ($http) {

    // Detect data changes
    var getFormChanges = function (ngForm, original, updated) {
        var fields = [];
        angular.forEach(ngForm, function (value, key) {

            var newValue = '', originalValue = '', fieldName = '';
            var fieldId = '', sbObject = '', property = '';
            var idx = 0;
            var afield;

            if (key[0] != '$') {
                if (!value.$pristine) {
                    if (key.indexOf('__') === -1) {
                        newValue = updated[key];
                        originalValue = ((original) ? original[key] : '');
                        fieldName = key;
                    }
                    else {
                        afield = key.split('__');
                        if (afield.length == 4) {
                            fieldId = afield[0];
                            sbObject = afield[1];
                            idx = afield[2];
                            property = afield[3];
                            if (original) {
                                if (jsUtils.isArray(original[sbObject]) && original[sbObject].length)
                                    originalValue = original[sbObject][idx][property];
                                else
                                    originalValue = undefined;
                            }
                        }
                        else {
                            fieldId = afield[0];
                            sbObject = afield[1];
                            property = afield[2];
                            originalValue = (original) ? original[sbObject][property] : '';
                        }
                        fieldName = sbObject + '.' + fieldId + '.' + property;
                    }
                    newValue = value.$modelValue
                    originalValue = (originalValue ? originalValue : '');
                    if (newValue != originalValue)
                    	fields.push({ "field": fieldName.toString(), "cambioA": newValue.toString(), "cambioDe": originalValue.toString() });
                }
            }
        });
        return fields;
    }

    return {
        getFormChanges: getFormChanges
    }
});
