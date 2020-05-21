var connection = new signalR.HubConnectionBuilder()
    .withUrl('/Chatter')
    .build();

connection.on('receiveMessage', addMessageToChat);

connection.start()
    .catch(error => {
        console.log(error.message);
    });


function sendMessageToHub(message) {
    connection.invoke('sendMessage', message).catch(err => console.error(err.toString()));
}