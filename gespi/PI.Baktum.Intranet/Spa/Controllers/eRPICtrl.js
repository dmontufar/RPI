//var yumKaaxControllers = angular.module('eRPIControllers', []);

yumKaaxControllers.controller('eRPICtrl', function ($scope, authService, $http, $location) {
    $scope.isAdmin = false;
    $scope.isLoggedIn = false;
    $scope.estadisticasByArea=null;
    //$scope.$watch(authService.getUserInfo, function () { $scope.userIdentity = authService.getUserInfo(); $scope.isAdmin = ($scope.userIdentity.Role == 'SuperAdmin' || $scope.userIdentity.Role == 'Admin'); $scope.isLoggedIn = ($scope.userIdentity != null) });

    //$(document).ready(function () {
        GetEstadisticasByArea();        
    //});

    function GetEstadisticasByArea() {
        $http.get(authService.api() + '/Admin/Entities/GetEstadisticasByArea?year=' + 2014)
        .then(function (result) {
            showDAEstadisticas(result.data.DataSet[0]);
        });
        $http.get(authService.api() + '/Admin/Entities/GetIngresoExpedientesPorMes?year=' + 2014)
        .then(function (result) {
            showExpIngresoPorMes(result.data.DataSet);
        });
    };

    $scope.logout = function () {
        authService.logout()
            .then(function (result) {
                $scope.userIdentity = null;
                $location.path("/login");
            }, function (error) {
                console.log(error);
            });
    };
});

function showDAEstadisticas(std) { 

    var options = {
        //Boolean - Whether we should show a stroke on each segment
        segmentShowStroke: true,

        //String - The colour of each segment stroke
        segmentStrokeColor: "#fff",

        //Number - The width of each segment stroke
        segmentStrokeWidth: 2,

        //Number - The percentage of the chart that we cut out of the middle
        percentageInnerCutout: 0, // This is 0 for Pie charts

        //Number - Amount of animation steps
        animationSteps: 100,

        //String - Animation easing effect
        animationEasing: "easeOutBounce",

        //Boolean - Whether we animate the rotation of the Doughnut
        animateRotate: true,

        //Boolean - Whether we animate scaling the Doughnut from the centre
        animateScale: false,

        //String - A legend template
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<segments.length; i++){%><li><span style=\"background-color:<%=segments[i].fillColor%>\"></span><%if(segments[i].label){%><%=segments[i].label%><%}%></li><%}%></ul>"
    }

    var dataIngreso = [
        {
            value: std.DAIngreso,
            color: "#9400D3",
            highlight: "#9932CC",
            label: "Derecho de Autor"
        },
        {
            value: std.SignosIngreso,
            color: "#20B2AA",
            highlight: "#66CDAA",
            label: "Marcas"
        },
        {
            value: std.PatentesIngreso,
            color: "#C71585",
            highlight: "#FF69B4",
            label: "Patentes"
        }
    ]
    var dataPublicacion = [
        {
            value: std.DAPublicacion,
            color: "#228B22",
            highlight: "#3CB371",
            label: "Derecho de Autor"
        },
        {
            value: std.SignosPublicacion,
            color: "#B8860B",
            highlight: "#DAA520",
            label: "Marcas"
        },
        {
            value: std.PatentesPublicacion,
            color: "#8B0000",
            highlight: "#B22222",
            label: "Patentes"
        }
    ]
    var dataTitulo = [
        {
            value: std.DAConTitulo,
            color: "#00008B",
            highlight: "#0000FF",
            label: "Derecho de Autor"
        },
        {
            value: std.SignosConTitulo,
            color: "#FF0000",
            highlight: "#FF7F50",
            label: "Marcas"
        },
        {
            value: std.PatentesConTitulo,
            color: "#006400",
            highlight: "#008000",
            label: "Patentes"
        }
    ]

    var ctxIngreso = document.getElementById("stdIngreso").getContext("2d");
    var ctxPublicacion = document.getElementById("stdPublicacion").getContext("2d");
    var ctxTitulo = document.getElementById("stdTitulo").getContext("2d");

    var myChart = new Chart(ctxIngreso).Pie(dataIngreso, options);
    var myChart = new Chart(ctxPublicacion).Pie(dataPublicacion, options);
    var myChart = new Chart(ctxTitulo).Pie(dataTitulo, options);

    //document.getElementById("legend").innerHTML = myChart.generateLegend();

    myChart.update();
}


