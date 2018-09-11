(function () {
    'use strict';

    var $ = window.jQuery || window.$ || (window.$ = {});

    /* 
     * 
     */
    function jrzStorage() {
        var isLocalStorageSupported = false;
        if ('localStorage' in window) {
            try {
                window.localStorage.setItem('__test', '__test');
                isLocalStorageSupported = true;
                window.localStorage.removeItem('__test');
            } catch (e) {
                console.log(e.message);
            }
        }
        else {
            // display err
        }
    }

    $.jrzStorage = {
        local: {
            exists: function (key) {
                return (localStorage[key]);
            },
            setItem: function (key, value, timeSpan) {
                // maybe add expiration to it
                try {
                    localStorage.setItem(key, JSON.stringify(value))
                }
                catch (e) {
                    console.log(e.message);
                    return null;
                }
                return value;
            },

            getItem: function (key) {
                try {
                    var value = JSON.parse(localStorage.getItem(key));
                }
                catch (e) {
                    console.log(e.message);
                    return null;
                }
                return value;
            },

            removeItem: function (key) {
                try {
                    localStorage.removeItem(key);
                    return true;
                }
                catch (e) {
                    console.log(e.message);
                }
                return false;
            },


            clear: function () {
                try {
                    localStorage.clear();
                }
                catch (e) {
                    console.log(e.message);
                }

                return true;
            }
        },

        session: {
            exists: function (key) {
                return (sessionStorage[key]);
            },
            setItem: function (key, value, timeSpan) {
                // maybe add expiration to it
                try {
                    sessionStorage.setItem(key, JSON.stringify(value))
                }
                catch (e) {
                    console.log(e.message);
                    return null;
                }
                return value;
            },

            getItem: function (key) {
                try {
                    var value = JSON.parse(sessionStorage.getItem(key));
                }
                catch (e) {
                    console.log(e.message);
                    return null;
                }
                return value;
            },

            removeItem: function (key) {
                try {
                    sessionStorage.removeItem(key);
                    return true;
                }
                catch (e) {
                    console.log(e.message);
                }
                return false;
            },


            clear: function () {
                try {
                    sessionStorage.clear();
                }
                catch (e) {
                    console.log(e.message);
                }

                return true;
            }
        }

    };

    // Initialize
    jrzStorage();

})();