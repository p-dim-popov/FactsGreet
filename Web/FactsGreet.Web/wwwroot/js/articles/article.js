(function (){
    new QRCode(document.getElementById("qrcode"), window.location.href);
    document.getElementById('qrcode').children[1].style.width = '30vw';

    new QRCode(document.getElementById("qrcode-md"), window.location.href);
    document.getElementById('qrcode-md').children[1].style.width = '10vw';
})()
