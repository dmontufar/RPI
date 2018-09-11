function getDateddMMYYYY(date) {
    var pattern = /(\d{2})\/(\d{2})\/(\d{4})/;
    var day = makeDoubleDigit(date.getDate());
    var month = makeDoubleDigit(date.getMonth() + 1);
    var year = date.getFullYear();
    var dt = new Date((day + '/' + month + '/' + year).replace(pattern, '$3-$2-$1'));
    return dt;
}

function makeDoubleDigit(fecha){
    if (fecha.toString().length==1)
        return "0"+fecha;
    return fecha;
}
