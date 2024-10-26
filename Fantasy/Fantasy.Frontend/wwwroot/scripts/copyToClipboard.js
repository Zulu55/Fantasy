function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(function () {
        console.log('Texto copiado al portapapeles');
    }).catch(function (error) {
        console.error('Error al copiar al portapapeles: ', error);
    });
}