class Message {
    constructor(username, text, when) {
        this.Name = username;
        this.Text = text;
        this.When = when;
    }
}

// userName is declared in razor page.
const username = document.getElementById('userName');
const textInput = document.getElementById('messageText');
const chat = document.getElementById('chat');
const messagesQueue = [];

$('#submitButton').click(() => {
    $("#qnimate").stop().animate({ scrollTop: $("#qnimate")[0].scrollHeight }, 1000);
});

$('.messager').click(function () {
    var a = $(this).find('.messagerName').text();
    $('.messager').removeClass('active');
    $('#name').text(a);
    $(this).toggleClass('active');
});

function addMessage(message) {
    let isCurrentUserMessage = message.Name === username.value;
    let container = document.createElement('li');
    container.className = isCurrentUserMessage ? "msg-right" : "msg-left";

    let mess = document.createElement('div');
    mess.className = "msg-desc";
    mess.innerHTML = message.Text;
    let time = document.createElement('small');
    time.innerHTML = message.When;
    container.appendChild(mess, time);
    $('.messageList').append(container);
}

function clearInputField() {
    messagesQueue.push(textInput.value);
    textInput.value = "";
}

function sendMessage() {
    let text = messagesQueue.shift() || "";
    if (text.trim() === "") return;

    let when = new Date();
    let message = new Message(username.value, text, when);
    sendMessageToHub(message);
}

function addMessageToChat(message) {
    let isCurrentUserMessage = message.name === username.value;
    let container = document.createElement('li');
    container.className = isCurrentUserMessage ? "msg-right" : "msg-left";

    let mess = document.createElement('div');
    mess.className = "msg-desc";
    mess.innerHTML = message.text;
    let time = document.createElement('small');
    time.innerHTML = message.when;
    container.appendChild(mess);
    container.appendChild(time);
    $('.messageList').append(container);
    scrollToBottom();
}
function scrollToBottom() {
    chat.scrollTop = 1000;
}
