const feedbackTitle = document.getElementById('feedbackTitle');
const feedbackContend = document.getElementById('feedbackContend');
const ratingVal = document.getElementById('ratingVal');
var parent = parent;

var images = [];
$('body').on('click', '.btn-delete-image', function (e) {
    e.preventDefault();
    $(this).closest('div').remove();
});
$("#fileImage").on('change', function () {
    var fileUpload = $(this).get(0);
    var files = fileUpload.files;
    var data = new FormData();
    for (var i = 0; i < files.length; i++) {
        data.append(files[i].name, files[i]);
    }
    $.ajax({
        type: "POST",
        url: "/Admin/Upload/UploadImage",
        contentType: false,
        processData: false,
        data: data,
        success: function (path) {
            clearFileInput($("#fileImage"));
            images.push(path);
            $('#image-list').append('<div class="col-md-3" style="text-align:center;"><img width="100px; height:100px;"  data-path="' + path + '" src="' + path + '"><br><a href="#" class="btn-delete-image"> <i class="icon-close"> </i> </a> </div>');
            $('#imageInput').val(path);
            atom.notify('Upload successfully!', 'success');

        },
        error: function () {
            atom.notify('There was error uploading files!', 'error');
        }
    });
});

function reloadFeedback(productId) {
    $.ajax({
        type: "Get",
        url: "/Product/GetFeedback",
        dataType: 'json',
        data: {
            productId: productId
        },
        success: function (response) {
            var template = $('#template-feedback').html();
            var render = "";
            $.each(response, function (i, item) {
                render += Mustache.render(template,
                    {
                        FullName: item.AppUser.FullName,
                        Title: item.Title,
                        Content: item.Content,
                        Image: item.Image,
                        DateCreated: atom.dateTimeFormatJson( item.DateCreated),
                        Rating: addRating(item.Rating)
                    });
            });
            if (render !== "") {
                $('.reviews-content-left').empty();
                $('.reviews-content-left').html(render);
            }
            else {
                $('.reviews-content-left').html('No feedback yet!');
            }
        },
        error: function () {
            atom.notify('Load feedback error!', 'error');
        }
    });

}
function success() {
    atom.notify('Review successful', 'success');
};
function fail() {
    atom.notify('Has an error in progress', 'error');
};

function clearFileInput(ctrl) {
    try {
        ctrl.value = null;
    } catch (ex) { }
    if (ctrl.value) {
        ctrl.parentNode.replaceChild(ctrl.cloneNode(true), ctrl);
    }
}

function addRating(rate) {
    let text='';
    for (i = 0; i < rate; i++) {
        text += '<i class="fa fa-star"></i>';
    }
    for (i = 0; i < 5-rate; i++) {
        text += '<i class="fa fa-star-o"></i>';
    }
    return text;
}