function showExpIngresoPorMes(std) {

    var options = {
        //Boolean - Whether the scale should start at zero, or an order of magnitude down from the lowest value
        scaleBeginAtZero: true,

        //Boolean - Whether grid lines are shown across the chart
        scaleShowGridLines: true,

        //String - Colour of the grid lines
        scaleGridLineColor: "rgba(0,0,0,.05)",

        //Number - Width of the grid lines
        scaleGridLineWidth: 2,

        //Boolean - Whether to show horizontal lines (except X axis)
        scaleShowHorizontalLines: true,

        //Boolean - Whether to show vertical lines (except Y axis)
        scaleShowVerticalLines: true,

        //Boolean - If there is a stroke on each bar
        barShowStroke: true,

        //Number - Pixel width of the bar stroke
        barStrokeWidth: 1,

        //Number - Spacing between each of the X value sets
        barValueSpacing: 5,

        //Number - Spacing between data sets within X values
        barDatasetSpacing: 1,

        //String - A legend template
        legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].fillColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>"
    };

    var _DAporMes = [];
    var _PATporMes = [];
    var _SIGporMes = [];
    for (var i = 0; i < 12; i++) {
        _SIGporMes[i] = _PATporMes[i] = _DAporMes[i] = 0;
    }

    for (var i = 0; i < std.length; i++) {
        var item = std[i];
        if (item.ModuloId==1){
            _SIGporMes[item.MES-1]=item.Conteo;
        }
        else if (item.ModuloId==2){
            _PATporMes[item.MES-1]=item.Conteo;
        }
        else if (item.ModuloId==3){
            _DAporMes[item.MES-1]=item.Conteo;
        }
    }

    var DAIngreso = {
        labels: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
        datasets: [
            {
                label: "Derecho de Autor",
                fillColor: "rgba(1, 163, 28, 1)",
                strokeColor: "rgba(220,220,220,0.8)",
                highlightFill: "rgba(220,220,220,0.75)",
                highlightStroke: "rgba(220,220,220,1)",
                data: _DAporMes
            },
        ]
    };

    var PATIngreso = {
        labels: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
        datasets: [
            {
                label: "Derecho de Autor",
                fillColor: "rgba(0, 114, 188, 1)",
                strokeColor: "rgba(220,220,220,0.8)",
                highlightFill: "rgba(220,220,220,0.75)",
                highlightStroke: "rgba(220,220,220,1)",
                data: _PATporMes
            },
        ]
    };

    var SIGIngreso = {
        labels: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
        datasets: [
            {
                label: "Derecho de Autor",
                fillColor: "rgba(102, 45, 145, 1)",
                strokeColor: "rgba(220,220,220,0.8)",
                highlightFill: "rgba(220,220,220,0.75)",
                highlightStroke: "rgba(220,220,220,1)",
                data: _SIGporMes
            },
        ]
    };


    var ctxDA = document.getElementById("DAExpMensual").getContext("2d");
    var ctxPAT = document.getElementById("PATExpMensual").getContext("2d");
    var ctxSIG = document.getElementById("SIGExpMensual").getContext("2d");

    var myChart1 = new Chart(ctxDA).Bar(DAIngreso, options);
    var myChart2 = new Chart(ctxPAT).Bar(PATIngreso, options);
    var myChart3 = new Chart(ctxSIG).Bar(SIGIngreso, options);

    //document.getElementById("legend").innerHTML = myChart.generateLegend();

    myChart1.update();
    myChart2.update();
    myChart3.update();
}