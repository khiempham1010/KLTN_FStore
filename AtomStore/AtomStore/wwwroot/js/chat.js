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

document.getElementById('submitButton').addEventListener('click', () => {
    $("#qnimate").stop().animate({ scrollTop: $("#qnimate")[0].scrollHeight }, 1000);
});

function clearInputField() {
    messagesQueue.push(textInput.value);
    textInput.value = "";
}

function sendMessage() {
    let text = messagesQueue.shift() || "";
    if (text.trim() === "") return;
    
    let when = new Date();
    let message = new Message(username.value, text,when);
    sendMessageToHub(message);
}

function addMessageToChat(message) {
    if (message.name != username.value) {
        atom.notify('You have received a message from ' + message.name, 'success');
    }
    let isCurrentUserMessage = message.name === username.value;

    let container = document.createElement('div');
    container.className =  "direct-chat-msg doted-border";

    let senderContainer = document.createElement('div');
    senderContainer.className = "direct-chat-info clearfix";

    let sender = document.createElement('span');
    sender.className = isCurrentUserMessage ? "direct-chat-name pull-right" : "direct-chat-name pull-left";
    sender.innerHTML = message.name;
    senderContainer.appendChild(sender);


    let text = document.createElement('div');
    text.className = isCurrentUserMessage ? "direct-chat-text" : "direct-chat-text2";
    text.innerHTML = message.text;


    let whenContainer = document.createElement('div');
    whenContainer.className = "direct-chat-info clearfix";

    let when = document.createElement('span');
    when.className = "direct-chat-timestamp pull-right";
    var currentdate = new Date();
    when.innerHTML = 
        (currentdate.getMonth() + 1) + "/"
        + currentdate.getDate() + "/"
        + currentdate.getFullYear() + " "
        + currentdate.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true })
    whenContainer.appendChild(when);

    container.appendChild(senderContainer);
    container.appendChild(text);
    container.appendChild(whenContainer);
    chat.appendChild(container);
}
