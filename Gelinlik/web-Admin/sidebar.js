$(document).ready(function() {
    // Menü toggle işlevi
    $('.menu-toggle').click(function() {
        $('.sidebar').toggleClass('active');
        $('.overlay').toggleClass('active');
    });

    $('.overlay').click(function() {
        $('.sidebar').removeClass('active');
        $('.overlay').removeClass('active');
    });

    // Dropdown menü işlevi
    $('.dropdown-toggle').click(function(e) {
        e.preventDefault();
        const $dropdownMenu = $(this).next('.dropdown-menu');
        const $arrow = $(this).find('.arrow');
        
        // Diğer açık menüleri kapat
        $('.dropdown-menu').not($dropdownMenu).slideUp(300);
        $('.arrow').not($arrow).removeClass('rotate');
        
        // Seçili menüyü aç/kapat
        $dropdownMenu.slideToggle(300);
        $arrow.toggleClass('rotate');
    });
});