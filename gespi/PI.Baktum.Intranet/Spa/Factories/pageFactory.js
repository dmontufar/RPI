/*
    page
*/

angular.module('pageFactory', [])
    .factory('Page', function () {
        var title = 'default';
        return {
            getTitle: function () { console.log('get title'); return title; },
            setTitle: function (newTitle) { console.log('set title'); title = newTitle }
        };
    });