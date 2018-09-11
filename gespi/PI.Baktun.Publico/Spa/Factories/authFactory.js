/*
    page
*/

angular.module('authFactory', [])
    .factory('authService', function ($http, $q, $window) {
        var userIdentity;
        var urlBase = '/eRegistroAPI/AuthPublic/';

        function login(userName, password) {
            var deferred = $q.defer();

            $http.post(urlBase + "Login", { userName: userName, password: password })
                .then(
                    function (result) {
                        if (result.data.Succeeded) {
                            userIdentity = {
                                accessToken: result.data.Result.token,
                                userName: result.data.Result.Nombre,
                                email: result.data.Result.Email
                                //Marcas: Marcas,
                                //MarcasAux: MarcasAux,
                                //Patentes: Patentes,
                                //DerechoDeAutor: DerechoDeAutor,
                                //Recepcion: Recepcion
                            };
                            console.log('Public LogIn');
                            console.log(JSON.stringify(userIdentity));
                            $.jrzStorage.local.setItem("appPublicUser", userIdentity)
                            deferred.resolve(userIdentity);
                        }
                        else {
                            deferred.reject("-Cuenta no existe en el sistema-");
                        }
                    },
                    function (error) {
                        deferred.reject(error);
                    }
                );
            return deferred.promise;
        }

        function logout() {
            var deferred = $q.defer();

            $http({
                method: "POST",
                url: urlBase + "logout",
                headers: {
                    "access_token": userIdentity.accessToken
                }
            }).then(function (result) {
                userIdentity = null;
                $.jrzStorage.local.removeItem("appPublicUser");
                deferred.resolve(result);
            }, function (error) {
                deferred.reject(error);
            });

            return deferred.promise;
        }

        function getUserInfo() {
            return userIdentity;
        }

        function getToken() {
            return userIdentity.accessToken;
        }

        function api() {
            return '/eRegistroAPI';
        }


        function init() {
            if ($.jrzStorage.local.exists("appPublicUser")) {
                userIdentity = $.jrzStorage.local.getItem("appPublicUser");
            }
        }
        init();

        return {
            login: login,
            logout: logout,
            getUserInfo: getUserInfo,
            getToken: getToken,
            api: api
        };
    })

    .factory('listService', function ($http, $q, authService) {
        var lists;
        var urlBase = '/eRegistroAPI/Admin/Entities/';

        function fetchLists() {
            var deferred = $q.defer();

            console.log('get lists (eConsulta)...');

            $http.get(urlBase + 'Get?entity=all', { headers: { "access_token": authService.getToken() } })
                .then
                (
                    function (result) {                        
                        lists = result.data.Result;
                        $.jrzStorage.local.setItem("lists", lists)
                        deferred.resolve(lists);
                        console.log(JSON.stringify(lists.tipoDeRegistroDeMarcas));
                    },
                    function (error) {
                        deferred.reject(error);
                    }
                );
            return deferred.promise;
        }

        function getLists() {
            return lists;
        }

        function reFetchLists() {
            fetchLists();
            return lists;
        }

        function init() {
            if ($.jrzStorage.local.exists("lists")) {
                lists = $.jrzStorage.local.getItem("lists");
            }
            else {
                fetchLists();
            }
        }

        init();

        return {
            getLists: getLists,
            reFetchLists: reFetchLists
        };
    })
    .factory('classSrvc', function ($http, $q, authService) {

        var vienaList;

        function getNizaClass() {

            var niza = [];
            // create niza list   
            for (i = 0; i < 46; i++) {
                niza.push({ code: i, selected: false });
            }
            niza.push({ code: 99, selected: false });
            niza.push({ code: "All", selected: false });
            return niza;
        }

        function _fetchVienaClass() {
            var deferred = $q.defer();
            $http.get("/publico/content/json/viena.json")
                .then(
                    function (result) {
                        vienaList = result.data;
                        $.jrzStorage.local.setItem("vienaClass", vienaList)
                        deferred.resolve(vienaList);
                    },
                    function (error) {
                        deferred.reject(error);
                    }
                );
            return deferred.promise;
        }

        function getVienaClass() {
            if ($.jrzStorage.local.exists("vienaClass")) {
                vienaList = $.jrzStorage.local.getItem("vienaClass");
            }
            else {
                _fetchVienaClass();
            }
            return vienaList;
        }

        function _init() {
            console.log('classSrvc init...');
            _fetchVienaClass();
        }

        _init();
        

        function getGrupos() {
            return $http.get('/eRegistroAPI/Favoritos/GetGrupos', { headers: { "access_token": authService.getToken() } });
        };

        return {
            getNizaClass: getNizaClass,
            getVienaClass: getVienaClass,
            getGrupos: getGrupos
        };
    });


