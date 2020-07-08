var HomeController = function () {
    this.initialize = function () {
        registerEvents();
        jQuery('#rev_slider_4').show().revolution({
            dottedOverlay: 'none',
            delay: 5000,
            startwidth: 865,
            startheight: 450,

            hideThumbs: 200,
            thumbWidth: 200,
            thumbHeight: 50,
            thumbAmount: 2,

            navigationType: 'thumb',
            navigationArrows: 'solo',
            navigationStyle: 'round',

            touchenabled: 'on',
            onHoverStop: 'on',

            swipe_velocity: 0.7,
            swipe_min_touches: 1,
            swipe_max_touches: 1,
            drag_block_vertical: false,

            spinner: 'spinner0',
            keyboardNavigation: 'off',

            navigationHAlign: 'center',
            navigationVAlign: 'bottom',
            navigationHOffset: 0,
            navigationVOffset: 20,

            soloArrowLeftHalign: 'left',
            soloArrowLeftValign: 'center',
            soloArrowLeftHOffset: 20,
            soloArrowLeftVOffset: 0,

            soloArrowRightHalign: 'right',
            soloArrowRightValign: 'center',
            soloArrowRightHOffset: 20,
            soloArrowRightVOffset: 0,

            shadow: 0,
            fullWidth: 'on',
            fullScreen: 'off',

            stopLoop: 'off',
            stopAfterLoops: -1,
            stopAtSlide: -1,

            shuffle: 'off',

            autoHeight: 'off',
            forceFullWidth: 'on',
            fullScreenAlignForce: 'off',
            minFullScreenHeight: 0,
            hideNavDelayOnMobile: 1500,

            hideThumbsOnMobile: 'off',
            hideBulletsOnMobile: 'off',
            hideArrowsOnMobile: 'off',
            hideThumbsUnderResolution: 0,


            hideSliderAtLimit: 0,
            hideCaptionAtLimit: 0,
            hideAllCaptionAtLilmit: 0,
            startWithSlide: 0,
            fullScreenOffsetContainer: ''
        });

    }
    function registerEvents() {
        $('.btnAddToWishlist').on('click', function (e) {
            e.preventDefault();
            var id = parseInt($(this).data('id'));
            var color = $(this).find("i");
            if (color.css("color") == "rgb(255, 255, 255)") {
                $.ajax({
                    url: '/Product/AddWishlist',
                    type: 'post',
                    dataType: 'json',
                    data: {
                        productId: id
                    },
                    success: function () {
                        atom.notify('Product was added successful', 'success');
                        color.removeClass("fa-heart")
                        color.addClass("fa-heart");
                        color.css('color', 'rgb(255, 0, 0)');

                    },
                    error: function () {
                        atom.notify('Log in to add product', 'error');
                        atom.stopLoading();
                    }
                });
            }
            else {
                $.ajax({
                    url: '/Product/AddWishlist',
                    type: 'post',
                    dataType: 'json',
                    data: {
                        productId: id
                    },
                    success: function () {
                        atom.notify('Product was deleted successful', 'success');
                        color.removeClass("fa-heart")
                        color.addClass("fa-heart");
                        color.css('color', 'rgb(255, 255, 255)');

                    },
                    error: function () {
                        atom.notify('Log in to add product', 'error');
                        atom.stopLoading();
                    }
                });
            }
        });

        //$('#btnAddViewed').on('click', function (e) {
        //    e.preventDefault();
        //    var id = parseInt($(this).data('id'));
        //    $.ajax({
        //        url: '/Product/AddViewedlist',
        //        type: 'post',
        //        dataType: 'json',
        //        data: {
        //            productId: id
        //        },
        //        success: function () {
        //            atom.notify('Product was added successful', 'success');
        //        },
        //        error: function () {
        //            atom.notify('Log in to add product', 'error');
        //            atom.stopLoading();
        //        }
        //    });
        //});
    }
}
