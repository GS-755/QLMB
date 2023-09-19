$('.carousel').each(function () {

    var currentCasouseID = '#' + $(this).attr('id');

    const multipleItemCarousel = document.querySelector(currentCasouseID)


    if (window.matchMedia("(min-width:576px)").matches) {
        const carousel = new bootstrap.Carousel(multipleItemCarousel,
            {
                interval: false
            })

        var carouseWidth = $(currentCasouseID + ' .carousel-inner')[0].scrollWidth;
        var cardWidth = $(currentCasouseID + ' .carousel-item').width();

        var scrollPosition = 0;
        $(currentCasouseID + ' .carousel-control-next').on('click', function () {
            if (scrollPosition < (carouseWidth - (cardWidth * 4))) {
                console.log('next');
                scrollPosition = scrollPosition + cardWidth;
                $(currentCasouseID + ' .carousel-inner').animate({ scrollLeft: scrollPosition }, 600);
            }
            else {
                scrollPosition = 0 - cardWidth;
            }
        });

        $(currentCasouseID + ' .carousel-control-prev').on('click', function () {
            if (scrollPosition > 0) {
                console.log('prev');
                scrollPosition = scrollPosition - cardWidth;
                $(currentCasouseID + ' .carousel-inner').animate({ scrollLeft: scrollPosition }, 600);
            }
        });
    }
    else {
        $(multipleItemCarousel().addClass('slide'));
    }


});

