var hareketsizSüre = 0;

// 1 saniyede bir hareketsiz süreyi artır
var zamanlayıcı = setInterval(zamanArtır, 1000);

// Kullanıcı hareket ettikçe hareketsiz süreyi sıfırla
$(document).on('mousemove keydown', function () {
    hareketsizSüre = 0;
});

function zamanArtır() {
    hareketsizSüre++;

    // Eğer 5 dakika (300 saniye) geçtiyse
    if (hareketsizSüre >= 300) {
        clearInterval(zamanlayıcı); // Zamanlayıcıyı durdur
        window.location.href = '/Login/Logout'; // Çıkış işlemi
    }
}


