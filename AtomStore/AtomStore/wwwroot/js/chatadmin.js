class Message {
    constructor(username, text, when, receiverName) {
        this.Name = username;
        this.Text = text;
        this.When = when;
        this.ReceiverName = receiverName
    }
}

// userName is declared in razor page.
const username = document.getElementById('userName');
const textInput = document.getElementById('messageText');
const chat = document.getElementById('chat');
const messagesQueue = [];
var receiverName = document.getElementById('name');

$('#submitButton').click(() => {
    //$("#qnimate").stop().animate({ scrollTop: $("#qnimate")[0].scrollHeight }, 1000);
});

//$('.messager').click(function () {
//    var a = $(this).find('.messagerName').text();
//    $('.messager').removeClass('active');
//    $('#name').text(a);
//    $(this).toggleClass('active');
//});

function addMessage(message) {
    let isCurrentUserMessage = message.Name === username.value;
    let container = document.createElement('li');
    container.className = isCurrentUserMessage ? "msg-right" : "msg-left";

    let mess = document.createElement('div');
    mess.className = "msg-desc";
    mess.innerHTML = message.Text;
    let time = document.createElement('small');
    var date = new Date(message.When);
    time.innerHTML = atom.dateTimeFormatJson(date);
    let timediv = document.createElement('div');
    timediv.className = isCurrentUserMessage ? "time-right" : "time-left";
    timediv.appendChild(time);
    container.appendChild(mess);
    container.appendChild(timediv);
    $('.messageList').append(container);

}

function updateleftsecsion() {

}

function groupBy(objectArray, property) {
    return objectArray.reduce(function (acc, obj) {
        var key = obj[property];
        if (!acc[key]) {
            acc[key] = [];
        }
        acc[key].push(obj);
        return acc;
    }, {});
}
function clearInputField() {
    messagesQueue.push(textInput.value);
    textInput.value = "";
}

function sendMessage() {
    let text = messagesQueue.shift() || "";
    if (text.trim() === "") return;

    let when = new Date();
    let message = new Message(username.value, text, when, receiverName.innerText);
    sendMessageToHub(message);
}

function addMessageToChat(message) {
    if (message.name != username.value) {
        atom.notify('You have received a message from ' + message.name, 'success');
    }
    var name = $('#name').text();
    var isNewMessage = false;
    if (name == message.name || name == message.receiverName) {
        let isCurrentUserMessage = message.name === username.value;
        let container = document.createElement('li');
        container.className = isCurrentUserMessage ? "msg-right" : "msg-left";

        let mess = document.createElement('div');
        mess.className = "msg-desc";
        mess.innerHTML = message.text;
        let time = document.createElement('small');
        var messageTime = new Date(message.when);
        var formatted_time = atom.dateTimeFormatJson(messageTime)
        time.innerHTML = formatted_time;
        let timediv = document.createElement('div');
        timediv.className = isCurrentUserMessage ? "time-right" : "time-left";
        timediv.appendChild(time);
        container.appendChild(mess);
        container.appendChild(timediv);
        $('.messageList').append(container);
    }
    $('.messager').each(function (i) {
        var messagerName = $(this).find('.messagerName').text();
        var messageText = $(this).find('.messageText');
        var when = $(this).find('.time');
        var time = new Date(message.when);
        var formatted_time = time_format(time)
        if (messagerName == message.name || messagerName == message.receivername) {
            messageText.text(message.text);
            when.text(formatted_time);
            $('.left-section-ul').prepend($(this));
            isNewMessage = true;
        }
    });
    if (message.name == username.value) {
        isNewMessage = true;
    }
    
    if (!isNewMessage) {
        let container = document.createElement('li');
        container.className = "messager";

        let chatList = document.createElement('div');
        chatList.className = "chatList";

        let img = document.createElement('div');
        img.className = "img";
        let imgsrc = document.createElement('img');
        img.appendChild(imgsrc);

        let desc = document.createElement('div');
        desc.className = "desc";
        let time = document.createElement('small');
        time.className = "time";
        var messageTime = new Date(message.when);
        var formatted_time = time_format(messageTime)
        time.innerHTML = formatted_time;
        let h5 = document.createElement('h5');
        h5.className = "messagerName";
        h5.innerHTML = message.name;
        let text = document.createElement('small');
        text.className = "messageText";
        text.innerHTML = message.text;
        desc.appendChild(time);
        desc.appendChild(h5);
        desc.appendChild(text);
        chatList.appendChild(img);
        chatList.appendChild(desc);
        container.appendChild(chatList);
        $('.left-section-ul').prepend(container);
    }
}



function time_format(d) {
    hours = format_two_digits(d.getHours());
    minutes = format_two_digits(d.getMinutes());
    return hours + ":" + minutes;
}

function format_two_digits(n) {
    return n < 10 ? '0' + n : n;
}
function scrollToBottom() {
    chat.scrollTop = 1000;
}
function loaddata(a) {

    $.ajax({
        type: 'GET',
        url: '/admin/message/getall',
        DataType: 'json',
        beforeSend: function () {
            atom.startLoading();
        },
        success: function (response) {
            $.each(response, function (i, item) {
                if (item.Name == a) {
                    $('#receiverId').val(item.UserId)
                }
                if (item.Name == a || item.ReceiverName == a) {
                    addMessage(item);
                }

            })
            atom.stopLoading();
        },
        error: function (e) {
            atom.notify('Has an error in progress', 'error');
            atom.stopLoading();
        }
    });
}
