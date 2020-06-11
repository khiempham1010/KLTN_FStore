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
            $('#image-list').append('<div class="col-md-3"><img width="100"  data-path="' + path + '" src="' + path + '"><br><a href="#" class="btn-delete-image">Delete</a></div>');
            $('#imageInput').val(path);
            atom.notify('Upload successfully!', 'success');

        },
        error: function () {
            atom.notify('There was error uploading files!', 'error');
        }
    });
});

function reloadFeedback(username) {

    let container = document.createElement('div');
    container.className = "review-ratting";

    let p = document.createElement('p');
    let a = document.createElement('a');
    a.innerHTM = feedbackTitle.text();
    p.appendChild(a);
    let table = document.createElement('table');
    let tbody = document.createElement('tbody');
    let tr = document.createElement('tr');
    let th = document.createElement('th');
    tr.appendChild(th);
    let td = document.createElement('td');
    let divStar=document.createElement('div')
    divStar.className = "rating";
    for (i = 0; i < ratingVal; i++) {
        let star = document.createElement('i');
        star.className = "fa fa-star";
        divStar.appendChild(star);
    }
    for (i = 0; i < 5 - ratingVal; i++) {
        let star = document.createElement('i');
        star.className = "fa fa-star-o";
        divStar.appendChild(star);
    }
    td.appendChild(divStar);
    tr.appendChild(td);
    tbody.appendChild(tr);
    table.appendChild(tbody);
    let pContend = document.createElement('p');
    pContend.innerHTM = feedbackContend.text();
    container.appendChild(pContend);
    let pAuthor = document.createElement('p');
    pAuthor.className = "author";
    pAuthor.innerHTM = username;
    let small = document.createElement('small');
    var time = new Date();
    var formatted_time = atom.dateTimeFormatJson(time)
    small.innerHTML ="Posted on " + formatted_time;
    pAuthor.appendChild(small);
    container.appendChild(pAuthor);
    $('.reviews-content-left').append(container);
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