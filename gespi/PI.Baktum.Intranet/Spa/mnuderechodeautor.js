var status = 130;
var getMnuOrder = function (status, ) { return this._estatus.indexOf(status) };

var mnu =
[
{ opcion: "Se declara con lugar	", view: "myview.html", _estatus: [130], migra: [], stl:"tile-yellow"},
{ opcion: "Rechazo	", view: "myview.html", _estatus: [130], migra: [], stl:"tile-green" },
{ opcion: "Suspenso	", view: "myview.html", _estatus: [130], migra: [], stl:"tile-red" },
{ opcion: "Operar Memorial	", view: "myview.html", _estatus: [131], migra: [02, 03, 04], stl:"tile-orange" },
{ opcion: "Levantar Suspenso	", view: "myview.html", _estatus: [133], migra: [02, 03, 04], stl:"tile-greenDark" },
{ opcion: "Operar Memorial	", view: "myview.html", _estatus: [133], migra: [02, 03, 04], stl:"tile-pink" },
{ opcion: "Se declara con lugar	", view: "myview.html", _estatus: [134], migra: [02, 03, 04], stl:"tile-blue" },
{ opcion: "Reposicion de Titulo	", view: "myview.html", _estatus: [135], migra: [02, 03, 04], stl:"tile-yellow" },
{ opcion: "Recurso de Revocatoria	", view: "myview.html", _estatus: [135], migra: [02, 03, 04], stl:"tile-green" },
{ opcion: "Elevando Recurso Revocatoria	", view: "myview.html", _estatus: [136], migra: [02, 03, 04], stl:"tile-yellow" },
{ opcion: "Por recibido MINECO	", view: "myview.html", _estatus: [137], migra: [02, 03, 04], stl:"tile-greenDark" },
{ opcion: "Se declara con lugar	", view: "myview.html", _estatus: [138], migra: [02, 03, 04], stl:"tile-blue" },
{ opcion: "Notificar	", view: "myview.html", _estatus: [140], migra: [02, 03, 04], stl:"tile-orange" },
{ opcion: "Emitir Titulo (Registro)	", view: "myview.html", _estatus: [140, 141], migra: [02, 03, 04], stl:"tile-blue" },
{ opcion: "Cambiar Estatus", view: "myview.html", _estatus: [], migra: []}
];